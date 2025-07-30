using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    public float speed = 5f;
    public float MaxSpeed = 1f;
    public Vector3 moveDirection;
    public bool jumping = false;
    public bool dashed = false;
    public bool onLadder = false; // Added for ladder functionality
    int lastMoveIndicator = 0;
    public GameObject spawnPoint;
    Rigidbody2D rb;
    private Interactable currentInteractable;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on the character.");
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Interactable"))
        {
            currentInteractable = collision.GetComponent<Interactable>();
            if (currentInteractable != null)
            {
                currentInteractable.Show();
            }
        }
        if (collision.gameObject.CompareTag("Ladder"))
        {
            rb.linearVelocity = Vector2.zero; // Stop the character when entering a ladder
            rb.gravityScale = 0; // Disable gravity while on the ladder
            jumping = false; // Reset jumping state when on a ladder
            dashed = false; // Reset dashed state when on a ladder
            onLadder = true; // Set onLadder to true when entering a ladder
            print("Entered ladder: " + collision.gameObject.name);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Interactable"))
        {
            if (currentInteractable != null)
            {
                currentInteractable.Hide();
                currentInteractable = null; // Clear the reference when exiting
            }
        }
        if (collision.gameObject.CompareTag("Ladder"))
        {
            print("Exited ladder: " + collision.gameObject.name);
            rb.gravityScale = 1; // Re-enable gravity when exiting the ladder
            onLadder = false; // Set onLadder to false when exiting a ladder
        }
    }
    public void OnInteract()
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
            currentInteractable.Hide();
            currentInteractable = null; // Clear the reference after interaction
        }
    }
    public void OnMove(InputValue value)
    {
        var movement = value.Get<Vector2>();
        if (movement != Vector2.zero)
        {
            // Move the character based on the input
            moveDirection = new Vector3(movement.x * speed, movement.y, 0);
            if (movement.x > 0)
            {
                lastMoveIndicator = 1; // Right
            }
            else if (movement.x < 0)
            {
                lastMoveIndicator = -1; // Left
            }
            else
            {
                lastMoveIndicator = 0; // No horizontal movement
            }
        }
        else
        {
            // Stop the character if no input is given
            moveDirection = Vector3.zero;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            LandOnGround();
        }
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            // Attach to the moving platform
            transform.SetParent(collision.transform);
            LandOnGround();
        }
        if (collision.gameObject.CompareTag("Lava"))
        {
            // Reset jumping state when colliding with the ground
            jumping = false;
            dashed = false; // Reset dashed state when touching the ground
            transform.position = spawnPoint.transform.position; // Reset position to spawn point
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            // Detach from the moving platform
            transform.SetParent(null);
        }
    }

    private void LandOnGround()
    {
        // Reset jumping state when colliding with the ground
        jumping = false;
        dashed = false; // Reset dashed state when touching the ground
        FindHorizontalVelocity();
    }

    public void OnJump()
    {
        if(jumping && dashed)
            return; // Prevent jumping if already in the air
        else if (jumping &&!dashed)
        {
            dashed = true; // Allow jumping only if not already dashed
            rb.AddForceX(5f * lastMoveIndicator, ForceMode2D.Impulse);
        }
        jumping = true;
        rb.AddForceY(5f, ForceMode2D.Impulse);
    }
    public void Update()
    {

        //transform.Translate(moveDirection * Time.deltaTime);
        //Instead of translate use rb to apply movement
        if (rb != null)
        {
            FindHorizontalVelocity();
            if (onLadder)
                LadderControl();
        }
        else
        {
            Debug.LogError("Rigidbody2D component is not assigned.");
        }

    }
    private void LadderControl()
    {
        if (moveDirection.y != 0)
        {
            //use translations for ladder movement
            transform.Translate(new Vector3(0, moveDirection.y * speed * Time.deltaTime, 0));
        }
    }
    private void FindHorizontalVelocity()
    {
        if (moveDirection.x ==0 && !jumping)
        {
            rb.linearVelocityX = 0;
        }
        var newHorizontalVelocity = moveDirection.x + rb.linearVelocityX;
        // put bounds on negative and positive value for max speed
        if (newHorizontalVelocity > MaxSpeed)
        {
            newHorizontalVelocity = MaxSpeed;
        }
        else if (newHorizontalVelocity < -MaxSpeed)
        {
            newHorizontalVelocity = -MaxSpeed;
        }
        if (!dashed)
            rb.linearVelocityX = newHorizontalVelocity;
    }
}
