using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickCounter : MonoBehaviour
{
    //store the number of clicks
    private int clicks = 0;

    //store the time elapsed
    private float timer = 0f;

    //store the click rate per second
    private float clickRate = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Add the time elapsed since the last frame to the timer
        timer += Time.deltaTime;

        // If the left mouse button is clicked, increment the clicks
        if (Input.GetMouseButtonDown(0))
        {
            clicks++;
        }

        // If the timer reaches one second, calculate the click rate and reset the timer and clicks
        if (timer >= 1f)
        {
            clickRate = clicks / timer;
            timer = 0f;
            clicks = 0;

            // Print the click rate to the console for debugging
            Debug.Log("Click rate: " + clickRate + " clicks per second");
        }
    }

    public float GetClickRate()
    {
        return clickRate;
    }
}
