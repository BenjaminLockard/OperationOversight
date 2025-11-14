/*
    Author: Lenny Ozaeta
    Assignment: Team Project (Operation Oversight)
    Description: Manages the title screen
    Initially Created: Tuesday, 11/11/25
        Modified: Thursday, 11/13/25
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Added

public class TitleScreenManager : MonoBehaviour
{
    // This function is called when the "Stage Select" button is clicked
    public void StageSelect()
    {
        SceneManager.LoadScene("StageSelectScreen");
    }
}
