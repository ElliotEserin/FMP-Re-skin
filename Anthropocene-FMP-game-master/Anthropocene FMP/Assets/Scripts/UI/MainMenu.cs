using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainMenu : MonoBehaviour
{
    public Button continueButton;

    private void Start()
    {
        Time.timeScale = 1f;

        if(File.Exists(Application.dataPath + "/save.txt"))
        {
            continueButton.interactable = true;
        }
        else
        {
            continueButton.interactable = false;
        }
    }

    public void StartNewGame()
    {
        PlayerPrefs.SetInt("STATUS", 0);

        if(File.Exists(Application.dataPath + "/save.txt"))
            File.Delete(Application.dataPath + "/save.txt");

        SceneManager.LoadScene(1);
    }
    public void LoadGame()
    {
        SceneManager.LoadScene(2);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
