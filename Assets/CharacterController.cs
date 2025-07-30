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
    Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on the character.");
        }
    }
    public void OnMove(InputValue value)
    {
        var movement = value.Get<Vector2>();
        if (movement != Vector2.zero)
        {
            // Move the character based on the input
            moveDirection = new Vector3(movement.x *speed, 0, 0);
            if(movement.x > 0)
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
            // Reset jumping state when colliding with the ground
            jumping = false;
            dashed = false; // Reset dashed state when touching the ground
            FindHorizontalVelocity();
        }
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
        }
        else
        {
            Debug.LogError("Rigidbody2D component is not assigned.");
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
