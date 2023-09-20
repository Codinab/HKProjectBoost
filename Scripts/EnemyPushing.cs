using System;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyPushing : MonoBehaviour
{
    public float pushPower = 10f;
    public float pushDelay = 0.5f;
    
    private bool _canPush = true;
    private bool _playerInTrigger = false;

    public GameObject player;
    private PlayerMovement _playerMovement;
    private PlayerCombat _playerCombat;

    private void Start()
    {
        _playerMovement = player.GetComponent<PlayerMovement>();
        _playerCombat = player.GetComponent<PlayerCombat>();
        
        if (_playerMovement == null || _playerCombat == null)
        {
            Debug.LogError("PlayerMovement or PlayerCombat not found on player");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _playerInTrigger = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _playerInTrigger = false;
    }

    private void EnablePushing()
    {
        _canPush = true;
    }

    private void FixedUpdate()
    {
        PushPlayer();
    }

    private void PushPlayer()
    {
        if (!_canPush || !_playerInTrigger) return;
        
        Vector2 pushDirection = this.player.transform.position - transform.position;
        _playerMovement.GetPushedByEnemy(pushDirection.normalized, pushPower);
        _playerCombat.ApplyDamage(1);
        _canPush = false;
        Invoke(nameof(EnablePushing), pushDelay);
    }
}