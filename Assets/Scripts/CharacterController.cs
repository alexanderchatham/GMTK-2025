using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
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
    public bool onGround = false; // Added for ground detection
    Coroutine dashCoroutine;
    public Transform PlayerRenderer;
    public Image dashIndicator; // Assuming you have a UI Image for dash indicator
    public List<GameObject> currentLadders = new List<GameObject>();
    int lastMoveIndicator = 0;
    public GameObject spawnPoint;
    Rigidbody2D rb;
    private Interactable currentInteractable;
    public GameObject sword;

    public AudioSource jumpSound;
    public AudioSource dyingSound;
    public AudioSource dashSound;
    public AudioSource walkingSound;
    public AudioSource swingSound;
    public AudioSource landingSound;
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
        if (GameSettings.Paused)
            return;
        if (rb != null)
        {
            if (onLadder)
                LadderControl();
            else if (!dashing)
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
        if (collision.gameObject.CompareTag("Secret"))
        {
            collision.gameObject.SetActive(false); // Deactivate the secret object when exiting
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
    public float dotProductThreshold = 0.5f; // Adjust this threshold as needed
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Lava"))
        {
            dyingSound.Play(); // Play dying sound
            StopDash();
            // Reset jumping state when colliding with the ground
            jumping = false;
            dashed = false; // Reset dashed state when touching the ground
            transform.position = spawnPoint.transform.position; // Reset position to spawn point
        }

        //see if player is above the ground or moving platform with dot product. if not return
        Vector2 contactPoint = collision.GetContact(0).point;
        Vector2 contactNormal = collision.GetContact(0).normal;
        Vector2 playerToContact = contactPoint - (Vector2)transform.position;
        float dotProduct = Vector2.Dot(playerToContact.normalized, contactNormal.normalized);
        if (dotProduct > dotProductThreshold) // Adjust this threshold as needed
        {
            print("Player is not above the ground or moving platform, dot product: " + dotProduct);
            return; // Player is not above the ground or moving platform
        }
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
        if (GameSettings.Paused)
            return;
        var movement = value.Get<Vector2>();
        if (movement != Vector2.zero)
        {
            // Move the character based on the input
            moveDirection = new Vector3(movement.x, movement.y, 0);

            if (dashing)
                return; // Prevent movement if dashing
            
            UpdateVisuals();
        }
        else
        {
            // Stop the character if no input is given
            moveDirection = Vector3.zero;
            UpdateVisuals();
        }
    }

    private void UpdateVisuals()
    {
        if (moveDirection.x > 0)
        {
            lastMoveIndicator = 1; // Right
        }
        else if (moveDirection.x < 0)
        {
            lastMoveIndicator = -1; // Left
        }
        if (lastMoveIndicator == 1)
        {
            sword.transform.rotation = Quaternion.Euler(0, 0, -90); // Face right when moving right
            sword.transform.localPosition = new Vector3(1f, 0, 0); // Adjust sword position when moving right
        }
        else if (lastMoveIndicator == -1)
        {
            sword.transform.rotation = Quaternion.Euler(0, 0, 90); // Face right when moving right
            sword.transform.localPosition = new Vector3(-1f, 0, 0); // Adjust sword position when moving right
        }
        PlayerRenderer.GetComponent<SpriteRenderer>().flipX = lastMoveIndicator < 0; // Flip the character sprite based on movement direction
    }

    private void LandOnGround(bool actuallyLadder = false)
    {
        // Reset jumping state when colliding with the ground
        if (!actuallyLadder)
        {
            GetComponentInChildren<Animator>().SetBool("InAir", false); // Set the moving animation to true when moving

            onGround = true; // Set onGround to true when landing on the ground
            if(moveDirection.x != 0)
                GetComponentInChildren<Animator>().SetBool("Moving",true); // Set the moving animation to true when landing on the ground
            else
                GetComponentInChildren<Animator>().SetBool("Moving", false); // Set the moving animation to false when not moving
            if (jumping)
                landingSound.Play(); // Play landing sound
            rb.gravityScale = 3; // Ensure gravity is enabled when landing on the ground
            //FindHorizontalVelocity();
        }
        else
        {
            rb.gravityScale = 0; // Disable gravity when landing on a ladder
            rb.linearVelocityY = 0; // Reset vertical velocity to prevent jumping while on a ladder
        }
        jumping = false;
        dashed = false; // Reset dashed state when touching the ground
        dashIndicator.gameObject.SetActive(true); // Hide the dash indicator
    }
    public void OnJump()
    {
        if(jumping && dashed)
            return; // Prevent jumping if already in the air
        else if (jumping &&!dashed&&!onLadder)
        {
            dashCoroutine = StartCoroutine(Dash()); // Start the dash coroutine
        }
        else
        {
            GetComponentInChildren<Animator>().SetBool("InAir", true); // Set the moving animation to true when moving

            jumpSound.Play(); // Play jump sound
            jumping = true;
            rb.linearVelocityY = 0; // Reset vertical velocity to prevent double jumping
            rb.AddForceY(jumpForce, ForceMode2D.Impulse);
            rb.gravityScale = 1; // Ensure gravity is enabled when jumping
        }
    }


    public void OnPause()
    {
        GameSettings.instance.TogglePause();
        rb.linearVelocity = Vector2.zero;
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
    public ScrollRect leaderboardScrollView; // Assuming you have a ScrollView for scrolling actions
    public void OnScroll(InputValue value)
    {
        //scroll the leaderboard based on the input
        if (GameSettings.Paused)
            return;
        var scrollDelta = value.Get<Vector2>();
        if (leaderboardScrollView != null)
        {
            // Adjust the scroll position based on the input
            leaderboardScrollView.verticalNormalizedPosition += scrollDelta.y * 0.1f; // Adjust the multiplier as needed
            leaderboardScrollView.horizontalNormalizedPosition += scrollDelta.x * 0.1f; // Adjust the multiplier as needed
        }
        else
        {
            Debug.LogWarning("Leaderboard ScrollView is not assigned.");
        }
    }
    IEnumerator Attack()
    {
        canAttack = false; // Disable attack action after performing it
        sword.SetActive(true); // Activate the sword
        swingSound.Play(); // Play attack sound
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
        dashSound.Play(); // Play dash sound
        dashing = true; // Set dashing state to true
        dashed = true;
        dashIndicator.gameObject.SetActive(false); // Hide the dash indicator
        PlayerRenderer.rotation = Quaternion.Euler(0,0, lastMoveIndicator == 1 ? -15 : 15); // Face the direction of the dash
        rb.gravityScale = 0; // Disable gravity while dashing
        rb.linearVelocityY = 0; // Reset vertical velocity to prevent jumping while dashing
        rb.linearVelocityX = 20f * lastMoveIndicator;
        float timer = 0f; // Timer for dash duration
        while (timer < 0.5f) // Dash duration
        {
            timer += Time.deltaTime;
            rb.linearVelocityY = 0; // Maintain horizontal velocity during dash
            rb.linearVelocityX = 20f * lastMoveIndicator; // Maintain horizontal velocity during dash
            // If the player is contacting a collider at a 45 degree angle translate the player up a little 
            var contactPoint = Physics2D.OverlapCircle(transform.position, .5f, LayerMask.GetMask("Ground"));
            if (contactPoint != null)
            {
                Vector2 contactNormal = contactPoint.ClosestPoint(transform.position) - (Vector2)transform.position;
                float dotProduct = Vector2.Dot(contactNormal.normalized, Vector2.up);
                if (dotProduct < 0.5f) // Adjust this threshold as needed
                {
                    // If the player is not above the ground, translate the player up a little
                    transform.Translate(new Vector3(0, 0.3f, 0)); // Adjust this value to control the upward translation during dash
                }
            }
            yield return null; // Wait for the next frame
        }
        PlayerRenderer.rotation = Quaternion.Euler(0, 0, 0); // Reset rotation after dash
        rb.linearVelocityX = rb.linearVelocityX * 0.5f; // Reduce speed after dash
        rb.gravityScale = 1; // Re-enable gravity after dash
        dashing = false; // Reset dashing state
        dashCoroutine = null; // Clear the dash coroutine reference
        UpdateVisuals();
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
        if (rb.linearVelocityY < 0)
        {
            rb.linearVelocityY = 0; // Stop downward movement on the ladder
            LandOnGround(true);
        }
        if (moveDirection.x < ladderStickiness && moveDirection.x > -ladderStickiness)
        {
            moveDirection.x = 0; // Stop horizontal movement on the ladder
        }
        //use translations for ladder movement
        transform.Translate(new Vector3(moveDirection.x * speed * Time.deltaTime, moveDirection.y * speed * Time.deltaTime, 0));
        
    }
    private void FindHorizontalVelocity()
    {
        if (moveDirection.x ==0 && !jumping)
        {
            rb.linearVelocityX = 0;
        }
        var newHorizontalVelocity = moveDirection.x *speed + rb.linearVelocityX;
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
            {
                PlayerRenderer.rotation = Quaternion.Euler(0, 0, lastMoveIndicator == 1 ? -5 : 5); // Face the direction of the dash
                if(onGround)
                    GetComponentInChildren<Animator>().SetBool("Moving", true); // Set the moving animation to true when moving

            }
            else
            {
                PlayerRenderer.rotation = Quaternion.Euler(0, 0, 0); // Reset rotation when not moving
                GetComponentInChildren<Animator>().SetBool("Moving", false); // Set the moving animation to true when moving

            }
            rb.linearVelocityX = newHorizontalVelocity;
            var contactPoint = Physics2D.OverlapCircle(transform.position, .5f, LayerMask.GetMask("Ground"));
            if (contactPoint != null)
            {
                Vector2 contactNormal = contactPoint.ClosestPoint(transform.position) - (Vector2)transform.position;
                float dotProduct = Vector2.Dot(contactNormal.normalized, Vector2.up);
                if (dotProduct < 0.5f) // Adjust this threshold as needed
                {
                    // If the player is not above the ground, translate the player up a little
                    transform.Translate(new Vector3(0, 0.3f, 0)); // Adjust this value to control the upward translation during dash
                }
            }
        }
    }
}
