/* Author: Benjamin Lockard
 * Date: 11/13/2025
 * Assignment: P06
 * Description: Camera tracks player
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrackOp : MonoBehaviour
{
    //Set reference in inspector
    public GameObject player;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(
            player.transform.position.x,
            player.transform.position.y,
            transform.position.z);
    }
}
