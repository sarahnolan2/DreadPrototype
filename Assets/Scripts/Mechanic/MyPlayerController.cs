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
    public float rotationDuration = 0.5f; //0.72 with coroutines

    bool isCoroutineRunning;

    bool isRotatingLeft;
    bool isRotatingRight;
    float rotationEndThreshold = 0.2f;
    float rotationSpeed = 1.5f;

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

        endRightRotation = Quaternion.Euler(0f, 0.8f, 0f); //0.05 w coroutines //boatRotation
        endLeftRotation = Quaternion.Euler(0f, -0.8f, 0f); //-0.05 w coroutines //-boatRotation

        isCoroutineRunning = false;
        isRotatingLeft = false;
        isRotatingRight = false;
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
        //if player is currently rotating - isRotating = true;
        //if player is not currently rotating - isRotating = false;

        if(paddleFLeft && !isCoroutineRunning && !isRotatingLeft && !isRotatingRight) //only runs once to move
        {
            paddleAudioSource.Play();
            // make the player paddle forward left
            //Debug.Log("paddle forward left");

            //move forward and left
            //this.transform.Translate(new Vector3(-0.5f, 0, -0.5f), Space.Self);

            //rb.AddForce(boatAccelerationDirection * boatAccelerationForce, ForceMode.Acceleration);

            rb.AddRelativeForce(new Vector3(-boatSpeed, 0, -boatSpeed));

            //this.GetComponent<Transform>().Rotate(0, -boatRotation, 0);
            //StartCoroutine(RotateBoatLeft());
            isRotatingLeft = true;


            if (rb.velocity.magnitude > topSpeed)
                rb.velocity = rb.velocity.normalized * topSpeed;

            //particle sploosh
            //GameObject.Instantiate(particles, (this.transform.position + new Vector3(-10.0f,1.0f,-10.0f)), Quaternion.identity);

            Debug.Log("paddleFLeft");
            paddleFLeft = false;
        }        

        if(paddleFRight && !isCoroutineRunning && !isRotatingLeft && !isRotatingRight) //only runs once to move
        {
            paddleAudioSource.Play();
            // make the player paddle forward right
            //Debug.Log("paddle forward right");

            //move forward and right

           // rb.AddForce(boatAccelerationDirection * boatAccelerationForce, ForceMode.Acceleration);

            this.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(-boatSpeed, 0, boatSpeed));

            //this.GetComponent<Transform>().Rotate(0, boatRotation, 0);
            //StartCoroutine(RotateBoatRight());
            isRotatingRight = true;

            if (rb.velocity.magnitude > topSpeed)
                rb.velocity = rb.velocity.normalized * topSpeed;

            //particle sploosh
            //GameObject.Instantiate(particles, (this.transform.position + new Vector3(-10.0f, 1.0f, 10.0f)), Quaternion.identity);
            Debug.Log("paddleFRight");
            paddleFRight = false;
        }

        
        if (paddleBLeft && !isCoroutineRunning && !isRotatingLeft && !isRotatingRight) //only runs once to move
        {
            paddleAudioSource.Play();
            // make the player paddle backward left
            //Debug.Log("paddle backward left");

            //move backward and left
            rb.AddForce(boatBackAccelerationDirection * boatAccelerationForce, ForceMode.Acceleration);

            this.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(boatSpeed, 0, -boatSpeed));

            //this.GetComponent<Transform>().Rotate(0, boatRotation, 0);
            //StartCoroutine(RotateBoatRight());
            isRotatingRight = true;

            if (rb.velocity.magnitude > topSpeed)
                rb.velocity = rb.velocity.normalized * topSpeed;

            //particle sploosh
            //GameObject.Instantiate(particles, (this.transform.position + new Vector3(10.0f, 1.0f, -10.0f)), Quaternion.identity);

            Debug.Log("paddleBLeft");
            paddleBLeft = false;
        }

        if (paddleBRight && !isCoroutineRunning && !isRotatingLeft && !isRotatingRight) //only runs once to move
        {
            paddleAudioSource.Play();
            // make the player paddle backward right
            //Debug.Log("paddle backward right");

            //move backwards and right
            rb.AddForce(boatBackAccelerationDirection * boatAccelerationForce, ForceMode.Acceleration);

            this.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(boatSpeed, 0, boatSpeed));

            //this.GetComponent<Transform>().Rotate(0, -boatRotation, 0);
            //StartCoroutine(RotateBoatLeft());
            isRotatingLeft = true;

            if (rb.velocity.magnitude > topSpeed)
                rb.velocity = rb.velocity.normalized * topSpeed;

            //particle sploosh
            //GameObject.Instantiate(particles, (this.transform.position + new Vector3(10.0f, 1.0f, 10.0f)), Quaternion.identity);
            Debug.Log("paddleBRight");
            paddleBRight = false;
        }

        if(isRotatingRight)
        {
            RotateBoatRight2();
        }

        if (isRotatingLeft)
        {
            RotateBoatLeft2();
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
            rotationCounter += Time.deltaTime; //Time.deltaTime // 0.0015f
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
            rotationCounter += Time.deltaTime; //Time.deltaTime // 0.0015f
            //Apply the rotation to the object
            transform.rotation = transform.rotation * endRightRotation;
            //Wait for next frame
            yield return null;
        }
        rotationCounter = 0f;
        yield return new WaitForSecondsRealtime(0.2f);
        isCoroutineRunning = false;
    }

    void RotateBoatLeft2()
    {
        Debug.Log("RotateBoatLeft2");
        paddleBRight = true; //these prevent the player from adding additional movements to a "movement queue" when pressing a movement key while the player is currently still rotating  
        paddleFLeft = true;
        paddleBLeft = true;
        paddleFRight = true;

        //While the elapsed time is less than the duration
        if (rotationCounter < rotationDuration)
        {
            //Increase the counter by delta time
            rotationCounter += Time.deltaTime; //Time.deltaTime // 0.0015f
            Debug.Log("rotCounter: " + rotationCounter);
            //Apply the rotation to the object
            transform.rotation = transform.rotation * endLeftRotation;
            return;
        }
        
        /*
        //wait 0.2f until continuing        
        float waitingCounter = 0f;
        if (waitingCounter < rotationEndThreshold)
        {
            waitingCounter += Time.deltaTime;
            return;
        }*/

        rotationCounter = 0f;
        paddleBRight = false;
        paddleFLeft = false;
        paddleBLeft = false;
        paddleFRight = false;
        isRotatingLeft = false;
    }

    void RotateBoatRight2()
    {
        Debug.Log("RotateBoatRight2");
        paddleBLeft = true;
        paddleFRight = true;
        paddleBRight = true;
        paddleFLeft = true;

        //While the elapsed time is less than the duration
        if (rotationCounter < rotationDuration)
        {
            //Increase the counter by delta time
            rotationCounter += Time.deltaTime; //Time.deltaTime // 0.0015f
            Debug.Log("rotCounter: "+rotationCounter);
            //Apply the rotation to the object
            transform.rotation = transform.rotation * endRightRotation;
            return;
        }
        
        /*
        //wait 0.2f until continuing           
        float waitingCounter = 0f;
        if (waitingCounter < rotationEndThreshold)
        {
            waitingCounter += Time.deltaTime;
            return;
        }*/

        rotationCounter = 0f;
        paddleBLeft = false;
        paddleFRight = false;
        paddleBRight = false;
        paddleFLeft = false;
        isRotatingRight = false;
    }
}
