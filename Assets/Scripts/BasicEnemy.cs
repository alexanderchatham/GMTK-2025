using System.Collections;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    public Transform player; // Reference to the player transform
    private Breakable Breakable; // Reference to the Breakable component
    public float speed = 3f;
    private void Start()
    {
        Breakable = GetComponent<Breakable>(); // Get the Breakable component attached to this enemy
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object colliding with this has the tag "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            // The player has tripped our radius so now the  enemy will start to chase the player
            Debug.LogWarning("Enemy has detected the player!");
            player = collision.transform; // Store the player's transform for movement reference
            transform.GetChild(0).gameObject.SetActive(false); // deactivate the sense object
            StartCoroutine(ChasePlayer()); // Start the chase coroutine
            GetComponent<Breakable>().breakable = true;
        }
    }
    IEnumerator ChasePlayer()
    {
        // This coroutine will handle the enemy's chase behavior
        while (true)
        {
            if(Breakable.KnockedBack)
                yield return new WaitForEndOfFrame(); // Wait for the next frame
            else
            {

                // Let's use the same code we are using the move the player to move the enemy in the direction of the player
                Vector3 direction = (player.transform.position - transform.position);
                if(direction.magnitude > 50f) // If the player is too far away, stop chasing
                {
                    transform.GetChild(0).gameObject.SetActive(true); // deactivate the sense object
                    yield break; // Exit the coroutine
                }
                direction.y = 0; // Ensure the enemy moves only in the XZ plane (2D movement)
                float step = speed * Time.deltaTime; // Calculate the step size based on speed and time
                // Move the enemy towards the player
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);
                yield return new WaitForEndOfFrame(); // Wait for the next frame
            }
        }
    }
}