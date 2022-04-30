// Author: Zachary Patrone
// CS583 - Professor Price
// Script for Main Menu

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }


    // used in Create Character Scene
    public void PlayGame(float difficulty)
    {
        GameController.instance.difficulty = difficulty;
        SceneManager.LoadScene(1);
    }

    public void About()
    {
        SceneManager.LoadScene(2);
    }

    public void HowToPlay()
    {
        SceneManager.LoadScene(3);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // Takes user to Create Character Scene after pressing "PLAY GAME"
    public void PlayGameChar()
    {
        SceneManager.LoadScene(4);
    }

}
