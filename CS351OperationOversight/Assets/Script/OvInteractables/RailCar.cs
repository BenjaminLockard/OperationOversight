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
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed;

    [Header("Player Settings")]
    public Transform player;         // Assign your player here
    public float attachDistance = 2f;

    [Header("Visual Settings")]
    public Color defaultColor = Color.white;
    public Color ridingColor = Color.yellow;

    private Vector3 targetPos;
    private bool movingToB = true;
    private bool isMoving = false;
    private bool playerRiding = false;

    private Rigidbody2D playerRb;
    private SpriteRenderer spriteRenderer;

    private Vector3 previousPosition;

    void Start()
    {
        if (pointA != null)
            transform.position = pointA.position;

        targetPos = pointB.position;
        previousPosition = transform.position;

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
        Vector3 newPos = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        Vector3 delta = newPos - transform.position;

        transform.position = newPos;

        // Move player with railcar if riding
        if (playerRiding && player != null)
        {
            player.position += delta;
        }

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            isMoving = false;
        }

        previousPosition = transform.position;
    }

    void OnMouseDown()
    {
        // Toggle railcar movement
        movingToB = !movingToB;
        targetPos = movingToB ? pointB.position : pointA.position;
        isMoving = true;

        // Attach player if in range
        if (player != null)
        {
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance <= attachDistance)
            {
                AttachPlayer();
            }
        }
    }

    void AttachPlayer()
    {
        if (playerRiding) return;

        playerRiding = true;

        // Optional: freeze vertical velocity to prevent falling
        if (playerRb != null)
        {
            playerRb.velocity = Vector2.zero;
        }

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

        // Draw a line between points
        if (pointA != null && pointB != null)
            Gizmos.DrawLine(pointA.position, pointB.position);
    }
}