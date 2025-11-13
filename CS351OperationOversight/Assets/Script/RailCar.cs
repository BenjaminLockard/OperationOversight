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
    
    public Transform pointA;
    public Transform pointB;

    public float moveSpeed = 2f;
    private bool movingToB = true;
    private bool isMoving = false;


    public Transform player;
    public float attachDistance = 1.5f;
    private bool isPlayerAttached = false;


    public Color defaultColor = Color.white;
    public Color selectedColor = Color.yellow;

    private bool isSelected = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.color = defaultColor;
    }

    void Update()
    {
        // Move the rail car if active
        if (isMoving)
        {
            MoveBetweenPoints();

            // Move player along if attached
            if (isPlayerAttached && player != null)
            {
                player.position = new Vector3(
                    transform.position.x,
                    player.position.y,
                    player.position.z
                );
            }
        }
    }

    private void MoveBetweenPoints()
    {
        Transform target = movingToB ? pointB : pointA;
        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.05f)
        {
            movingToB = !movingToB;
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
        isSelected = !isSelected;

        if (spriteRenderer != null)
            spriteRenderer.color = isSelected ? selectedColor : defaultColor;

        if (isSelected)
        {
            // Start moving when selected
            isMoving = true;

            // Attach player if close enough
            TryAttachPlayer();
        }
        else
        {
            // Stop movement and detach player
            isMoving = false;
            DetachPlayer();
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
            Debug.Log("Player detached from rail car!");
        }
    }
}