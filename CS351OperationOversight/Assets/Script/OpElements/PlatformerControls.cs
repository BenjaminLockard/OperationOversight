/* Author: Benjamin Lockard
 * Date: 11/7/2025
 * Assignment: P06
 * Description: Controls platformer player
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerControls : MonoBehaviour
{
    // member vars & references ----------------------------------------------------------------------------------------

    public float moveSpeed, jumpForce, wallJumpHoriForce, wallJumpVertForce, wjHoriPause, springPause, deathPause, acceleration, deceleration;

    public LayerMask groundLayer;

    public Transform groundCheck, wallJumpCheck;
    private float directionalNegation;

    public float groundCheckRadius, wallCheckRadius;

    private Rigidbody2D rb;

    public float knockbackForce;

    private float horizontalInput;
    public float vertTransModifier;
    private bool verticalInput, verticalInputReleased, isGrounded, isOnWall, jumpRequest, wallJumpRequest, inputBlocked = false, negationChangeBlocked, landSoundBlocked, runStartBlocked, airJumpBlocked;

    private float jumpBufferCounter, coyoteTimeCounter;
    public float jumpBufferTime = 0.10f, coyoteTimeDuration = 0.10f;

    private Vector3 currentRespawnPosition = new Vector3(0, 0);

    private AudioSource playerAudio;
    public AudioClip jumpSound, airJumpSound, landSound, dieSound, runSound;

    private Animator animator;

    // general functions ----------------------------------------------------------------------------------------

    IEnumerator blockInput(float delay)
    {
        inputBlocked = true;
        yield return new WaitForSeconds(delay);
        inputBlocked = false;
    }

    public void setCheckpoint(Vector3 checkpointPosition)
    {
        currentRespawnPosition = checkpointPosition;
    }


    public void Die(Vector3 hazardPosition)
    {
        inputBlocked = true;

        playerAudio.PlayOneShot(dieSound, 0.25f);
        Vector2 direction = transform.position - hazardPosition;
        direction.Normalize();

        direction.y = direction.y * 0.5f + 0.5f;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);

      
 //Cole added following lines 11/18 to reset all activated hazards when player dies
	//code to reset all blockades when you die
        ButtonActivatedBlockade[] allBlockades = FindObjectsOfType<ButtonActivatedBlockade>();

        foreach (ButtonActivatedBlockade blockade in allBlockades)
        {
            blockade.ResetBlockade();
        }

	{
// code to reset all railcars location to starting point when you die
    RailCar[] allRailCars = FindObjectsOfType<RailCar>();
    foreach (RailCar rc in allRailCars)
    {
        rc.ResetRailcars();
    }
}


        StartCoroutine(respawn());

    }

    IEnumerator respawn()
    {
        //start death animation here

        //incorporate death effect into animation or make new effect
        //adjust pause based on animation length in inspector
        yield return new WaitForSeconds(deathPause);

        //ensure sprite returns to idle
        //Consider adding screen transition
        transform.position = currentRespawnPosition;

        rb.velocity = Vector2.zero;

        inputBlocked = false;
    }

    IEnumerator audiblyWalk()
    {
        runStartBlocked = true;
        while (isGrounded && horizontalInput != 0)
        {
            playerAudio.PlayOneShot(landSound, 0.05f);
            yield return new WaitForSeconds(0.25f);
        }
        runStartBlocked = false;
    }

    public void launch(Vector2 direction, float magnitude, bool isHorizontal)
    {
        if (isHorizontal)
            StartCoroutine(blockInput(springPause));
        rb.AddForce(direction * magnitude, ForceMode2D.Impulse);
    }

    // runtime functions ----------------------------------------------------------------------------------------
    void Start()
    {
        playerAudio = GetComponent<AudioSource>();
        groundCheckRadius = 0.1f;
        wallCheckRadius = 0.65f;
        rb = GetComponent<Rigidbody2D>();

        if (groundCheck == null || wallJumpCheck == null)
        {
            Debug.LogError("a check is unassigned to player controller.");
        }

        //playerAudio = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        verticalInput = Input.GetButtonDown("Jump");
        verticalInputReleased = Input.GetButtonUp("Jump");

        if (verticalInput && !inputBlocked)
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;


        if (isGrounded)
            coyoteTimeCounter = coyoteTimeDuration;
        else
            coyoteTimeCounter -= Time.deltaTime;

        if ((isGrounded || coyoteTimeCounter > 0.0f) && jumpBufferCounter > 0.0f && !inputBlocked)
        {
            jumpRequest = true;
        }
        else if (isOnWall && jumpBufferCounter > 0.0f && !inputBlocked)
        {
            wallJumpRequest = true;
        }

        if (verticalInputReleased && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / vertTransModifier);
        }

        //    //playerAudio.PlayOneShot(jumpSound, 1.0f)
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isOnWall = Physics2D.OverlapCircle(wallJumpCheck.position, wallCheckRadius, groundLayer);
        if (isGrounded)
        {
            airJumpBlocked = false;
            if (!landSoundBlocked)
            {
                playerAudio.PlayOneShot(landSound, 0.25f);
                landSoundBlocked = true;
            }
        }
        else
        {
             landSoundBlocked = false;
        }


        if (isOnWall)
        {
            if (!negationChangeBlocked) {
                directionalNegation = wallJumpCheck.position.x > transform.position.x ? -1f : 1f;

                negationChangeBlocked = true;
            }
        } else
        {
            negationChangeBlocked = false;
        }


        if (!inputBlocked)
        {//uses acceleration & deceleration
            float targetSpeed = horizontalInput * moveSpeed;
            float speedDifference = targetSpeed - rb.velocity.x;
            float currentAcceleration = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;

            if (Mathf.Abs(horizontalInput) > 0.01f && Mathf.Sign(horizontalInput) != Mathf.Sign(rb.velocity.x))
            {
                currentAcceleration = acceleration; // Prioritize acceleration when changing direction
            }

            // add force rather than set, allows concurrent multiple sources of velocity
            rb.AddForce(Vector2.right * (speedDifference * currentAcceleration), ForceMode2D.Force);

            // clamp horizontal velocity, ensure it remains within reasonable bounds
            //rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -moveSpeed, moveSpeed), rb.velocity.y);
        }

        if (jumpRequest)
        {
            playerAudio.PlayOneShot(jumpSound, 0.45f);

            jumpRequest = false;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpBufferCounter = 0.0f;
            coyoteTimeCounter = 0.0f;
        }
        else if (wallJumpRequest)
        {
            playerAudio.PlayOneShot(jumpSound, 0.35f);

            wallJumpRequest = false;
            StartCoroutine(blockInput(wjHoriPause));

            //directionalNegation = wallJumpCheck.position.x > transform.position.x ? -1f : 1f;
            rb.AddForce(new Vector2(wallJumpHoriForce * directionalNegation, wallJumpVertForce), ForceMode2D.Impulse);
            jumpBufferCounter = 0.0f;
        }

        animator.SetFloat("XVelocityAbs", Mathf.Abs(rb.velocity.x));

        animator.SetFloat("YVelocity", rb.velocity.y + 0.5f);

        animator.SetBool("OnGround", isGrounded);

        animator.SetBool("OnWall", isOnWall);

        if (horizontalInput != 0 && !runStartBlocked)
        {
            StartCoroutine(audiblyWalk());
        }
        if (horizontalInput > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0); //right
        }
        else if (horizontalInput < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0); //left
        }
    }
    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(0.4f);
        animator.SetBool("Activate", false);
    }

    // interaction functionality ----------------------------------------------------------------------------------------
    void OnMouseDown()
    {
        if (!airJumpBlocked && !isOnWall)
        {
            airJumpBlocked = true;
            playerAudio.PlayOneShot(airJumpSound, 0.45f);
            rb.velocity = new Vector2(rb.velocity.x, 0.75f * jumpForce);
            animator.SetBool("Activate", true);
            StartCoroutine(Cooldown());

        }
    }
}
