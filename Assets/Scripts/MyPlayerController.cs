using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayerController : MonoBehaviour
{
    //public ParticleSystem waterParticle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            // make the player paddle forward left
            Debug.Log("paddle forward left");

            //move forward and left

            //particle sploosh
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            // make the player paddle forward right
            Debug.Log("paddle forward right");

            //move forward and right

            //particle sploosh
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            // make the player paddle backward left
            Debug.Log("paddle backward left");

            //move forward and left

            //particle sploosh
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            // make the player paddle backward right
            Debug.Log("paddle backward right");

            //move backwards and right

            //particle sploosh
        }
    }
}
