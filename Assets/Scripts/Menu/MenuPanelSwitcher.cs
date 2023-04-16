using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanelSwitcher : MonoBehaviour
{

    public GameObject mainMenuScreen;
    public GameObject loadingScreen;

    List<GameObject> menuScreens;
    private string currentScreenName;
    private GameObject currentScreen;

    // Start is called before the first frame update
    void Start()
    {
        menuScreens = new List<GameObject>();

        menuScreens.Add(mainMenuScreen);
        menuScreens.Add(loadingScreen);

        foreach (GameObject screen in menuScreens)
        {
            screen.SetActive(false);
        }

        LoadScreen("MainMenuPanel");

        Cursor.lockState = CursorLockMode.None;

        //-----------------------

        if (GlobalGameBehavior.GetLastScene().Equals(""))
        {
            GlobalGameBehavior.SetLastScene("Menu");
        }
    }


    public void LoadScreen(string panelName)
    {
        Debug.Log(panelName);
        if (currentScreen)
            currentScreen.SetActive(false);

        foreach (GameObject screen in menuScreens)
        {
            if (screen.name.Equals(panelName))
            {
                currentScreenName = screen.name;
                currentScreen = screen;
            }
        }

        currentScreen.SetActive(true);
    }

    public void QuitGame()
    {
        // save any game data here
        #if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
