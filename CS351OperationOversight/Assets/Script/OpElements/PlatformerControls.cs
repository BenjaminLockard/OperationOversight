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

    public float moveSpeed, jumpForce, wallJumpHoriForce, wallJumpVertForce, wjHoriPause, springPause, deathPause;


    public LayerMask groundLayer;

    public Transform groundCheck, wallJumpCheck;
    private float storedNegation, directionalNegation = -1.0f;

    public float groundCheckRadius, wallCheckRadius;

    private Rigidbody2D rb;

    public float knockbackForce;

    private float horizontalInput;
    public float vertTransModifier;
    private bool verticalInput, verticalInputReleased;

    private bool isGrounded, isOnWall, jumpRequest, wallJumpRequest, inputBlocked = false;

    private float jumpBufferCounter, coyoteTimeCounter;
    public float jumpBufferTime = 0.10f, coyoteTimeDuration = 0.10f;

    private Vector3 currentRespawnPosition = Vector3.zero;

    //private AudioSource playerAudio;

    // public AudioClip jumpSound;

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

        Vector2 direction = transform.position - hazardPosition;
        direction.Normalize();

        direction.y = direction.y * 0.5f + 0.5f;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);


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

    public void launch(Vector2 direction, float magnitude, bool isHorisontal)
    {
        if (isHorisontal)
            StartCoroutine(blockInput(springPause));
        rb.AddForce(direction * magnitude, ForceMode2D.Impulse);
    }

    // runtime functions ----------------------------------------------------------------------------------------
    void Start()
    {
        groundCheckRadius = 0.05f;
        wallCheckRadius = 0.25f;
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
            storedNegation = directionalNegation;
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

        if (!inputBlocked)
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

        if (jumpRequest)
        {
            jumpRequest = false;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpBufferCounter = 0.0f;
            coyoteTimeCounter = 0.0f;
        }
        else if (wallJumpRequest)
        {
            wallJumpRequest = false;
            StartCoroutine(blockInput(wjHoriPause));
            rb.velocity = new Vector2(wallJumpHoriForce * storedNegation, wallJumpVertForce);
            jumpBufferCounter = 0.0f;
        }

        animator.SetFloat("XVelocityAbs", Mathf.Abs(rb.velocity.x));

        animator.SetFloat("YVelocity", rb.velocity.y + 0.5f);

        animator.SetBool("OnGround", isGrounded);    
        
        animator.SetBool("OnWall", isOnWall);

        if (horizontalInput > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0); //right
            directionalNegation = -1.0f;
        }
        else if (horizontalInput < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0); //left
            directionalNegation = 1.0f;
        }
    }

    // interaction functionality ----------------------------------------------------------------------------------------

}
