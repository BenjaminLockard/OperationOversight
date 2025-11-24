/* Author: Cole Dixon
 * Date: 11/12/2025
 * Assignment: P06
 * Description: Rail car 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailCar : MonoBehaviour
{
    
    private Animator railAnimator;

    
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed;

    
    public Transform player;         // Assign your player here
    public float attachDistance = 2f;


    public Color defaultColor = Color.white;
    public Color ridingColor = Color.yellow;

    // Movement variables
    private Vector3 targetPos;
    private bool isMoving = false;
    private bool playerRiding = false;
    private bool movingToB = true; // Track direction

    private Rigidbody2D playerRb;
    private SpriteRenderer spriteRenderer;
    private Vector3 previousPosition;

   
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        railAnimator = GetComponent<Animator>();

        if (pointA != null)
            transform.position = pointA.position + new Vector3(0f, -0.5f, 0f);

        targetPos = pointB.position + new Vector3(0f, -0.5f, 0f);
        previousPosition = transform.position + new Vector3(0f, -0.5f, 0f);

        // Save initial state for ResetRailcars
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        if (player != null)
            playerRb = player.GetComponent<Rigidbody2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.color = defaultColor;
    }

    void Update()
    {
        if (isMoving)
            MoveRailCar();

        // Allow player to jump off
        if (playerRiding && playerRb != null && Mathf.Abs(playerRb.velocity.y) > 0.1f)
        {
            DetachPlayer();
        }
    }

    void MoveRailCar()
    {
        Vector3 newPos = Vector3.MoveTowards(transform.position, targetPos + new Vector3(0f, -0.5f, 0f), moveSpeed * Time.deltaTime);
        Vector3 delta = newPos - transform.position;

        transform.position = newPos;

        // Move player with railcar if riding
        if (playerRiding && player != null)
            player.position += delta;

        // Check if target reached
        if (Vector3.Distance(transform.position, targetPos + new Vector3(0f, -0.5f, 0f)) < 0.01f)
        {
            isMoving = false;

            // Flip direction for next click
            movingToB = !movingToB;

            if (railAnimator != null)
                railAnimator.SetBool("IsMoving", false);
        }

        previousPosition = transform.position;
    }

    void OnMouseDown()
    {
        // Only respond if not already moving
        if (isMoving) return;

        if (railAnimator != null)
            railAnimator.SetBool("IsMoving", true);

        // Set target based on direction
        targetPos = movingToB ? pointB.position : pointA.position;
        isMoving = true;

        // Attach player if in range
        if (player != null)
        {
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance <= attachDistance)
                AttachPlayer();
        }
    }

    void AttachPlayer()
    {
        if (playerRiding) return;

        playerRiding = true;

        if (playerRb != null)
            playerRb.velocity = Vector2.zero;

        if (spriteRenderer != null)
            spriteRenderer.color = ridingColor;
    }

    void DetachPlayer()
    {
        if (!playerRiding) return;

        playerRiding = false;

        if (spriteRenderer != null)
            spriteRenderer.color = defaultColor;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (pointA != null) Gizmos.DrawSphere(pointA.position, 0.2f);
        if (pointB != null) Gizmos.DrawSphere(pointB.position, 0.2f);

        if (pointA != null && pointB != null)
            Gizmos.DrawLine(pointA.position, pointB.position);
    }

    public void ResetRailcars()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        isMoving = false;
        movingToB = true;
        targetPos = pointB.position + new Vector3(0f, -0.5f, 0f);

        if (railAnimator != null)
            railAnimator.SetBool("IsMoving", false);

        if (playerRiding)
            DetachPlayer();
    }
}