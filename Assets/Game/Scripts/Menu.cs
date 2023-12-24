using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject chacracters;
    public GameObject characterSelection;
    public GameObject menu;
    public GameObject tutorial;
    public GameObject opption;

    public void Optiom1()
    {
        menu.SetActive(false);
        characterSelection.SetActive(true);
        chacracters.SetActive(true);
    }
    public void Option3()
    {
        tutorial.SetActive(true);
    }
    public void Exit()
    {
        tutorial.SetActive(false);
    }

    public void CloseOpption()
    {
        opption.SetActive(false);
    }

    public void OpenOpption()
    {
        opption.SetActive(true);
    }
    public void QuitGame()
    {
        Application.Quit();
    } 
}
