/* Author: Cole Dixon
 * Date: 11/12/2025
 * Assignment: P06
 * Description: Mouse cursor controller
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private void OnMouseEnter()
    {
        CustomCursorManager.HoverObject(true);
    }

    private void OnMouseExit()
    {
        CustomCursorManager.HoverObject(false);
    }
}

