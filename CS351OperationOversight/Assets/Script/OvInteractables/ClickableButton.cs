/* Author: Cole Dixon
 * Date: 11/18/2025
 * Assignment: P06
 * Description: Clickable button
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableButton : MonoBehaviour
{
    
    public ButtonActivatedBlockade blockade;  
    public SpriteRenderer buttonSprite;       

    private void OnMouseDown()
    {
        if (blockade != null)
        {
            blockade.ToggleBlockade();
        }
        else
        {
            Debug.LogWarning("No blockade assigned to this button");
        }
    }

    private void OnMouseEnter()
    {
        // Optional: change cursor when hovering
        CustomCursorManager.HoverObject(true);
    }

    private void OnMouseExit()
    {
        // Optional: reset cursor when not hovering
        CustomCursorManager.HoverObject(false);
    }
}
