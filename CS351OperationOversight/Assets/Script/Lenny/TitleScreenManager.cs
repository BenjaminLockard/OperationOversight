/*
    Author: Lenny Ozaeta
    Assignment: Team Project (Operation Oversight)
    Description: Manages the title screen
    Initially Created: Tuesday, 11/11/25
        Modified: Thursday, 11/13/25
        Modified: Wednesday, 11/19/25
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Added (for changing scene)
using TMPro; // Added (for "text glitch" effect)

public class TitleScreenManager : MonoBehaviour
{
    // Variables for "text glitch" effect
    public TextMeshProUGUI tmp;
    public float glitchDuration = 0.30f; // How long each glitch lasts
    public float glitchInterval = 3f; // Time between glitches
    private string originalText;
    private char[] randomChars = "!@#$%^&*()<>?/[]{}+=-".ToCharArray(); // The glitch

    // This function is called when the "Stage Select" button is clicked
    public void StageSelect()
    {
        SceneManager.LoadScene("StageSelectScreen");
    }

    // Start is called before the first frame update
    void Start()
    {
        originalText = tmp.text;
        StartCoroutine(GlitchRoutine());
    }

    // Coroutine that continuously generates "text glitch" effect
    IEnumerator GlitchRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(glitchInterval);

            // Scrambles text
            float endTime = Time.time + glitchDuration;
            while (Time.time < endTime)
            {
                tmp.text = GenerateGlitchText(originalText);
                yield return null;
            }

            // Restore original text
            tmp.text = originalText;
        }
    }

    // Generates random text for "text glitch" effect
    string GenerateGlitchText(string baseText)
    {
        char[] result = baseText.ToCharArray();

        for (int i = 0;  i < result.Length; i++)
        {
            if (Random.value > 0.8f) // 20% chance per character
            {
                result[i] = randomChars[Random.Range(0, randomChars.Length)];
            }
        }
        return new string(result);
    }
}
