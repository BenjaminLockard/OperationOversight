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

    
    public SpriteRenderer buttonSprite;
    public Color inactiveColor = Color.white;
    public Color activeColor = Color.green;

   
    public AudioSource audioSource;
    public AudioClip openClip;
    public AudioClip closeClip;

    public bool isActive = true;   // 

    private GameObject blockadeInstance;

    void Start()
    {
        if (blockadePrefab != null)
        {
            blockadeInstance = Instantiate(blockadePrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Blockade prefab is not assigned!");
        }

        //Force ON at start 
        isActive = true;
        ApplyBlockadeState();
    }

    public void ToggleBlockade()
    {
        isActive = !isActive;

        // SFX
        if (audioSource != null)
        {
            if (isActive && openClip != null)
                audioSource.PlayOneShot(openClip);
            else if (!isActive && closeClip != null)
                audioSource.PlayOneShot(closeClip);
        }

        ApplyBlockadeState();
    }

    private void ApplyBlockadeState()
    {
        if (buttonSprite != null)
            buttonSprite.color = isActive ? activeColor : inactiveColor;

        if (blockadeInstance == null) return;

        if (isActive)
        {
            blockadeInstance.SetActive(true);

          
            Vector3 midpoint = (pointA.position + pointB.position) / 2f;
            blockadeInstance.transform.position = midpoint;

           
            Vector3 direction = pointB.position - pointA.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            blockadeInstance.transform.rotation = Quaternion.Euler(0, 0, angle);

            //  Force correct scale regardless of the prefab
            float distance = direction.magnitude;
            blockadeInstance.transform.localScale = new Vector3(
                distance,
                1f,   // Fixed height (adjust as needed)
                1f
            );
        }
        else
        {
            blockadeInstance.SetActive(false);
        }
    }

    public void ResetBlockade()
    {
        isActive = true;
        ApplyBlockadeState();
    }
}