using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private void FixedUpdate()
    {
        UpdateState();
        
    }
    
    private void UpdateState()
    {
        _alive = _health > 0;
    }
    
    private void ApplyDamage(int damage)
    {
        if (_invincible) return;
        _health -= damage;
        _invincible = true;
        Invoke(nameof(ResetInvincibility), _invincibilityDurationAfterDamaged);
    }
    
    private void ResetInvincibility()
    {
        _invincible = false;
    }

    private PlayerMovement _lazyPlayerMovement;

    private PlayerMovement PlayerMovement
    {
        get
        {
            if (_lazyPlayerMovement != null) return _lazyPlayerMovement;

            _lazyPlayerMovement = GetComponent<PlayerMovement>();
            return _lazyPlayerMovement;
        }
    }

}
