/* Author: Cole Dixon
 * Date: 11/18/2025
 * Assignment: P06
 * Description: Handles the action of forming a blockade between points a and b
 */

using System.Collections;
using UnityEngine;

public class ButtonActivatedBlockade : MonoBehaviour
{
    
    public GameObject blockadePrefab;
    public Transform pointA;
    public Transform pointB;
    public float animationDuration = 0.5f;

   
    public SpriteRenderer buttonSprite; 
    public Color inactiveColor = Color.white;
    public Color activeColor = Color.green;

    
    public bool isActive = false; 

    private GameObject blockadeInstance;
    private bool isAnimating = false;

    void Start()
    {
        
        if (blockadePrefab != null)
        {
            blockadeInstance = Instantiate(blockadePrefab, Vector3.zero, Quaternion.identity);
            blockadeInstance.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Blockade prefab is not assigned!");
        }

        // Set button initial color
        if (buttonSprite != null)
            buttonSprite.color = inactiveColor;
    }


public void ResetBlockade()
{
    if (isActive)
    {
        // If it's active, turn it off without toggling button color twice
        StartCoroutine(AnimateBlockade(false));
        isActive = false;

        if (buttonSprite != null)
            buttonSprite.color = inactiveColor;
    }
}




    // Toggle blockade on/off
    public void ToggleBlockade()
    {
        if (isAnimating) return; // prevent interrupting animation

        if (isActive)
            StartCoroutine(AnimateBlockade(false));
        else
            StartCoroutine(AnimateBlockade(true));

        // Flip state
        isActive = !isActive;

        // Update button color
        if (buttonSprite != null)
            buttonSprite.color = isActive ? activeColor : inactiveColor;
    }

    // Returns whether the blockade is currently active
    public bool IsBlockadeActive()
    {
        return isActive;
    }

    // Coroutine to animate blockade
    private IEnumerator AnimateBlockade(bool turnOn)
    {
        isAnimating = true;

        // Set blockade position and rotation
        Vector3 midpoint = (pointA.position + pointB.position) / 2f;
        blockadeInstance.transform.position = midpoint;

        Vector3 direction = pointB.position - pointA.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        blockadeInstance.transform.rotation = Quaternion.Euler(0, 0, angle);

        float distance = direction.magnitude;

        // Activate if turning on
        if (turnOn)
            blockadeInstance.SetActive(true);

        Vector3 startScale = blockadeInstance.transform.localScale;
        Vector3 targetScale = new Vector3(distance, startScale.y, startScale.z);

        Vector3 initialScale = startScale;
        Vector3 finalScale = turnOn ? targetScale : new Vector3(0, startScale.y, startScale.z);

        float timer = 0f;
        while (timer < animationDuration)
        {
            timer += Time.deltaTime;
            float t = timer / animationDuration;
            blockadeInstance.transform.localScale = Vector3.Lerp(initialScale, finalScale, t);
            yield return null;
        }

        blockadeInstance.transform.localScale = finalScale;

        // Deactivate after turning off
        if (!turnOn)
            blockadeInstance.SetActive(false);

        isAnimating = false;
    }
}