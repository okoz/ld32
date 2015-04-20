using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
	public void NewGame()
    {
        Application.LoadLevel("World");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void LoadMainMenu()
    {
        Application.LoadLevel("MainMenu");
    }
}
