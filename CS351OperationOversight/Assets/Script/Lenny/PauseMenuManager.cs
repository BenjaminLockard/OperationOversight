/*
    Author: Lenny Ozaeta
    Assignment: Team Project (Operation Oversight)
    Description: Manages the pause menu (in the main scene)
    Initially Created: Wednesday, 11/19/25
        Modified: Friday, 11/21/25
        Modified: Friday, 11/28/25
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Added (for reloading scene)

public class PauseMenuManager : MonoBehaviour
{
    // Main variables for this class
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    // Variables for fading out music when paused (set in Inspector)
    public AudioSource musicSource; // Drag music AudioSource here
    public float fadeDuration = 0.5f; // How long "fade" will last

    // Fade-Out & Fade-In Coroutines (for fading out music)
    private Coroutine fadeCoroutine;
    private IEnumerator FadeAudio(float startVolume, float endVolume, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // Important (works even when timeScale = 0)
            musicSource.volume = Mathf.Lerp(startVolume, endVolume, elapsed / duration);
            yield return null;
        }

        musicSource.volume = endVolume;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if(GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    // Called when "Resume" button clicked
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Resume game time
        GameIsPaused = false;

        AudioListener.pause = false; // Resume all audio

        // Fade music in
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeAudio(musicSource.volume, 1f, fadeDuration));
    }

    // Called when scene first loads & whenever "Escape" key pressed
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Stop game time
        GameIsPaused = true;

        AudioListener.pause = true; // Pause all audio

        // Fade music out
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeAudio(musicSource.volume, 0f, fadeDuration));
    }

    // Called when "Restart" button clicked
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reloads this scene

        AudioListener.pause = false; // Resume all audio (avoids error where this doesn't occur after clicking this button)
    }
}
