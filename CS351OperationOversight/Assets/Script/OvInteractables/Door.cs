/* Author: Cole Dixon
 * Date: 11/12/2025
 * Assignment: P06
 * Description: Rail car 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Door : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        animator.SetBool("IsOpen", true);
    }
}