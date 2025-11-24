using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

	public Door doorToOpen;
    private Animator animator;

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

            player.setCheckpoint(transform.position);
            animator = GetComponent<Animator>();
            animator.SetBool("Activate",true);

		if (doorToOpen != null)
		{
			doorToOpen.OpenDoor();
		}

            StartCoroutine(Cooldown());
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
        animator = GetComponent<Animator>();
        animator.SetBool("Activate",false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
