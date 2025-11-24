/*
    Author: Lenny Ozaeta
    Assignment: Team Project (Operation Oversight)
    Description: Manages the stage select screen
    Initially Created: Thursday, 11/13/25
        Modified: Sunday, 11/23/25
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Added
using TMPro; // Added

public class StageSelectManager : MonoBehaviour
{
    // Variables for "text jitter" effect
    public RectTransform rect;
    public float glitchStrength = 4f; // How many pixels glitch shakes
    public float glitchDuration = 0.1f; // How long the glitch lasts
    public float glitchInterval = 2f; // Time between glitches
    private Vector3 originalPos; // Needed to reset text to original state

    // Start is called before the first frame update
    void Start()
    {
        // EFFECT: Text Jitter
        if (rect == null)
        {
            rect = GetComponent<RectTransform>();
        }
        originalPos = rect.localPosition;
        StartCoroutine(JitterRoutine());


    }

    // Coroutine that continuously generates "text jitter" effect
    IEnumerator JitterRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(glitchInterval); // Wait until next glitch event

            float endTime = Time.time + glitchDuration;
            while (Time.time < endTime)
            {
                rect.localPosition = originalPos + new Vector3(Random.Range(-glitchStrength, glitchStrength), Random.Range(-glitchStrength, glitchStrength), 0);
                yield return null;
            }
            rect.localPosition = originalPos; // Reset to original position
        }
    }

    // This function is called when the "Stage 1" button is clicked
    public void StageOneSelect()
    {
        SceneManager.LoadScene("OperationSeer");
    }
}
