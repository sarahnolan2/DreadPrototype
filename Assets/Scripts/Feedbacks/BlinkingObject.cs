using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingObject : MonoBehaviour
{

    public GameObject Crate; // The crate object you want to blink
    public Material white, yellow; // The materials you want to switch between
    public Camera mainCamera; // The main camera of the scene
    public float fovThreshold; // The FOV value that triggers the blinking

    //private float timer; // A timer to control the blinking frequency
    [Range(0, 10)]
    public float speed = 1;
    public float duration = 2f;
    private bool blinkObject; // A flag to indicate whether to blink or not
    private Renderer[] crateRenderers; // An array of renderers for the crate object

    private void Awake()
    {
        // Get all the renderers of the crate object and its children
        crateRenderers = Crate.GetComponentsInChildren<Renderer>(true);
        //timer = 1f;
        //blinkObject = true;
    }

    private void Update()
    {
        // Check if the camera FOV is below the threshold
        if (mainCamera.fieldOfView < fovThreshold)
        {
            // Enable blinking
            blinkObject = true;
        }
        else
        {
            // Disable blinking and reset the material to white
            blinkObject = false;
            SetMaterial(white);
        }

        // If blinking is enabled, update the timer and switch materials
        if (blinkObject)
        {
            float lerp = Mathf.PingPong(Time.time, duration) / duration; 
            crateRenderers[0].materials[0].Lerp(white, yellow,lerp); //
        }
    }

    // A helper method to set the material for all renderers
    private void SetMaterial(Material material)
    {
        foreach (var renderer in crateRenderers)
        {
            renderer.material = material; //renderer.materials[0] = material;
        }
    }
}
