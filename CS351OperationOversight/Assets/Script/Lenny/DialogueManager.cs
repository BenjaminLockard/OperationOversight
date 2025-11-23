/*
    Author: Lenny Ozaeta
    Assignment: Team Project (Operation Oversight)
    Description: Manages the dialogue panel (in the main scene)
    Initially Created: Friday, 11/21/25 (originally was TutorialManager - morphed into dictionary-based dialogue system)
        Modified: Saturday, 11/22/25 (dictionary approach wasn't fully working - turned attention to array-based system instead)
    Additional Comments:
        I went with this approach to the dialogue because it seems scalable (i.e. reusable/adaptable)
        I tried to add enough comments in order to maximize comprehensibiliy
        The code at the bottom is just for backup purposes (in case current system doesn't end up working out)
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Added
using TMPro; // Added

public class DialogueManager : MonoBehaviour
{
    // ----------------------------------------------------------
    // SETTING UP VARIABLES & REFERENCES
    // ----------------------------------------------------------

    // Each dialogue "step" (i.e. each turn when a character is speaking) represented by this class
    [System.Serializable]
    public class DialogueStep
    {
        public string speakerName;
        public string line;
        public Sprite portrait; // Optional (can be removed if portraits don't end up being used)
        public bool isLeftSpeaker; // left = true, right = false
    }

    // UI References (this is what is seen in DialoguePanel)
    [Header("UI References")]
    public Image portraitLeft;
    public Image portraitRight;
    public TextMeshProUGUI nameLeft;
    public TextMeshProUGUI nameRight;
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;
    public Button continueButton;
    public Button skipButton;

    // Internal Data (dialogue will be stored in arrays; each array will correspond to dialogue for a different point in the game)
    private DialogueStep[] currentDialogue;
    private int currentIndex;

    // ----------------------------------------------------------
    // MAIN START
    // ----------------------------------------------------------
    void Start()
    {
        dialoguePanel.SetActive(false);
        StartDialogue(introDialogue); // This is what starts the first "set" of dialogue
        continueButton.onClick.AddListener(OnAdvanceInput); // Waits for player to press continueButton
    }

    // ----------------------------------------------------------
    // STEP 0: Dialogue sequence(s) stored in array(s)
    // ----------------------------------------------------------
    private DialogueStep[] introDialogue = new DialogueStep[] // This array will contain the intro dialogue
    {
        new DialogueStep
        {
            speakerName = "Operative",
            line = "...so, explain to me what I'm trying to do again?",
            isLeftSpeaker = true,
            // portrait = null
        },
        new DialogueStep
        {
            speakerName = "Overseer",
            line = "(Sigh). Get to the objective. It's that simple.",
            isLeftSpeaker = false,
            // portrait = null
        },
        new DialogueStep
        {
            speakerName = "Operative",
            line = "...what...why can't I remember how I got here? Why does everything feel..so weird..",
            isLeftSpeaker = true
        },
        new DialogueStep
        {
            speakerName = "Overseer",
            line = "You'll get used to it. Try using WASD to move.",
            isLeftSpeaker = false
        },
        new DialogueStep
        {
            speakerName = "Operative",
            line = "No...I wasn't here before, I know it...I was doing something else..and then I was here.",
            isLeftSpeaker = true
        },
        new DialogueStep
        {
            speakerName = "Overseer",
            line = "I know you're confused right now. Just focus on the objective. Use the mouse to click on interactables.",
            isLeftSpeaker = false
        },
        new DialogueStep
        {
            speakerName = "Operative",
            line = "Hold on...are you controlling me? Can I even do anyth--am I even alive?",
            isLeftSpeaker = true
        },
        new DialogueStep
        {
            speakerName = "Overseer",
            line = "Things will start to make more sense as you near completion of your objective. And, yes, you are alive; I am not controlling you; I am merely helping you reach your objective; you could even say you're 'within control' since you still have free will.",
            isLeftSpeaker = false
        }
    };

    // ----------------------------------------------------------
    // STEP 1: THIS is how you start dialogue programmatically
    // ----------------------------------------------------------
    public void StartDialogue(DialogueStep[] steps)
    {
        currentDialogue = steps;
        currentIndex = 0;

        dialoguePanel.SetActive(true);
        ShowDialogue(currentIndex);
    }

    // ----------------------------------------------------------
    // STEP 2: Displays a specific dialogue step
    // ----------------------------------------------------------
    void ShowDialogue(int index)
    {
        if (index >= currentDialogue.Length)
        {
            EndDialogue();
            return;
        }

        var step = currentDialogue[index];

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

    // ----------------------------------------------------------
    // STEP 3: Handle continueButton (and/or other player input if applicable)
    // ----------------------------------------------------------
    public void OnAdvanceInput()
    {
        var step = currentDialogue[currentIndex];

        currentIndex++;
        ShowDialogue(currentIndex);
    }

    // ----------------------------------------------------------
    // STEP 4: Close dialoguePanel
    // ----------------------------------------------------------
    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
    }

    // ----------------------------------------------------------
    // ADDITIONAL FUNCTIONS
    // ----------------------------------------------------------
    public void SkipTutorial() // Used by skipButton
    {
        EndDialogue();
    }
}

/********************************************************************************************************************/
// ==========================
// CODE FOR DICTIONARY-BASED DIALOGUE SYSTEM (DISREGARD)
// ==========================

/*
    // Each dialogue "step" (i.e. each turn when a character is speaking) represented by this class
    [System.Serializable]
    public class DialogueStep
    {
        public string speakerName;
        [TextArea]
        public string line;
        public Sprite portrait;
        public bool isLeftSpeaker; // left = true, right = false
    }

    // UI References (this is what is seen in DialoguePanel)
    public Image portraitLeft;
    public Image portraitRight;
    public TextMeshProUGUI nameLeft;
    public TextMeshProUGUI nameRight;
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;

    public Button continueButton;
    public Button skipButton;

    // Internal Data (this is how the dialogue data will be stored: using a dictionary)
    private Dictionary<string, DialogueStep[]> dialogueTable; // Overarching dictionary of arrays containing all dialogue sentences
    private DialogueStep[] currentDialogue; // Current dictionary entry
    private int currentIndex;

    // Initializes variables & sets up references before game even starts
    void Awake()
    {
        InitializeDialogueTable();
    }
    
    // ==========================
    // STEP 1: Initialize all dialogue
    // ==========================
    void InitializeDialogueTable()
    {
        // Define all sequences
        var introDialogue = new DialogueStep[] // This will be the array containing all of the introductory "tutorial" dialogue
        {
            new DialogueStep
            {
                speakerName = "Operative", line = "...so, explain to me what I'm trying to do again?", isLeftSpeaker = true
            },
            new DialogueStep
            {
                speakerName = "Overseer", line = "(Sigh). Get to the objective. It's that simple.", isLeftSpeaker = false
            },
            new DialogueStep
            {
                speakerName = "Operative", line = "...what...why can't I remember how I got here? Why does everything feel..so weird..", isLeftSpeaker = true
            },
            new DialogueStep
            {
                speakerName = "Overseer", line = "You'll get used to it. Try using WASD to move.", isLeftSpeaker = false
            },
            new DialogueStep
            {
                speakerName = "Operative", line = "No...I wasn't here before, I know it...I was doing something else..and then I was here.", isLeftSpeaker = true
            },
            new DialogueStep
            {
                speakerName = "Overseer", line = "I know you're confused right now. Just focus on the objective. Use the mouse to click on interactables.", isLeftSpeaker = false
            },
            new DialogueStep
            {
                speakerName = "Operative", line = "Hold on...are you controlling me? Can I even do anyth--am I even alive?", isLeftSpeaker = true
            },
            new DialogueStep
            {
                speakerName = "Overseer", line = "Things will start to make more sense as you near completion of your objective. And, yes, you are alive; I am not controlling you; I am merely helping you reach your objective; you could even say you're 'within control' since you still have free will.", isLeftSpeaker = false
            }
        };

        // Future dialogue can easily be added here (e.g. for different points in the game, like objectives)

        // Fill dictionary
        dialogueTable = new Dictionary<string, DialogueStep[]>
        {
            {"Intro", introDialogue},
            // Add more here as needed
        };
    }

    // ==========================
    // STEP 2: Start a dialogue by name
    // ==========================
    public void StartDialogue(string key)
    {
        if (!dialogueTable.ContainsKey(key))
        {
            Debug.LogError($"Dialogue key not found: {key}");
            return;
        }

        currentDialogue = dialogueTable[key];
        currentIndex = 0;

        dialoguePanel.SetActive(true);
        ShowDialogueStep(currentIndex);
    }

    // ==========================
    // STEP 3: Show a single step
    // ==========================
    void ShowDialogueStep(int index)
    {
        if (index >= currentDialogue.Length)
        {
            EndDialogue();
            return;
        }

        DialogueStep step = currentDialogue[index];

        // Determines speaker positioning (part of UI)
        if (step.isLeftSpeaker)
        {
            portraitLeft.gameObject.SetActive(true);
            portraitRight.gameObject.SetActive(false); // CAUSES BUG

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

        // NOTE: THIS is the line that enables dialogue to display (not sure why doesn't without it)
        dialogueText.text = step.line;
    }

    // ==========================
    // STEP 4: Advance on user input
    // ==========================
    public void Advance()
    {
        DialogueStep step = currentDialogue[currentIndex];

        // Move to next
        currentIndex++;
        ShowDialogueStep(currentIndex);
    }
    */
/*
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dialogueManager.StartDialogue("Intro");
        }
    }
*/

// ==========================
// STEP 5: End dialogue
// ==========================
/*
void EndDialogue()
{
    dialoguePanel.SetActive(false);
}

// ==========================
// ADDITIONAL STUFF
// ==========================

// Used for "Skip" button
public void SkipTutorial()
{
    EndDialogue();
}

// ==========================
// MAIN START FUNCTION
// ==========================
void Start()
{
    Awake(); // Initializes & sets up everything
    StartDialogue("Intro"); // Starts introductory dialogue set
    continueButton.onClick.AddListener(Advance); // When "continue" button pressed, dialogue proceeds
}
*/

/********************************************************************************************************************/
/********************************************************************************************************************/

// ==========================
// FORMER TUTORIALMANAGER CODE (DISREGARD)
// ==========================

/*** ********** DIALOGUE SET-UP: START ********** ***/
/*
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
*/
/*** ********** DIALOGUE SET-UP: END ********** ***/

/*** ********** ADDITIONAL SET-UP ********** ***/
/*
public GameObject tutorialPanel;

public Button continueButton;
public Button skipButton;
*/
// private bool firstRound = true;

/*** ********** TUTORIAL SET-UP: START ********** ***/
/*
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

private int currentStep = 0;
*/
/*** ********** TUTORIAL SET-UP: END ********** ***/

// Start is called before the first frame update
/*
void Start()
{
    // TUTORIAL (REMOVED)
    /*
    Time.timeScale = 0f; // Disables player controls (movement/actions); re-enabled later
    ShowStep(0);
    continueButton.onClick.AddListener(NextStep);
    */

// DIALOGUE
/*
continueButton.onClick.AddListener(NextDialogue);
ShowDialogue(0);
*/

// Copied from MedievalMedic
/*
public bool isFirstRound()
{
    return firstRound;
}
*/

/*** ********** TUTORIAL FUNCTIONS (REMOVED) ********** ***/
/*
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
*/

/*** ********** DIALOGUE FUNCTIONS ********** ***/
/*
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
*/
/*** ********** OTHER FUNCTIONS ********** ***/
/*
void EndTutorial()
{
    tutorialPanel.SetActive(false);

    // Time.timeScale = 1f; // Re-enables player controls (EDIT: no longer needed)

    // firstRound = false; // Needed to make sure tutorial only shows up on first playthrough
}

// Used for skipButton
public void SkipTutorial()
{
    EndTutorial();
}

}
*/