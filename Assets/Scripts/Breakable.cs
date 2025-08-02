using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Breakable : MonoBehaviour
{
    public float explodeDistance;
    public float explodeTime;
    public UnityEvent BreakEvent;
    public bool KnockedBack = false; // Flag to check if the object has been knocked back
    public bool enemy = false;
    public bool breakable = true; // Flag to check if the object is breakable
    public int health = 1; // Health of the breakable object, can be used to determine how many hits it can take before breaking
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!breakable) return; // If the object is not breakable, do nothing
        // Check if the object colliding with this has the tag "Player"
        if (collision.CompareTag("Sword"))
        {
            health--; // Decrease health by 1 when hit by the sword
            if (health <= 0) // If health is less than or equal to 0, break the object
            {
                Break(collision);
            }
            else
            {
                // Optionally, you can add a visual or audio feedback for the hit
                Debug.Log("Breakable object hit! Remaining health: " + health);
                if(enemy)
                {
                    // If this is an enemy breakable object, you can add additional logic here
                    // For example, you might want to play a hit animation or sound
                    Knockback(collision.transform.position); // Apply knockback effect
                }
            }
        }
    }
    public void SpecialBreak()
    {
        StartCoroutine(BreakAway(transform.position+Vector3.left*3+Vector3.down*3)); // Call the BreakAway coroutine to handle the break effect
    }
    private void Break(Collider2D collision)
    {
        Destroy(GetComponent<Rigidbody2D>()); // Set the Rigidbody to kinematic to prevent further physics interactions
        Destroy(GetComponent<Collider2D>()); // Destroy the collider to prevent further collisions
        StartCoroutine(BreakAway(collision.transform.position));
        BreakEvent?.Invoke(); // Invoke the break event if there are any listeners
    }

    IEnumerator BreakAway(Vector3 explosionPosition)
    {
        // Use a coroutine to handle the breakaway effect with a float timer a while loop and lerping the breakable away while adjusting its alpha
        float timer = 0f;
        float duration = explodeTime; // Duration of the breakaway effect
        var direction = (transform.position - explosionPosition).normalized;
        Vector3 endPosition = transform.position + direction * explodeDistance;
        Vector3 startPosition = transform.position;
        Vector3 gravityEffect = Vector3.zero;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            // Apply gravity effect if needed (e.g., simulate falling)
            gravityEffect += new Vector3(0,Physics2D.gravity.y) * Time.deltaTime; // Simulate gravity effect
            float t = timer / duration;
            // Lerp the position away from the explosion point
            transform.position = Vector3.Lerp(startPosition, endPosition+gravityEffect, t);
            // Adjust the alpha of the sprite
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(1f, 0f, t);
            spriteRenderer.color = newColor;
            yield return null; // Wait for the next frame
        }
        Destroy(gameObject); // Destroy the breakable object after the effect is complete
    } 
    public void Knockback(Vector3 explosionPosition)
    {
        StopAllCoroutines(); // Stop any ongoing breakaway coroutine to prevent conflicts
        KnockedBack = true; // Set the flag to true to prevent multiple knockbacks
        // Apply a knockback effect to the breakable object using the rigidbody component
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 direction = (transform.position - explosionPosition).normalized; // Calculate the direction away from the explosion
            rb.AddForce(direction * explodeDistance/3+Vector2.up*5, ForceMode2D.Impulse); // Apply an impulse force in that direction
        }
        else
        {
            Debug.LogWarning("No Rigidbody2D component found on the breakable object for knockback.");
        }
        StartCoroutine(KnockbackCooldown()); // Start the cooldown coroutine to reset the knocked back flag
    }
    IEnumerator KnockbackCooldown()
    {
        yield return new WaitForSeconds(1f); // Cooldown duration before allowing another knockback
        KnockedBack = false; // Reset the knocked back flag
    }
    public float a, b;
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {

        if (breakable)
        {
            Gizmos.color = Color.red;
            DrawTrajectory(transform.position, transform.position+Vector3.left, explodeDistance, explodeTime, true);
        }
    }

    private void DrawTrajectory(Vector3 start, Vector3 source, float distance, float duration, bool isBreakaway)
    {
        int segments = 30;
        Vector3 direction = (start - source).normalized;
        Vector3 gravityEffect = Vector3.zero;

        Vector3 end = start + direction * distance;
        Vector3 prev = start;

        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            float segmentDuration = duration / segments;

            if (isBreakaway)
            {
                gravityEffect += new Vector3(0, Physics2D.gravity.y) * segmentDuration;
            }

            Vector3 point = Vector3.Lerp(start, end + gravityEffect, t);
            Gizmos.DrawLine(prev, point);
            prev = point;
        }
    }
#endif
}

