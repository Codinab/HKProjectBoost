using System;
using UnityEngine;

public class Camera : MonoBehaviour
{
    private const float MaxHeight = 13.5f;
    
    public GameObject player;

    private void Start()    
    {
        if (player == null)
        {
            Debug.LogError("Player not found");
        }
    }

    void Update()
    {
        // Follow the player
        if (player == null) return;

        Vector2 playerPosition = player.transform.position;
        
        float newYPosition = Mathf.Min(playerPosition.y + 2f, MaxHeight);
        transform.position = new Vector3(playerPosition.x, newYPosition, transform.position.z);
    }
}