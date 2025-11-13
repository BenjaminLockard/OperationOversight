/* Author: Cole Dixon
 * Date: 11/12/2025
 * Assignment: P06
 * Description: Rail car 
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RailCar : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;

    [Header("Player Attachment")]
    public Transform player;
    public float attachDistance = 1.5f;
    private bool isPlayerAttached = false;

    [Header("Visuals")]
    public Color defaultColor = Color.white;
    public Color selectedColor = Color.yellow;

    private bool isSelected = false;
    private bool isMoving = false;
    private bool movingToB = true;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.color = defaultColor;
    }

    void Update()
    {
        if (isMoving)
        {
            MoveBetweenPoints();
            MoveAttachedPlayer();
        }
    }

    private void MoveBetweenPoints()
    {
        Transform target = movingToB ? pointB : pointA;

        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        // Stop when we reach the target
        if (Vector2.Distance(transform.position, target.position) < 0.05f)
        {
            isMoving = false;

            // If player is attached and we've arrived, detach them
            if (isPlayerAttached)
                DetachPlayer();

            Debug.Log($"Rail car arrived at {(movingToB ? "Point B" : "Point A")}");
        }
    }

    private void MoveAttachedPlayer()
    {
        if (isPlayerAttached && player != null)
        {
            player.position = new Vector3(
                transform.position.x,
                player.position.y,
                player.position.z
            );
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
        // Toggle direction when clicked
        if (!isMoving)
        {
            movingToB = !movingToB; // flip direction
            isMoving = true;

            // Change visual and attach player if near
            if (spriteRenderer != null)
                spriteRenderer.color = selectedColor;

            TryAttachPlayer();
        }
        else
        {
            // Optional: ignore clicks while moving, or stop mid-way
            // For now, we’ll ignore mid-move clicks
            Debug.Log("Rail car is moving, wait until it stops to click again.");
        }
    }

    private void TryAttachPlayer()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attachDistance)
        {
            isPlayerAttached = true;
            Debug.Log("Player attached to rail car!");
        }
    }

    private void DetachPlayer()
    {
        if (isPlayerAttached)
        {
            isPlayerAttached = false;
            if (spriteRenderer != null)
                spriteRenderer.color = defaultColor;

            Debug.Log("Player detached from rail car!");
        }
    }
}