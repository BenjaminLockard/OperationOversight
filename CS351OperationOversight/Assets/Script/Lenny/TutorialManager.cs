/*
    Author: Lenny Ozaeta
    Assignment: Team Project (Operation Oversight)
    Description: Manages the tutorial panel (in the main scene)
    Initially Created: Thursday, 11/20/25
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Added
using TMPro; // Added

public class TutorialManager : MonoBehaviour
{
    /*** DIALOGUE SET-UP: START ***/
    [System.Serializable]
    public class DialogueStep
    {
        public string speakerName;
        [TextArea]
        public string line;
        public Sprite portrait;
        public bool isLeftSpeaker; // left = true, right = false;
    }

    public DialogueStep[] dialogueSteps;

    public Image portraitLeft;
    public Image portraitRight;
    public TextMeshProUGUI nameLeft;
    public TextMeshProUGUI nameRight;
    public TextMeshProUGUI dialogueText;

    private int currentIndex = 0;
    /*** DIALOGUE SET-UP: END ***/

    /*** TUTORIAL SET-UP: START ***/
    [System.Serializable]
    public class TutorialStep
    {
        [TextArea]
        public string storyText;
        public string controlHint;
    }

    public TutorialStep[] steps;

    public TextMeshProUGUI storyTextUI;
    public TextMeshProUGUI controlHintUI;

    public GameObject tutorialPanel;
    public Button continueButton;

    private int currentStep = 0;
    /*** TUTORIAL SET-UP: END ***/

    // Optional
    public Button skipButton;
    // private bool firstRound = true;

    // Start is called before the first frame update
    void Start()
    {
        // TUTORIAL
        Time.timeScale = 0f; // Disables player controls (movement/actions); re-enabled later
        ShowStep(0);
        continueButton.onClick.AddListener(NextStep);

        // DIALOGUE
        continueButton.onClick.AddListener(NextDialogue);
        ShowDialogue(0);
    }

    /*
    // Copied from MedievalMedic
    public bool isFirstRound()
    {
        return firstRound;
    }
    */

    void ShowStep(int index)
    {
        if (index < steps.Length)
        {
            tutorialPanel.SetActive(true);
            storyTextUI.text = steps[index].storyText;
            controlHintUI.text = steps[index].controlHint;
        }
        else
        {
            EndTutorial();
        }
    }

    void NextStep()
    {
        currentStep++;
        ShowStep(currentStep);
    }

    void ShowDialogue(int index)
    {
        if (index >= dialogueSteps.Length)
        {
            EndTutorial();
            return;
        }

        tutorialPanel.SetActive(true);
        var step = dialogueSteps[index];

        // Determine speaker side (i.e. which character currently speaking)
        if (step.isLeftSpeaker)
        {
            portraitLeft.gameObject.SetActive(true);
            portraitRight.gameObject.SetActive(false);

            portraitLeft.sprite = step.portrait;
            nameLeft.text = step.speakerName;
        }
        else
        {
            portraitLeft.gameObject.SetActive(false);
            portraitRight.gameObject.SetActive(true);

            portraitRight.sprite = step.portrait;
            nameRight.text = step.speakerName;
        }

        dialogueText.text = step.line;
    }

    void NextDialogue()
    {
        currentIndex++;
        ShowDialogue(currentIndex);
    }

    void EndTutorial()
    {
        tutorialPanel.SetActive(false);
        Time.timeScale = 1f; // Re-enables player controls

        // firstRound = false; // Needed to make sure tutorial only shows up on first playthrough
    }

    public void SkipTutorial()
    {
        EndTutorial();
    }
}
