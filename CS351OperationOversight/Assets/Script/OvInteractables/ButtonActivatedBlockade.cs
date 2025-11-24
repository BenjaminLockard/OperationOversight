/* Author: Cole Dixon
 * Date: 11/18/2025
 * Assignment: P06
 * Description: Handles the action of forming a blockade between points a and b
 * Modified to keep fixed vertical length and play SFX
 */

using System.Collections;
using UnityEngine;

public class ButtonActivatedBlockade : MonoBehaviour
{
    
    public GameObject blockadePrefab;
    public Transform pointA;
    public Transform pointB;
    public float animationDuration = 0.5f;

    [Header("Button Settings")]
    public SpriteRenderer buttonSprite; 
    public Color inactiveColor = Color.white;
    public Color activeColor = Color.green;

    [Header("Audio Settings")]
    public AudioSource audioSource; // Assign AudioSource component here
    public AudioClip openClip;      // Sound when opening
    public AudioClip closeClip;     // Sound when closing

    public bool isActive = true; 

    private GameObject blockadeInstance;
    private bool isAnimating = false;

    void Start()
    {
        isActive = true;

        if (blockadePrefab != null)
        {
            blockadeInstance = Instantiate(blockadePrefab, Vector3.zero, Quaternion.identity);
            blockadeInstance.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Blockade prefab is not assigned!");
        }

        // Set button initial color
        if (buttonSprite != null)
            buttonSprite.color = inactiveColor;

        StartCoroutine(AnimateBlockade(true));
    }
    

    public void ResetBlockade()
    {
        StopAllCoroutines();
        isActive = true;

        if (buttonSprite != null)
            buttonSprite.color = activeColor;

        if (blockadeInstance != null)
            blockadeInstance.SetActive(true);

        // Reset position and scale
        Vector3 midpoint = (pointA.position + pointB.position) / 2f;
        blockadeInstance.transform.position = midpoint;

        Vector3 direction = pointB.position - pointA.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        blockadeInstance.transform.rotation = Quaternion.Euler(0, 0, angle);

        float distance = direction.magnitude;
        blockadeInstance.transform.localScale = new Vector3(
            distance,
            blockadeInstance.transform.localScale.y,
            blockadeInstance.transform.localScale.z
        );
    }

    public void ToggleBlockade()
    {
        if (isAnimating) return;

        // Play SFX
        if (audioSource != null)
        {
            if (isActive && closeClip != null)
                audioSource.PlayOneShot(closeClip);
            else if (!isActive && openClip != null)
                audioSource.PlayOneShot(openClip);
        }

        if (isActive)
            StartCoroutine(AnimateBlockade(false));
        else
            StartCoroutine(AnimateBlockade(true));

        isActive = !isActive;

        if (buttonSprite != null)
            buttonSprite.color = isActive ? activeColor : inactiveColor;
    }

    public bool IsBlockadeActive()
    {
        return isActive;
    }

    private IEnumerator AnimateBlockade(bool turnOn)
    {
        isAnimating = true;

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