using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrankController : MonoBehaviour
{

    private bool hasCrank;

    [Range(8.0f,10.0f)]
    public float baseCrankAmount; //9f
    private float crankAmount;

    [Range(1,15)]
    public int alwaysInLightTimeToWait = 5; //the cooldown time that begins if the player is above the highFov (the player always being in the light)
    [Range(40,60)]
    public int highFov = 40; // fov that will determine at what point the crank will possibly break if kept above this high level for too long

    [SerializeField] private CameraController camManager;
    private ClickCounter clickCounter;
    [Range(3.0f, 7.0f)]
    public float highClickRateLimit;
    [Range(3.0f, 7.0f)]
    public float lowClickRateLimit;
    [Range(0.0f, 50.0f)]
    public float maxFOVForSingleCrank;

    private float crankFOVCounter;

    //Audio
    [SerializeField] AudioSource crankingAudioSrc;
    [SerializeField] AudioSource crankBreakSrc;
    [SerializeField] AudioSource crankGetSrc;
    [SerializeField] AudioSource crateBreakSrc; //not needed?

    // Start is called before the first frame update
    void Start()
    {
        //start with a crank
        hasCrank = true;
        clickCounter = this.gameObject.GetComponent<ClickCounter>();

        crankAmount = baseCrankAmount;
        crankFOVCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            if(hasCrank && camManager.GetHasDarknessStarted() && camManager.GetFOV() + crankAmount < maxFOVForSingleCrank) // //crankAmount + camManager.GetFOV() < camManager.GetMAXFOV()
            {
                //Debug.Log("we crankin");
                crankingAudioSrc.pitch = 1.0f;
                crankAmount = baseCrankAmount;

                // crank amount is the amount of FOV we are acquiring -> lower is more difficult, higher is easier.
                if (clickCounter.GetClickRate() > highClickRateLimit) //if player's click rate is high:
                {
                    crankAmount = baseCrankAmount - 0.5f; // make the game harder by subtracting from crank amount
                    crankingAudioSrc.pitch = 1.5f;
                }

                if (clickCounter.GetClickRate() < lowClickRateLimit) //if player's click rate is low:
                {
                    crankAmount = baseCrankAmount + 0.5f; // make the game easier by adding to crank amount
                    crankingAudioSrc.pitch = 0.5f;
                }

                crankFOVCounter += crankAmount;

                //crank the lamp
                camManager.CrankLife(crankAmount);
                crankingAudioSrc.Play();

                //Debug.Log("crankAmount: " + crankAmount);

                //maxFOV that someone can earn from cranking before it breaks: x amount
            }
        }
    }

    private void FixedUpdate()
    {
        
        
        if (hasCrank && camManager.GetHasDarknessStarted() ) //
        {
            if (crankFOVCounter > maxFOVForSingleCrank*2) 
            {
                BreakCrank(); //sets hasCrank to false and all the other changes with it
                Debug.Log("break crank. cam fov: " + camManager.GetFOV());
            }
        }
    }

    public void EarnCrank(GameObject crateObj)
    {
        
        //take the crank + sfx + remove crate
        hasCrank = true;
        Debug.Log("CRANK GET");
        crankGetSrc.Play();

        GameObject.Destroy(crateObj);
        //crateBreakSrc.Play();
    }
    
    public void BreakCrank()
    {
        //crank has broken + play sfx of being broken
        hasCrank = false;
        Debug.Log("CRANK BREAK");
        crankBreakSrc.Play();

        crankFOVCounter = 0;
    }

    public bool GetHasCrank()
    {
        //read the current crank status
        return hasCrank;
    }


    /*
    void StartTimer()
    {
        if (!timerRunning) // If the timer is not running
        {
            Debug.Log("Starting timer");
            timerRunning = true; // Set the timer status to true
        }

        if (timeRemaining > 0) // If there is time to count down
        {
            timeRemaining -= Time.deltaTime; // Subtract the delta time from the time remaining
            Debug.Log("Time remaining: " + timeRemaining);
        }
    }

    void PauseTimer()
    {
        if (timerRunning) // If the timer is running
        {
            Debug.Log("Pausing timer");
            timerRunning = false; // Set the timer status to false
        }
    }

    void ResetTimer()
    {
        Debug.Log("Resetting timer");
        timeRemaining = alwaysInLightTimeToWait; // Set the time remaining back to 8 seconds
    }
    */
}
