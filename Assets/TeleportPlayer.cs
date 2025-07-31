using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    public Transform player;
    public Transform teleportTarget; // The target position to teleport the player to
    public void Teleport()
    {
        if (teleportTarget != null)
        {
            // Teleport the player to the target position
            player.position = teleportTarget.position;
            // Optionally, reset the player's velocity if using Rigidbody
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; // Reset velocity to prevent movement after teleport
            }
        }
        else
        {
            Debug.LogWarning("Teleport target is not set.");
        }
    }
}
