using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Breakable : MonoBehaviour
{
    public float explodeDistance;
    public float explodeTime;
    public UnityEvent BreakEvent;
    public bool enemy = false;
    public bool breakable = true; // Flag to check if the object is breakable
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!breakable) return; // If the object is not breakable, do nothing
        // Check if the object colliding with this has the tag "Player"
        if (collision.CompareTag("Sword"))
        {
            Destroy(GetComponent<Rigidbody2D>()); // Set the Rigidbody to kinematic to prevent further physics interactions
            Destroy(GetComponent<Collider2D>()); // Destroy the collider to prevent further collisions
            StartCoroutine(BreakAway(collision.transform.position));
        }
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
}
