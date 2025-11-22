/*
    Author: Lenny Ozaeta
    Assignment: Team Project (Operation Oversight)
    Description: Manages the audio for UI elements in the Title & Stage Select Screens
    Initially Created: Thursday, 11/13/25
        Modified: Friday, 11/21/25
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Added

public class UIAudioManager : MonoBehaviour
{
    // Main variables for this class
    public AudioSource uiAudioSource;
    public AudioClip buttonClickSound;

    // Start is called before the first frame update
    void Start()
    {
        uiAudioSource = GetComponent<AudioSource>();

        // Automatically find all buttons and add a listener
        foreach (Button button in FindObjectsOfType<Button>())
        {
            button.onClick.AddListener(() => PlayClick());
        }
    }

    // Plays specified audio clip when any button clicked
    void PlayClick()
    {
        uiAudioSource.PlayOneShot(buttonClickSound);
    }
}