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
    public float moveSpeed, jumpForce, wallJumpHoriForce, wallJumpVertForce, wjHoriPause;

    public LayerMask groundLayer;

    public Transform groundCheck, wallJumpCheck;
    private float directionalNegation = -1.0f;

    public float groundCheckRadius, wallCheckRadius;

    //References rigid body
    private Rigidbody2D rb;

    // horisontal var
    private float horizontalInput, vertTransModifier = 2.5f;
    private bool verticalInput, verticalInputReleased;

    private bool isGrounded, isOnWall, wallJumped = false;

    //private AudioSource playerAudio;

    // public AudioClip jumpSound;

    //private Animator animator;


    IEnumerator justWallJumped()
    {
        wallJumped = true;
        yield return new WaitForSeconds(wjHoriPause);
        wallJumped = false;
    }



    // Start is called before the first frame update
    void Start()
    {
        groundCheckRadius = 0.15f;
        wallCheckRadius = 0.5f;
        rb = GetComponent<Rigidbody2D>();

        if (groundCheck == null || wallJumpCheck == null)
        {
            Debug.LogError("a check is unassigned to player controller.");
        }

        //playerAudio = GetComponent<AudioSource>();
        //animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        verticalInput = Input.GetButtonDown("Jump");
        verticalInputReleased = Input.GetButtonUp("Jump");

        if (verticalInput)
        {
            if (isGrounded)
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            else if (isOnWall)
            {
                StartCoroutine(justWallJumped());
                rb.velocity = new Vector2(wallJumpHoriForce * directionalNegation, wallJumpVertForce);
            }
        }

        if (verticalInputReleased && rb.velocity.y > 0) { 
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / vertTransModifier);
        }
        //    //playerAudio.PlayOneShot(jumpSound, 1.0f)
    }

    void FixedUpdate()
    {
        if (!wallJumped)
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

        //animator.SetFloat("xVelocityAbs", Mathf.Abs(rb.velocity.x));

        //animator.SetFloat("yVelocity", rb.velocity.y);

        //animator.SetBool("onGround", isGrounded);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isOnWall = Physics2D.OverlapCircle(wallJumpCheck.position, wallCheckRadius, groundLayer);

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

}
