using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class CustomCursorManager : MonoBehaviour
{
    //using this script to manage custom cursor behavior
    public Texture2D customCursorTexture;
    public Vector2 hotSpot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(customCursorTexture, hotSpot, cursorMode);


    }

    // Update is called once per frame
    void Update()
    {
        //when mouse hover over UI element, change cursor




    }
}
