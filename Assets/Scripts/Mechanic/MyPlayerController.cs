using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MyPlayerController : MonoBehaviour
{
    //public ParticleSystem waterParticle;

    public GameObject character;

    public float boatRotation = 9.0f;
    public float boatSpeed = 150.0f;
    public float topSpeed = 50.0f;

    private Vector3 boatAccelerationDirection = new Vector3(-1,0,0);
    private Vector3 boatBackAccelerationDirection = new Vector3(1,0,0); 
    [Range(1,30)]
    public float boatAccelerationForce = 1f;
    

    //---------------------------

    Quaternion endRightRotation;
    Quaternion endLeftRotation;

    float rotationCounter = 0f;
    [Range(0.1f,3.0f)]
    public float rotationDuration = 0.5f;

    bool isCoroutineRunning;

    //----------------------

    public GameObject particles;

    private bool paddleFLeft;
    private bool paddleFRight;
    private bool paddleBLeft;
    private bool paddleBRight;

    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private AudioSource paddleAudioSource;

    [SerializeField]
    TextMeshProUGUI debugText;

    // Start is called before the first frame update
    void Start()
    {
        paddleFLeft = false;
        paddleFRight = false;
        paddleBLeft = false;
        paddleBRight = false;

        endRightRotation = Quaternion.Euler(0f, 0.05f, 0f); //0.1 //boatRotation
        endLeftRotation = Quaternion.Euler(0f, -0.05f, 0f); //-0.1 //-boatRotation

        isCoroutineRunning = false;
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


        
        if(Input.GetKeyDown(KeyCode.U))
        {
            //restart the game if stuck
            SceneManager.LoadScene("SampleScene");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //quit to menu
            SceneManager.LoadScene("Menu");
        }
    }

    private void FixedUpdate()
    {
        if(paddleFLeft && !isCoroutineRunning)
        {
            paddleAudioSource.Play();
            // make the player paddle forward left
            //Debug.Log("paddle forward left");

            //move forward and left
            //this.transform.Translate(new Vector3(-0.5f, 0, -0.5f), Space.Self);

            //rb.AddForce(boatAccelerationDirection * boatAccelerationForce, ForceMode.Acceleration);

            rb.AddRelativeForce(new Vector3(-boatSpeed, 0, -boatSpeed));

            //this.GetComponent<Transform>().Rotate(0, -boatRotation, 0);
            StartCoroutine(RotateBoatLeft());
            
            if (rb.velocity.magnitude > topSpeed)
                rb.velocity = rb.velocity.normalized * topSpeed;

            //particle sploosh
            //GameObject.Instantiate(particles, (this.transform.position + new Vector3(-10.0f,1.0f,-10.0f)), Quaternion.identity);
            
            paddleFLeft = false;
        }

        if(paddleFRight && !isCoroutineRunning)
        {
            paddleAudioSource.Play();
            // make the player paddle forward right
            //Debug.Log("paddle forward right");

            //move forward and right

           // rb.AddForce(boatAccelerationDirection * boatAccelerationForce, ForceMode.Acceleration);

            this.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(-boatSpeed, 0, boatSpeed));

            //this.GetComponent<Transform>().Rotate(0, boatRotation, 0);
            StartCoroutine(RotateBoatRight());

            if (rb.velocity.magnitude > topSpeed)
                rb.velocity = rb.velocity.normalized * topSpeed;

            //particle sploosh
            //GameObject.Instantiate(particles, (this.transform.position + new Vector3(-10.0f, 1.0f, 10.0f)), Quaternion.identity);

            paddleFRight = false;
        }


        
        if (paddleBLeft && !isCoroutineRunning)
        {
            paddleAudioSource.Play();
            // make the player paddle backward left
            //Debug.Log("paddle backward left");

            //move backward and left
            rb.AddForce(boatBackAccelerationDirection * boatAccelerationForce, ForceMode.Acceleration);

            this.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(boatSpeed, 0, -boatSpeed));

            //this.GetComponent<Transform>().Rotate(0, boatRotation, 0);
            StartCoroutine(RotateBoatRight());

            if (rb.velocity.magnitude > topSpeed)
                rb.velocity = rb.velocity.normalized * topSpeed;

            //particle sploosh
            //GameObject.Instantiate(particles, (this.transform.position + new Vector3(10.0f, 1.0f, -10.0f)), Quaternion.identity);

            paddleBLeft = false;
        }

        if(paddleBRight && !isCoroutineRunning)
        {
            paddleAudioSource.Play();
            // make the player paddle backward right
            //Debug.Log("paddle backward right");

            //move backwards and right
            rb.AddForce(boatBackAccelerationDirection * boatAccelerationForce, ForceMode.Acceleration);

            this.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(boatSpeed, 0, boatSpeed));

            //this.GetComponent<Transform>().Rotate(0, -boatRotation, 0);
            StartCoroutine(RotateBoatLeft());

            if (rb.velocity.magnitude > topSpeed)
                rb.velocity = rb.velocity.normalized * topSpeed;

            //particle sploosh
            //GameObject.Instantiate(particles, (this.transform.position + new Vector3(10.0f, 1.0f, 10.0f)), Quaternion.identity);

            paddleBRight = false;
        }
        //------------------------
        debugText.text = "\nSpeed: " + gameObject.GetComponent<Rigidbody>().velocity;
    }

    IEnumerator RotateBoatLeft()
    {
        isCoroutineRunning = true;
        //While the elapsed time is less than the duration
        while (rotationCounter < rotationDuration)
        {
            //Increase the counter by delta time
            rotationCounter += Time.deltaTime;
            //Apply the rotation to the object
            transform.rotation = transform.rotation * endLeftRotation;
            //Wait for next frame
            yield return null;
        }
        rotationCounter = 0f;
        yield return new WaitForSecondsRealtime(0.2f);
        isCoroutineRunning = false;
    }

    IEnumerator RotateBoatRight()
    {
        isCoroutineRunning = true;
        //While the elapsed time is less than the duration
        while (rotationCounter < rotationDuration)
        {
            //Increase the counter by delta time
            rotationCounter += Time.deltaTime;
            //Apply the rotation to the object
            transform.rotation = transform.rotation * endRightRotation;
            //Wait for next frame
            yield return null;
        }
        rotationCounter = 0f;
        yield return new WaitForSecondsRealtime(0.2f);
        isCoroutineRunning = false;
    }
}
