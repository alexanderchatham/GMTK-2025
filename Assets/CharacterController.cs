using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    public float MaxSpeed = 1f;
    public float ladderStickiness = 0.5f; // Adjust this value to control how sticky the character is to the ladder
    public Vector3 moveDirection;
    public bool jumping = false;
    public bool dashed = false;
    public bool dashing = false;
    public bool onLadder = false; // Added for ladder functionality
    Coroutine dashCoroutine;
    public Transform PlayerRenderer;
    public Image dashIndicator; // Assuming you have a UI Image for dash indicator
    public List<GameObject> currentLadders = new List<GameObject>();
    int lastMoveIndicator = 0;
    public GameObject spawnPoint;
    Rigidbody2D rb;
    private Interactable currentInteractable;
    public GameObject sword;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on the character.");
        }
    }
    public void Update()
    {

        //transform.Translate(moveDirection * Time.deltaTime);
        //Instead of translate use rb to apply movement
        if (rb != null)
        {
            if (onLadder)
                LadderControl();
            FindHorizontalVelocity();
        }
        else
        {
            Debug.LogError("Rigidbody2D component is not assigned.");
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
            StopDash();
            rb.linearVelocity = Vector2.zero; // Stop the character when entering a ladder
            rb.gravityScale = 0; // Disable gravity while on the ladder
            jumping = false; // Reset jumping state when on a ladder
            dashed = false; // Reset dashed state when on a ladder
            onLadder = true; // Set onLadder to true when entering a ladder
            dashIndicator.gameObject.SetActive(true); // Hide the dash indicator
            print("Entered ladder: " + collision.gameObject.name);
            if(!currentLadders.Contains(collision.gameObject))
            {
                currentLadders.Add(collision.gameObject); // Add the ladder to the list if not already present
            }
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

            if(currentLadders.Contains(collision.gameObject))
            {
                currentLadders.Remove(collision.gameObject); // Remove the ladder from the list when exiting
            }
            if(currentLadders.Count == 0)
            {
                rb.gravityScale = 1; // Re-enable gravity when exiting the ladder
                onLadder = false; // Set onLadder to false when exiting a ladder
            }
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
            StopDash();
            // Reset jumping state when colliding with the ground
            jumping = false;
            dashed = false; // Reset dashed state when touching the ground
            transform.position = spawnPoint.transform.position; // Reset position to spawn point
        }
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumping = false; // Reset jumping state when colliding with the ground
            dashed = false; // Reset dashed state when touching the ground
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
                sword.transform.rotation = Quaternion.Euler(0, 0, -90); // Face right when moving right
                sword.transform.localPosition = new Vector3(1f, 0, 0); // Adjust sword position when moving right
            }
            else if (movement.x < 0)
            {
                lastMoveIndicator = -1; // Left
                sword.transform.rotation = Quaternion.Euler(0, 0, 90); // Face right when moving right
                sword.transform.localPosition = new Vector3(-1f, 0, 0); // Adjust sword position when moving right
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
    private void LandOnGround()
    {
        // Reset jumping state when colliding with the ground
        jumping = false;
        dashed = false; // Reset dashed state when touching the ground
        dashIndicator.gameObject.SetActive(true); // Hide the dash indicator
        FindHorizontalVelocity();
    }
    public void OnJump()
    {
        if(jumping && dashed)
            return; // Prevent jumping if already in the air
        else if (jumping &&!dashed)
        {
            dashCoroutine = StartCoroutine(Dash()); // Start the dash coroutine
        }
        else
        {
            jumping = true;
            rb.AddForceY(jumpForce, ForceMode2D.Impulse);
        }
    }
    bool canAttack = false;
    public void EnableAttack()
    {
        canAttack = true; // Enable attack action
    }
    public void OnAttack()
    {
        print("Fire action triggered");
        if (canAttack)
        {
            StartCoroutine(Attack());
        }
    }
    
    IEnumerator Attack()
    {
        canAttack = false; // Disable attack action after performing it
        sword.SetActive(true); // Activate the sword
        // Implement attack logic here
        yield return new WaitForSeconds(0.5f); // Wait for the attack animation to finish
        sword.SetActive(false); // Deactivate the sword after the attack
        canAttack = true; // Re-enable attack action after the cooldown
        print("Attack performed");

    }

    IEnumerator Dash()
    {
        if (dashed)
            yield break; // Prevent multiple dashes
        dashing = true; // Set dashing state to true
        dashed = true;
        dashIndicator.gameObject.SetActive(false); // Hide the dash indicator
        PlayerRenderer.rotation = Quaternion.Euler(0,0, lastMoveIndicator == 1 ? -15 : 15); // Face the direction of the dash
        rb.gravityScale = 0; // Disable gravity while dashing
        rb.linearVelocityY = 0; // Reset vertical velocity to prevent jumping while dashing
        rb.linearVelocityX = 30f * lastMoveIndicator;
        yield return new WaitForSeconds(0.3f); // Dash duration
        PlayerRenderer.rotation = Quaternion.Euler(0, 0, 0); // Reset rotation after dash
        rb.linearVelocityX = rb.linearVelocityX * 0.5f; // Reduce speed after dash
        rb.gravityScale = 1; // Re-enable gravity after dash
        dashing = false; // Reset dashing state
        dashCoroutine = null; // Clear the dash coroutine reference
    }
    private void StopDash()
    {
        if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine); // Stop any ongoing dash or jump when entering a ladder
            dashing = false; // Reset dashing state when on a ladder
            PlayerRenderer.rotation = Quaternion.Euler(0, 0, 0); // Reset rotation when entering a ladder
            rb.gravityScale = 1; // Re-enable gravity after dash
        }
    }
    private void LadderControl()
    {
        if(moveDirection.x < ladderStickiness && moveDirection.x > -ladderStickiness)
        {
            moveDirection.x = 0; // Stop horizontal movement on the ladder
        }
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
        if (!dashing)
        {
            if(newHorizontalVelocity!= 0)
                PlayerRenderer.rotation = Quaternion.Euler(0, 0, lastMoveIndicator == 1 ? -5 : 5); // Face the direction of the dash
            else
            {
                PlayerRenderer.rotation = Quaternion.Euler(0, 0, 0); // Reset rotation when not moving
            }
            rb.linearVelocityX = newHorizontalVelocity;
        }
    }
}
