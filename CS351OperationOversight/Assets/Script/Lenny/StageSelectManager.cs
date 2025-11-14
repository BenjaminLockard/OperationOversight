/*
    Author: Lenny Ozaeta
    Assignment: Team Project (Operation Oversight)
    Description: Manages the stage select screen
    Initially Created: Thursday, 11/13/25
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Added

public class StageSelectManager : MonoBehaviour
{
    // This function is called when the "Stage 1" button is clicked
    public void StageOneSelect()
    {
        SceneManager.LoadScene("OperationSeer");
    }
}
