using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayerController : MonoBehaviour
{
    //public ParticleSystem waterParticle;

    public GameObject character;

    public float boatRotation = 9.0f;
    public float boatSpeed = 150.0f;

    public GameObject particles;

    private bool paddleFLeft;
    private bool paddleFRight;
    private bool paddleBLeft;
    private bool paddleBRight;

    // Start is called before the first frame update
    void Start()
    {
        paddleFLeft = false;
        paddleFRight = false;
        paddleBLeft = false;
        paddleBRight = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            paddleFLeft = true;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            paddleFRight = true;
            
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            paddleBLeft = true;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            paddleBRight = true;
        }
    }

    private void FixedUpdate()
    {
        if(paddleFLeft)
        {
            // make the player paddle forward left
            //Debug.Log("paddle forward left");

            //move forward and left
            //this.transform.Translate(new Vector3(-0.5f, 0, -0.5f), Space.Self);

            this.GetComponent<Rigidbody>().AddForce(new Vector3(-boatSpeed, 0, -boatSpeed));

            this.GetComponent<Transform>().Rotate(0, -boatRotation, 0);

            //particle sploosh
            //GameObject.Instantiate(particles, (this.transform.position + new Vector3(-10.0f,1.0f,-10.0f)), Quaternion.identity);

            paddleFLeft = false;
        }

        if(paddleFRight)
        {
            // make the player paddle forward right
            //Debug.Log("paddle forward right");

            //move forward and right

            this.GetComponent<Rigidbody>().AddForce(new Vector3(-boatSpeed, 0, boatSpeed));

            this.GetComponent<Transform>().Rotate(0, boatRotation, 0);

            //particle sploosh
            //GameObject.Instantiate(particles, (this.transform.position + new Vector3(-10.0f, 1.0f, 10.0f)), Quaternion.identity);

            paddleFRight = false;
        }

        if (paddleBLeft)
        {
            // make the player paddle backward left
            //Debug.Log("paddle backward left");

            //move backward and left
            this.GetComponent<Rigidbody>().AddForce(new Vector3(boatSpeed, 0, -boatSpeed));

            this.GetComponent<Transform>().Rotate(0, boatRotation, 0);

            //particle sploosh
            //GameObject.Instantiate(particles, (this.transform.position + new Vector3(10.0f, 1.0f, -10.0f)), Quaternion.identity);

            paddleBLeft = false;
        }

        if(paddleBRight)
        {
            // make the player paddle backward right
            //Debug.Log("paddle backward right");

            //move backwards and right
            this.GetComponent<Rigidbody>().AddForce(new Vector3(boatSpeed, 0, boatSpeed));

            this.GetComponent<Transform>().Rotate(0, -boatRotation, 0);

            //particle sploosh
            //GameObject.Instantiate(particles, (this.transform.position + new Vector3(10.0f, 1.0f, 10.0f)), Quaternion.identity);

            paddleBRight = false;
        }
    }
}
