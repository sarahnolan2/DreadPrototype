using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalGameBehavior
{
    private static string lastScene = "";

    public static string GetLastScene()
    {
        return lastScene;
    }
    public static void SetLastScene(string scene)
    {
        lastScene = scene;
    }

}
public class GameBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
