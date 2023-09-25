using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static PlayerMovement;

public class PlayerCombat : MonoBehaviour
{
    
    private int _health = 5;
    private int _maxHealth = 5;
    private int _damage = 1;
    private float _invincibilityDurationAfterDamaged = 3f;
    private bool _alive = true;
    private bool _invincible = false;
    
    

    public bool IsInvincible()
    {
        return _invincible;
    }
    
    public bool Alive
    {
        get => _alive;
        private set => _alive = value;
    }

    private void Start()
    {
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement not found on player");
        }
    }

    private void FixedUpdate()
    {
        UpdateState();
        
        if (CanAttack()) AttackLookingDirection();
        
    }
    
    public Text healthText; // This field will hold the reference to the HealthText UI element.

    private void Awake()
    {
        if (healthText == null)
            Debug.LogError("HealthText is not assigned in PlayerCombat!");
    }
    private void UpdateState()
    {
        _alive = _health > 0;

        // Update health display
        if (healthText != null)
            healthText.text = "Health: " + _health;
    }
    
    public void ApplyDamage(int damage)
    {
        if (_invincible) return;
        _health -= damage;
        SetInvincible();
        Invoke(nameof(ResetInvincibility), _invincibilityDurationAfterDamaged);
    }

    private void SetInvincible()
    {
        playerMovement.ResetJumps();
        _invincible = true;
    }

    private void ResetInvincibility()
    {
        _invincible = false;
    }
    
    private List<GameObject> _objectsInAttackRange = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Enemy")) return;
        _objectsInAttackRange.Add(other.gameObject);
        Debug.Log(other.gameObject.name + " entered player trigger");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Enemy")) return;
        _objectsInAttackRange.Remove(other.gameObject);
        Debug.Log(other.gameObject.name + " exited player trigger");
    }

    private bool _usedAttackKey = false;
    private bool CanAttack()
    {
        bool attackKey = Input.GetKey(KeyCode.C);
        if (attackKey && !_usedAttackKey)
        {
            return true;
        }
        if (!attackKey) _usedAttackKey = false;
        return false;
    }

    private bool _attacked = false;
    private void AttackLookingDirection()
    {
        if (_attacked) return;
        _attacked = true;
        _usedAttackKey = true;
        
        Debug.Log("Attack");
        
        Vector2Int lookingDirection = playerMovement.GetLookingDirection();
        
        foreach (GameObject gameObject in _objectsInAttackRange)
        {
            if (gameObject.CompareTag("Enemy") && EnemyInAttackDirection(lookingDirection, gameObject))
            {
                // TODO: Apply damage to enemy
                gameObject.GetComponent<MeshRenderer>().enabled = !gameObject.GetComponent<MeshRenderer>().isVisible;
            }
        }
        Debug.Log(lookingDirection);
        Vector2 pushBackDirection = lookingDirection;
        pushBackDirection.x = -pushBackDirection.x;
        playerMovement.GetPushed(pushBackDirection, 0.1f);
        Invoke(nameof(ResetAttack), 0.25f);
        Invoke(nameof(Attacking), 0);
        Invoke(nameof(StopAttackInProgress), 0.1f);
    }

    private bool _attackInProgress = false;
    private void Attacking()
    {
        _attackInProgress = true;
        while (_attackInProgress)
        {
            Vector2Int lookingDirection = playerMovement.GetLookingDirection();
        
            foreach (GameObject enemy in _objectsInAttackRange)
            {
                if (enemy.CompareTag("Enemy") && EnemyInAttackDirection(lookingDirection, enemy))
                {
                    // TODO: Apply damage to enemy
                    enemy.GetComponent<MeshRenderer>().enabled = !enemy.GetComponent<MeshRenderer>().isVisible;
                }
            }
        }
    }

    private void StopAttackInProgress()
    {
        _attackInProgress = false;
    }

    private void ResetAttack()
    {
        _attacked = false;
    }
    
    private bool EnemyInAttackDirection(Vector2Int lookingDirection, GameObject enemy)
    {
        Vector2 enemyPosition = enemy.transform.position;
        Vector2 playerPosition = transform.position;
        Vector2 attackDirection = enemyPosition - playerPosition;

        Vector2 approximatedDirection;

        if (Mathf.Abs(attackDirection.x) > Mathf.Abs(attackDirection.y))
        {
            // x-axis is dominant
            approximatedDirection = new Vector2(Mathf.Sign(attackDirection.x), 0);
        }
        else
        {
            // y-axis is dominant
            approximatedDirection = new Vector2(0, Mathf.Sign(attackDirection.y));
        }

        return approximatedDirection == (Vector2)lookingDirection;
    }
    
    public PlayerMovement playerMovement;
}
