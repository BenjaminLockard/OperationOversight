/*
    Author: Lenny Ozaeta
    Assignment: Team Project (Operation Oversight)
    Description: Manages the title screen
    Initially Created: Tuesday, 11/11/25
        Modified: Thursday, 11/13/25
        Modified: Wednesday, 11/19/25
        Modified: Monday, 11/24/25
        Modified: Friday, 11/28/25
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Added (for changing scene)
using TMPro; // Added (for "text glitch" effect)
using DG.Tweening; // Added (for "boot-up" sequence & its "glitch flash")
using UnityEngine.UI; // Added (for "glitch flash")

public class TitleScreenManager : MonoBehaviour
{
    // Variables for "boot-up" sequence
    [Header("UI References")]
    public TextMeshProUGUI bootText;
    public CanvasGroup mainMenu;
    public CanvasGroup blackOverlay;
    public Image blackOverlayImage;

    [Header("Boot Settings")]
    [TextArea(5, 20)]
    public string[] bootLines; // Each line shown one after the other (set in Inspector)
    public float lineDelay = 0.2f;
    public float fadeDuration = 0.8f;

    // Variables for "text glitch" effect
    [Header("Text Glitch Settings")]
    public TextMeshProUGUI tmp;
    public float glitchDuration = 0.30f; // How long each glitch lasts
    public float glitchInterval = 3f; // Time between glitches
    private string originalText;
    private char[] randomChars = "!@#$%^&*()<>?/[]{}+=-".ToCharArray(); // The glitch

    // Other UI variables
    [Header("Typewriter Settings")]
    public float typingSpeed = 0.02f; // Set in Inspector

    // Start is called before the first frame update
    void Start()
    {
        // Disables main menu & activates "boot-up sequence" UI
        mainMenu.alpha = 0;
        bootText.text = "";
        StartCoroutine(RunBootSequence());

        // Activates "text glitch" effect for main title text
        originalText = tmp.text;
        StartCoroutine(GlitchRoutine());
    }

    // This function is called when the "Stage Select" button is clicked
    public void StageSelect()
    {
        SceneManager.LoadScene("StageSelectScreen");
    }

    // Generates "boot-up" sequence
    private System.Collections.IEnumerator RunBootSequence()
    {
        blackOverlay.DOFade(0, fadeDuration); // Fade in from black

        yield return new WaitForSeconds(0.5f); // Wait a bit before text starts

        foreach (string line in bootLines) // Type each line (with "typewriter" effect)
        {
            // bootText.text += line + "\n";
            // yield return new WaitForSeconds(lineDelay);
            yield return StartCoroutine(TypeLine(line));
            yield return new WaitForSeconds(lineDelay);
        }

        yield return GlitchFlash(); // Optional: quick "glitch flash"

        bootText.DOFade(0, 0.5f); // Fade boot text out
        yield return new WaitForSeconds(0.5f);

        // Reveal (enable) main menu
        mainMenu.gameObject.SetActive(true);
        mainMenu.DOFade(1, 1f);

        blackOverlayImage.gameObject.SetActive(false); // Makes sure continueButton can still be clicked
    }

    // Optional: Generates "glitch flash" 
    private System.Collections.IEnumerator GlitchFlash()
    {
        // Creates a quick white flash (by setting flash color using Image component)
        blackOverlayImage.color = new Color(1, 1, 1, 0); // Transparent white

        // The actual "flash" (using CanvasGroup alpha)
        blackOverlay.DOFade(0.3f, 0.05f);
        yield return new WaitForSeconds(0.05f);
        blackOverlay.DOFade(0, 0.1f);
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

    // Coroutine that generates "typewriter" effect for "boot-up" sequence
    private IEnumerator TypeLine(string line)
    {
        foreach (char c in line)
        {
            bootText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        // After finishing a line, add a line break
        bootText.text += "\n";
    }
}
