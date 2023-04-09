using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    MenuPanelSwitcher menuManager;
    // Start is called before the first frame update
    void Start()
    {
        menuManager = GameObject.Find("Canvas").GetComponent<MenuPanelSwitcher>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScreen(string screenName)
    {
        menuManager.LoadScreen(screenName);
    }

    public static void LoadScene(string sceneName)
    {
        Debug.Log("Clicked " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        menuManager.QuitGame();
    }
}
