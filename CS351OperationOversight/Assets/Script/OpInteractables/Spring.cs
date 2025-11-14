using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    // in inspector: -1= left launch, 0 = up, 1 = right
    // use horis sparingly, a bit janky
    public int indicateDirection;
    // 15 recommended
    public float launchPower;

    //If horisontal, set to -1 for launch left, 1 for launch right in inspector
    private Vector2 direction;
    private bool isHori;

    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlatformerControls player = collision.gameObject.GetComponent<PlatformerControls>();

            if (player == null)
            {
                Debug.LogError("Player DNE");
                return;
            }

            switch (indicateDirection)
            {
                case -1:
                    direction = (Vector2.left + Vector2.up/2).normalized;
                    isHori = true;
                    break;
                case 0:
                    direction = Vector2.up;
                    isHori = false;
                    break;
                case 1:
                    direction = (Vector2.right + Vector2.up/2).normalized;
                    isHori = true;
                    break;
            }

            player.launch(direction, launchPower, isHori);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
