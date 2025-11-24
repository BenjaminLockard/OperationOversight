using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

	
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

            //disable collider to prevent reactivation
            GetComponent<Collider2D>().enabled = false;

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
