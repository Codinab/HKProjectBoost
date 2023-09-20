using UnityEngine;

public class EnemyPushing : MonoBehaviour
{
    public float pushPower = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player == null) return; // means the enemy collided with the player
        Vector2 pushDirection = other.transform.position - transform.position;
        player.GetPushedByEnemy(pushDirection.normalized, pushPower);
    }
}