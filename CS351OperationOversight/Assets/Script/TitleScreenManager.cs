/*
    Author: Lenny Ozaeta
    Assignment: Team Project (Operation Oversight)
    Description: Manages the title screen
    Initially Created: Tuesday, 11/11/25
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    // This function is called when the "Level Select" button is clicked
    public void LevelSelect()
    {
        SceneManager.LoadScene("LevelSelectScreen");
    }
}
