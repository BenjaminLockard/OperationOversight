/*
    Author: Lenny Ozaeta
    Assignment: Team Project (Operation Oversight)
    Description: Manages the dialogue panel (in the main scene)
    Initially Created: Friday, 11/21/25 (originally was TutorialManager - morphed into dictionary-based dialogue system)
        Modified: Saturday, 11/22/25 (dictionary approach wasn't fully working - turned attention to array-based system instead)
        Modified: Sunday, 11/23/25 (troubleshooting & final touches to playtesting version)
        Modified: Wednesday, 11/26/25 (fixing dialogue system)
        Modified: Friday, 11/28/25 (fixing dialogue system)
        Modified: Sunday, 12/7/25 (adding final touches to dialogue system before final submission)
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Added
using TMPro;
using JetBrains.Annotations; // Added

public class DialogueManager : MonoBehaviour
{
    // ----------------------------------------------------------
    // SETTING UP VARIABLES & REFERENCES
    // ----------------------------------------------------------
    public static DialogueManager Instance;

    public System.Action onDialogueComplete; // Callback used to check when finalCheckpoint reached

    [Header("Character Portraits")]
    public Sprite operativePortrait;
    public Sprite overseerPortrait;

    // Each dialogue "step" (i.e. each turn when either character is speaking) represented by this class
    [System.Serializable]
    public class DialogueStep
    {
        public string speakerName;
        public string line;
        public Sprite portrait;
        public bool isLeftSpeaker; // left = true, right = false
    }

    // UI References (this is what is seen in DialoguePanel)
    [Header("UI References")]
    public Image portraitLeft; // "Operative (P1)" image
    public Image portraitRight; // "Overseer (P2)" image
    public TextMeshProUGUI nameLeft;
    public TextMeshProUGUI nameRight;
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;
    public Button continueButton;
    public Button skipButton;

    // Fields for "typewriter" effect for dialogue
    [SerializeField]
    private float typingSpeed = 0.015f; // Time (seconds) between each character's line
    private Coroutine typingCoroutine; // Allows stopping/restarting effect cleanly
    private bool isTyping = false; // Detects when player clicks "Continue" while text is still typing

    // Internal Data (dialogue will be stored in arrays; each array will correspond to a dialogue sequence for a different area)
    private DialogueStep[] currentDialogue;
    private int currentIndex;

    // This dictionary maps speaker names to sprites
    private Dictionary<string, Sprite> portraitLookup;

    /*
        Returns correct sprite for current speaker.
        If a DialogueStep already has a portrait assigned, it uses that; otherwise, it looks up the portrait based on speakerName.
    */
    public Sprite GetPortraitForStep(DialogueStep step)
    {
        if (step.portrait != null)
        {
            return step.portrait;
        }

        if(portraitLookup.TryGetValue(step.speakerName, out Sprite sprite))
        {
            return sprite;
        }

        return null; // Fallback if speaker not in dictionary
    }

    void Awake()
    {
        Instance = this;

        // Create lookup table for automatic portrait assignment
        portraitLookup = new Dictionary<string, Sprite>
        {
            {"Operative (P1)", operativePortrait},
            {"Overseer (P2)", overseerPortrait}
        };
    }

    // ----------------------------------------------------------
    // STEP 0: Dialogue sequences stored in separate arrays (each activated in their own area)
    // ----------------------------------------------------------
    public static class DialogueLibrary
    {
        public static DialogueStep[] introCheckpoint = new DialogueStep[] // This array contains the intro dialogue
        {
            new DialogueStep
            {
                speakerName = "Operative (P1)",
                line = "Alright, remind me why I'm here?",
                isLeftSpeaker = true,
                // portrait = operativePortrait
            },
            new DialogueStep
            {
                speakerName = "Overseer (P2)",
                line = "It's pretty simple. Get to the control room. I'll be helping you along the way.",
                isLeftSpeaker = false,
                // portrait = overseerPortrait
            },
            new DialogueStep
            {
                speakerName = "Operative (P1)",
                line = "And you're sure you know what you're doing?",
                isLeftSpeaker = true,
                // portrait = operativePortrait
            },
            new DialogueStep
            {
                speakerName = "Overseer (P2)",
                line = "Yeah, probably. I've managed to hack into this facility's mainframe. Go ahead and use the 'A' and 'D' keys to get a move on, or the arrow keys if you prefer. Oh, and use the space bar to jump.",
                isLeftSpeaker = false,
                // portrait = overseerPortrait
            },
            new DialogueStep
            {
                speakerName = "Operative (P1)",
                line = "It seems you've hacked into me as well, not cool. We didn't talk about this...",
                isLeftSpeaker = true,
                // portrait = operativePortrait
            },
            new DialogueStep
            {
                speakerName = "Overseer (P2)",
                line = "Don't worry, I'm not controlling you per se, but I am pulling the strings in your favor. There's some platforms in here I can move with a click, those'll give you a leg up.",
                isLeftSpeaker = false,
                // portrait = overseerPortrait
            },
            new DialogueStep
            {
                speakerName = "Operative (P1)",
                line = "Works for me, I'll get jumping.",
                isLeftSpeaker = true,
                // portrait = operativePortrait
            },
            new DialogueStep
            {
                speakerName = "Overseer (P2)",
                line = "Perfect. I'll stay in touch, this'll progressively get more difficult.",
                isLeftSpeaker = false,
                // portrait = overseerPortrait
            },
            new DialogueStep
            {
                speakerName = "Operative (P1)",
                line = "Okay, talk soon.",
                isLeftSpeaker = true,
                // portrait = operativePortrait
            },
            new DialogueStep
            {
                speakerName = "Overseer (P2)",
                line = "Just remember to pause with the 'P' key if you need a breather. Good luck.",
                isLeftSpeaker = false,
                // portrait = overseerPortrait
            }
        };

        public static DialogueStep[] checkpointOne = new DialogueStep[] // This array contains dialogue for the first checkpoint
        {
            new DialogueStep
            {
                speakerName = "Operative (P1)",
                line = "Wait, I can't make that. How do I get past this next part?",
                isLeftSpeaker = true,
                // portrait = operativePortrait
            },
            new DialogueStep
            {
                speakerName = "Overseer (P2)",
                line = "You can with my help. If I click on you mid-jump, you'll get an extra boost. That should provide the necessary lift.",
                isLeftSpeaker = false,
                // portrait = overseerPortrait
            },
            new DialogueStep
            {
                speakerName = "Operative (P1)",
                line = "Huh. Is that safe?",
                isLeftSpeaker = true,
                // portrait = operativePortrait
            },
            new DialogueStep
            {
                speakerName = "Overseer (P2)",
                line = "Maybe. Just stay on task.",
                isLeftSpeaker = false,
                // portrait = overseerPortrait
            }
        };

        public static DialogueStep[] checkpointTwo = new DialogueStep[] // This array contains dialogue for the second checkpoint
        {
            new DialogueStep
            {
                speakerName = "Operative (P1)",
                line = "I think I'm getting the hang of this. How much farther until I'm there?",
                isLeftSpeaker = true,
                // portrait = operativePortrait
            },
            new DialogueStep
            {
                speakerName = "Overseer (P2)",
                line = "I can't tell. All I can see is security getting denser from here. There's not much more I can tell you, trust your gut.",
                isLeftSpeaker = false,
                // portrait = overseerPortrait
            },
            new DialogueStep
            {
                speakerName = "Operative (P1)",
                line = "Wait, are you leaving me?",
                isLeftSpeaker = true,
                // portrait = operativePortrait
            },
            new DialogueStep
            {
                speakerName = "Overseer (P2)",
                line = "No, I'll be here. I'm just tired of talking. Over and out.",
                isLeftSpeaker = false,
                // portrait = overseerPortrait
            }
        };

        // This array contains dialogue for the final checkpoint (redirected to main menu immediately after)
        public static DialogueStep[] finalCheckpoint = new DialogueStep[]
        {
            new DialogueStep
            {
                speakerName = "Operative (P1)",
                line = "This looks to be the control room. Finally. Are you still here?",
                isLeftSpeaker = true,
            },
            new DialogueStep
            {
                speakerName = "Overseer (P2)",
                line = "Yeah, I'm here. Just hold on a second, I'll take it from here.",
                isLeftSpeaker = false,
            },
            new DialogueStep
            {
                speakerName = "Operative (P1)",
                line = "Wait, what do you mean by that?",
                isLeftSpeaker = true,
            },
            new DialogueStep
            {
                speakerName = "Overseer (P2)",
                line = "Don't worry about it. Enjoy the break.",
                isLeftSpeaker = false,
            }
        };
    }
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

        Sprite portraitToShow = GetPortraitForStep(step);

        if (step.isLeftSpeaker)
        {
            portraitLeft.sprite = portraitToShow;
            // portraitLeft.sprite = step.portrait;
            nameLeft.text = step.speakerName;

            portraitLeft.gameObject.SetActive(true);
            portraitRight.gameObject.SetActive(false);
        }
        else
        {
            portraitRight.sprite = portraitToShow;
            // portraitRight.sprite = step.portrait;
            nameRight.text = step.speakerName;

            portraitLeft.gameObject.SetActive(false);
            portraitRight.gameObject.SetActive(true);
        }

        // Stop old coroutine if text was still typing
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // Start new "typewriter" effect
        isTyping = true; // Important to set before starting coroutine
        typingCoroutine = StartCoroutine(TypeSentence(step.line));
    }

    // ----------------------------------------------------------
    // STEP 3: Handle continueButton (and/or other player input if applicable)
    // ----------------------------------------------------------
    public void OnAdvanceInput()
    {
        var step = currentDialogue[currentIndex];

        // If text is still typing, show full text instantly
        if (isTyping)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            dialogueText.text = step.line;
            isTyping = false;
            return;
        }

        // Otherwise, go to next dialogue step
        currentIndex++;
        ShowDialogue(currentIndex);
    }

    // ----------------------------------------------------------
    // STEP 4: Close dialoguePanel
    // ----------------------------------------------------------
    void EndDialogue()
    {
        dialoguePanel.SetActive(false);

        // This lets any script attach its own custom action to this event
        onDialogueComplete?.Invoke(); // Call event if it exists
        onDialogueComplete = null; // Clear event so it doesn't persist
    }

    // ----------------------------------------------------------
    // ADDITIONAL FUNCTIONS
    // ----------------------------------------------------------
    public void SkipTutorial() // Used by skipButton
    {
        EndDialogue();
    }

    // Coroutine for displaying dialogue text with "typewriter" effect
    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in sentence)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        typingCoroutine = null;
    }

    // ----------------------------------------------------------
    // MAIN START
    // ----------------------------------------------------------
    void Start()
    {
        Awake();
        dialoguePanel.SetActive(false);
        StartDialogue(DialogueLibrary.introCheckpoint); // This is what starts the first "set" of dialogue
        continueButton.onClick.AddListener(OnAdvanceInput); // Waits for player to press continueButton
    }
}

// I COULD DELETE ALL OF THE BELOW COMMENTED-OUT CODE, BUT I'LL KEEP IT HERE JUST TO SHOW HOW MUCH EFFORT I PUT INTO THIS GAME
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
                speakerName = "Operative (P1)", line = "...so, explain to me what I'm trying to do again?", isLeftSpeaker = true
            },
            new DialogueStep
            {
                speakerName = "Overseer (P2)", line = "(Sigh). Get to the objective. It's that simple.", isLeftSpeaker = false
            },
            new DialogueStep
            {
                speakerName = "Operative (P1)", line = "...what...why can't I remember how I got here? Why does everything feel..so weird..", isLeftSpeaker = true
            },
            new DialogueStep
            {
                speakerName = "Overseer (P2)", line = "You'll get used to it. Try using WASD to move.", isLeftSpeaker = false
            },
            new DialogueStep
            {
                speakerName = "Operative (P1)", line = "No...I wasn't here before, I know it...I was doing something else..and then I was here.", isLeftSpeaker = true
            },
            new DialogueStep
            {
                speakerName = "Overseer (P2)", line = "I know you're confused right now. Just focus on the objective. Use the mouse to click on interactables.", isLeftSpeaker = false
            },
            new DialogueStep
            {
                speakerName = "Operative (P1)", line = "Hold on...are you controlling me? Can I even do anyth--am I even alive?", isLeftSpeaker = true
            },
            new DialogueStep
            {
                speakerName = "Overseer (P2)", line = "Things will start to make more sense as you near completion of your objective. And, yes, you are alive; I am not controlling you; I am merely helping you reach your objective; you could even say you're 'within control' since you still have free will.", isLeftSpeaker = false
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