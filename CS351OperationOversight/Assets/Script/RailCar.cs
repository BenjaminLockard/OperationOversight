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
    [Header("Movement Settings")]
    public Transform pointA;          // Starting point
    public Transform pointB;          // Destination
    public float moveSpeed = 2f;      // Speed of movement

    
    public Transform player;          // Player reference
    public float attachDistance = 1.5f; // Max distance to attach

    
    public Color defaultColor = Color.white;
    public Color ridingColor = Color.yellow;

    private bool isMoving = false;
    private bool movingToB = true;
    private bool isPlayerAttached = false;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D playerRb;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.color = defaultColor;

        if (player != null)
            playerRb = player.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isMoving)
        {
            MoveBetweenPoints();
        }
    }

    private void MoveBetweenPoints()
    {
        Transform target = movingToB ? pointB : pointA;
        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.05f)
        {
            movingToB = !movingToB; // Reverse direction at endpoints
        }
    }

    private void OnMouseEnter()
    {
        CustomCursorManager.HoverObject(true);
    }

    private void OnMouseExit()
    {
        CustomCursorManager.HoverObject(false);
    }

    private void OnMouseDown()
    {
        if (player == null) return;

        if (isPlayerAttached)
        {
            DetachPlayer(); // Hop off
        }
        else
        {
            TryAttachPlayer(); // Hop on if nearby
        }
    }

    private void TryAttachPlayer()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attachDistance)
        {
            isPlayerAttached = true;
            isMoving = true; // Start moving when player hops on
            player.SetParent(transform);

            // Freeze physics to prevent player falling
            if (playerRb != null)
            {
                playerRb.isKinematic = true;
                playerRb.velocity = Vector2.zero;
            }

            // Adjust local position
            Vector3 localPos = player.localPosition;
            localPos.y = 0.5f; // tweak as needed
            player.localPosition = localPos;

            if (spriteRenderer != null)
                spriteRenderer.color = ridingColor;

            Debug.Log("Player attached. Rail car started moving!");
        }
    }

    private void DetachPlayer()
    {
        isPlayerAttached = false;
        isMoving = false; // Stop moving immediately
        player.SetParent(null);

        // Re-enable physics
        if (playerRb != null)
            playerRb.isKinematic = false;

        if (spriteRenderer != null)
            spriteRenderer.color = defaultColor;

        Debug.Log("Player detached. Rail car stopped.");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (pointA != null) Gizmos.DrawSphere(pointA.position, 0.2f);
        if (pointB != null) Gizmos.DrawSphere(pointB.position, 0.2f);
    }
}