using System;
using System.Collections;
using System.Collections.Generic;
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
    
    
    
    

    public PlayerMovement playerMovement;
}
