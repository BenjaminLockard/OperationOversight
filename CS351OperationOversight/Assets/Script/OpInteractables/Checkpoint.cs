using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
// Modified by Lenny: Friday, 11/28/25 (fixing dialogue system)

public class Checkpoint : MonoBehaviour
{
    // LENNY'S CODE TO FIX DIALOGUE SYSTEM
    [Header("Which dialogue sequence?")]
    public string dialogueKey;
    public bool playOnlyOnce = true;
    private bool hasPlayed = false;

    private Animator animator;

    private AudioSource flagAudio;
    public AudioClip flagSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlatformerControls player = collision.gameObject.GetComponent<PlatformerControls>();

            if (player == null)
            {
                Debug.LogError("Player DNE");
                return;
            }

            flagAudio.PlayOneShot(flagSound, 0.3f);
            player.setCheckpoint(transform.position);
            animator = GetComponent<Animator>();
            animator.SetBool("Activate",true);

		    StartCoroutine(Cooldown());

            // LENNY'S CODE TO FIX DIALOGUE SYSTEM
            DialogueManager.DialogueStep[] steps = GetSteps(dialogueKey);

            if (steps != null)
            {
                DialogueManager.Instance.StartDialogue(steps);
                hasPlayed = true;
            }
            else
            {
                Debug.LogWarning("No dialogue found for key: " + dialogueKey);
            }

            // Disable collider to prevent reactivation
            GetComponent<Collider2D>().enabled = false;
        }
    }

    // LENNY'S CODE TO FIX DIALOGUE SYSTEM
    private DialogueManager.DialogueStep[] GetSteps(string key)
    {
        switch(key)
        {
            case "introCheckpoint": return DialogueManager.DialogueLibrary.introCheckpoint;
            case "checkpointOne": return DialogueManager.DialogueLibrary.checkpointOne;
            case "checkpointTwo": return DialogueManager.DialogueLibrary.checkpointTwo;
            default: return null;
        }
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(0.4f);
        animator.SetBool("Activate", false);
    }

    // Start is called before the first frame update
    void Start()
    {
        flagAudio = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        animator.SetBool("Activate",false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
