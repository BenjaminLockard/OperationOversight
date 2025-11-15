/* Author: Cole Dixon
 * Date: 11/12/2025
 * Assignment: P06
 * Description: Mouse cursor controller
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class CustomCursorManager : MonoBehaviour
{
   
    public Texture2D defaultCursorTexture;
    public Texture2D hoverCursorTexture;

    private Vector2 hotSpot = new Vector2(64, 64);
    public CursorMode cursorMode = CursorMode.Auto;

    private bool isOverUI = false;
    private static CustomCursorManager instance;



    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SetDefaultCursor();
    }

    void Update()
    {
        bool pointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

        if (pointerOverUI && !isOverUI)
        {
            SetHoverCursor();
            isOverUI = true;
        }
        else if (!pointerOverUI && isOverUI)
        {
            SetDefaultCursor();
            isOverUI = false;
        }
    }

    public void SetDefaultCursor()
    {
        Cursor.SetCursor(defaultCursorTexture, hotSpot, cursorMode);
    }

    public void SetHoverCursor()
    {
        Cursor.SetCursor(hoverCursorTexture, hotSpot, cursorMode);
    }

    public static void HoverObject(bool isHovering)
    {
        if (instance == null) return;

        if (isHovering)
            instance.SetHoverCursor();
        else
            instance.SetDefaultCursor();
    }
}