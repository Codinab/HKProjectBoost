using UnityEngine;

public class Camera : MonoBehaviour
{


    void Update()
    {
        //Follow the player
        GameObject player = GameObject.Find("Player");
        if (player == null) return;
        
        Vector2 playerPosition = player.transform.position;

        transform.position = new Vector3(playerPosition.x, playerPosition.y + 2f, transform.position.z);
    }
}
