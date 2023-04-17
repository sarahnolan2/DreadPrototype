using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class CameraController : MonoBehaviour
{
    
    public Transform player;
    private float playerLife; // determined by the camera FOV

    private float countDownTime;

    //how far the camera is from the player at the start
    public float cameraInitOffsetY = 4.0f;
    public float cameraInitOffsetZ = 15.0f;

    public float cameraRotationSpeed = 4.0f;
    public float cameraZoomSpeed; //0.01 is a good speed?
    public float finalFOV; //final distance from player

    private Material ogSkybox;
    public Material otherSkybox;
    private PhysicMaterial ogPhysicMaterial;
    public PhysicMaterial otherPhysMaterial;
    public Material water;
    public Light sunlight;

    private Vector3 offset;
    private Camera cam;
    private float initialFOV;
    private bool hasDarknessStarted;
    public float amountFOV = 40f;
    private float resultFOV;

    private bool isCrankCalled; //checks if we are incrementing the lamp with the crank or the buoy

    private bool isTimerRunning;
    //private float startTime;
    private bool didWeChangeMood;
    private float speedModifier;

    private ChromaticAberration chrome;
    private LensDistortion lens;
    private FilmGrain film;
    private ColorAdjustments adj;
    private SplitToning split;
    private Tonemapping tone;
    private Vignette vignette;
    
    //private AudioSource audioSource = new AudioSource();
    public CrossfadeMusic crossfade;
    public AudioClip[] audioClips = new AudioClip[3];


    [SerializeField] private GameObject MimicPrefab;


    //UI
    public TextMeshProUGUI debugText;
    private DisplayText display;

    void resetVariables()
    {

        playerLife = cam.fieldOfView;
        sunlight.intensity = 2f;

        // reset water material
        water.color = new Color(0.0f / 255.0f, 79.0f / 255.0f, 190.0f / 255.0f); //reset it to blue
        water.SetFloat("_Smoothness", 1.0f); //reset smoothness

        //reset post p
        split.active = false;
        chrome.intensity.Override(0.0f);
        film.intensity.Override(0.0f);
        lens.intensity.Override(0.0f);
        adj.contrast.Override(0.0f);
        adj.saturation.Override(0.0f);
        tone.mode.Override(TonemappingMode.Neutral);
        //vignette.intensity.Override(0.0f);
    }
    // Start is called before the first frame update
    void Start()
    {
        //------------------------------------------------------------------------------------------------------
        //if game hasn't loaded the menu yet, load the menu
        if (GlobalGameBehavior.GetLastScene().Equals(""))
        {
            SceneManager.LoadScene("Menu");
        }

        //------------------------------------------------------------------------------------------------------
        crossfade.newSoundtrack(audioClips[0]);
        // camera position
        cam = GetComponent<Camera>();
        offset = new Vector3(player.position.x + cam.transform.position.x, player.position.y + cam.transform.position.y, player.position.z + cam.transform.position.z); //(player.position.x, player.position.y + 8.0f, player.position.z + 7.0f);
                                                                                                                                                                        //offset = new Vector3(player.position.x, player.position.y + cameraInitOffsetY, player.position.z + cameraInitOffsetZ); //(player.position.x, player.position.y + 8.0f, player.position.z + 7.0f);
                                                                                                                                                                        
        ogSkybox = RenderSettings.skybox;
        ogPhysicMaterial = GameObject.Find("SeaFloor").GetComponent<Collider>().material;
        //RenderSettings.ambientIntensity = 0.4f;
        display = GameObject.Find("Canvas").GetComponent<DisplayText>();

        didWeChangeMood = false;
        initialFOV = cam.fieldOfView;
        playerLife = cam.fieldOfView;
        hasDarknessStarted = false;
        //isCameraRotationFree = true; //can use this to switch from free roaming camera to the single axis camera.
        speedModifier = 2f; 
              
        // post p  
        UnityEngine.Rendering.VolumeProfile volumeProfile = GameObject.Find("Global Volume").GetComponent<UnityEngine.Rendering.Volume>()?.profile;
        if (!volumeProfile) throw new System.NullReferenceException(nameof(volumeProfile));

        volumeProfile.TryGet<ChromaticAberration>(out chrome);
        if(!volumeProfile.TryGet<ChromaticAberration>(out chrome)) throw new System.NullReferenceException(nameof(chrome));
        volumeProfile.TryGet<FilmGrain>(out film);
        if (!volumeProfile.TryGet<FilmGrain>(out film)) throw new System.NullReferenceException(nameof(film));
        volumeProfile.TryGet<LensDistortion>(out lens);
        if (!volumeProfile.TryGet<LensDistortion>(out lens)) throw new System.NullReferenceException(nameof(lens));
        volumeProfile.TryGet<SplitToning>(out split);
        if (!volumeProfile.TryGet<SplitToning>(out split)) throw new System.NullReferenceException(nameof(split));
        volumeProfile.TryGet<ColorAdjustments>(out adj);
        if (!volumeProfile.TryGet<ColorAdjustments>(out adj)) throw new System.NullReferenceException(nameof(adj));
        volumeProfile.TryGet<Tonemapping>(out tone);
        if (!volumeProfile.TryGet<Tonemapping>(out tone)) throw new System.NullReferenceException(nameof(tone));
        //volumeProfile.TryGet<Vignette>(out vignette);
        //if (!volumeProfile.TryGet<Vignette>(out vignette)) throw new System.NullReferenceException(nameof(Vignette));

        //setupAudio();

        resetVariables();
        Cursor.lockState  = CursorLockMode.Locked;
    }

    void StartDarkness()
    {
        //would essentially begin the isTimerRunning process
        isTimerRunning = true;

        //the music gets crossfaded rather than played one at a time
        crossfade.newSoundtrack(audioClips[1]);
    }    

    // Update is called once per frame
    void LateUpdate()
    {
        playerLife = cam.fieldOfView;

        //start darkness once player's -x position is smaller than [insert amount]
        if (player.position.x < -68f && !hasDarknessStarted)
        {            
            StartDarkness();
            hasDarknessStarted = true;
        }


        if (isTimerRunning) //makes sure we dont run "timer ran out" case infinitely and only once
        {

            if (cam.fieldOfView > (finalFOV+5)) //if the field of view is bigger than this, darkness grows
            {
                //here we reduce the camera distance from the player              
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, finalFOV, cameraZoomSpeed * speedModifier * Time.deltaTime);


                //change the mood when the field of view is halfway completed its trajectory
                //only runs once
                if (cam.fieldOfView <= 45f && !didWeChangeMood)
                {
                    crossfade.newSoundtrack(audioClips[2]);

                    ChangeSkybox();

                    //SlowerPhysicsMaterial();

                    //speedModifier = 2.0f; //slower


                    split.active = true; //show split toning
                    tone.mode.Override(TonemappingMode.ACES);
                    //sunlight.intensity = 0.1f;
                    RenderSettings.fogColor = new Color(119.0f / 255.0f, 135.0f / 255.0f, 164.0f / 255.0f);

                    didWeChangeMood = true;

                    SpawnSpiders(1);//20

                    string displayThis = "I should use my lamp since it's dark.";
                    StartCoroutine(display.DisplayThisText(displayThis));
                }/*
                else if (cam.fieldOfView / initialFOV <= 0.3 && didWeChangeMood) {
                    water.SetFloat("_Smoothness", 0.0f);
                }*/
                else if (didWeChangeMood) //changed mood true and want to repeatedly call stuff
                {
                    water.color = Color.Lerp(water.color, Color.black, cameraZoomSpeed * 2 * Time.deltaTime);
                    
                    
                    //lerp post-p 
                    LerpPostProcessing();
                }

                //lerp the fog
                if (cam.fieldOfView > 45)
                {
                    RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, new Color(2.0f / 255.0f, 19.0f / 255.0f, 53.0f / 255.0f), cameraZoomSpeed * 2f * Time.deltaTime); //new Color(41.0f / 255.0f, 17.0f / 255.0f, 16.0f / 255.0f)
                }
                else
                {
                    RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, Color.black, cameraZoomSpeed * 0.5f * Time.deltaTime);
                }

                RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, 0.03f, cameraZoomSpeed * 0.5f * Time.deltaTime);

                //Debug.Log(cam.fieldOfView);

            }
            else //darkness has consumed the player
            {
                Debug.Log("Darkness consumed!");
                StartCoroutine(processDarkEnding());
                //isTimerRunning = true;
            }

        }

        //Lerps back all the things that were lerped when we get light
        if (!isTimerRunning && hasDarknessStarted) 
        {
            //move camera back and increase life bar
            //also lerp the effects back
                        
            //keep lerping while the field of view is still smaller than the result
            if (cam.fieldOfView < (resultFOV - 8f)) //8f is for lerp adjustment
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, resultFOV, cameraZoomSpeed * speedModifier * 5 * Time.deltaTime);
                playerLife = cam.fieldOfView;
                //Debug.Log("lerp cam: " + cam.fieldOfView + " result: " + resultFOV);


                water.color = Color.Lerp(water.color, new Color(0.0f / 255.0f, 79.0f / 255.0f, 190.0f / 255.0f), cameraZoomSpeed * speedModifier * 2 * Time.deltaTime); //reset it to blue

                //lerp the fog

                RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, new Color(146.0f / 255.0f, 168.0f / 255.0f, 206.0f / 255.0f), cameraZoomSpeed *  speedModifier * 2f * Time.deltaTime);

                RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, 0.001f, cameraZoomSpeed * speedModifier * 2 * Time.deltaTime);

                //lerp post-p 
                LerpPostProcessingBackwards();

                if(cam.fieldOfView > 40)
                {
                    DespawnSpiders();
                }
            }
            else
            {
                //now that the life is back, darkness returns
                StartCoroutine(darknessBack());
            }
        }        

        //allows for camera rotation with mouse on X axis
        offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * cameraRotationSpeed, Vector3.up) * offset;
        transform.position = player.position + offset;
        transform.LookAt(player.position + new Vector3(0, 0.9f, 0)); //we add y height for the face distance offset

        

        if(Input.GetKeyDown(KeyCode.Tab)) //TODO: DELETE THIS, ONLY USEFUL FOR DEBUGGING
        {
            Cursor.lockState = CursorLockMode.None;
        }

        //-----------------------------------------------
        debugText.text = "FOV: " + cam.fieldOfView
            +"\nResultFOV: "+resultFOV;
    }

    IEnumerator darknessBack()
    {
        if(isCrankCalled)
        {
            yield return new WaitForSeconds(0.2f); //wait before making the darkness return
        }
        else
        {
            yield return new WaitForSeconds(1.5f); //wait before making the darkness return
        }        
        //Debug.Log("darkness returns");
        isTimerRunning = true;
        isCrankCalled = false;
    }
    
    //gets called when player triggers buoy
    public void getLightLife(GameObject light)
    {
        isCrankCalled = false; //the buoy calls the light

        //kill the light
        GameObject.Destroy(light);

        if (!hasDarknessStarted)
            return;

        //Debug.Log("get light");
        resultFOV = amountFOV + cam.fieldOfView;
        isTimerRunning = false;

        //Debug.Log("buoying light to: " + resultFOV);
        //add life to the player
        //expand field of view until we reach currentFOV + amount


        //Lerp back all the things that were lerped
        //StartCoroutine(LerpLife(resultFOV));

        //Change back all the things if we went past the mid-point
        DarknessRemovalCheck();


        //now that the life is back, darkness returns
        

        //and keep decreasing FOV when done (isTimerTunning = true) 

    }

    

    public void CrankLife(float incrementAmount) //earn a small bit of life from cranking once, making the player press a lot to earn more
    {
        isCrankCalled = true; //the crank calls the light

        if (!hasDarknessStarted)
            return;

        //Debug.Log("get light");
        resultFOV = incrementAmount + cam.fieldOfView;
        isTimerRunning = false;
        //Debug.Log("cranking to: "+resultFOV);
        //add life to the player
        //expand field of view until we reach currentFOV + amount


        //Change back all the things if we went past the mid-point

        DarknessRemovalCheck();


        //now that the life is back, darkness returns


        //and keep decreasing FOV when done (isTimerTunning = true) 

    }

    private void DarknessRemovalCheck()
    {
        /* //commented out to keep nightime after first time under 45FOV
        if (resultFOV >= 45)
        {
            //ChangeSkybox
            RenderSettings.skybox = ogSkybox;

            //OgPhysicsMaterial
            //Collider physmaterialCollider = GameObject.Find("SeaFloor").GetComponent<Collider>();
            //physmaterialCollider.material = ogPhysicMaterial;

            //speedModifier = 5.0f; //faster
            sunlight.intensity += 0.5f;
            split.active = false; //hide split toning
            tone.mode.Override(TonemappingMode.Neutral);

            didWeChangeMood = false;
            crossfade.newSoundtrack(audioClips[1]);
        }
        else
        {*/
            crossfade.newSoundtrack(audioClips[1]);
        //}
    }


    private void SpawnSpiders(int amount)
    {
        GameObject mimicsParent = GameObject.Find("Mimics");
        float minDistance = 60f;
        float maxDistance = 80f;
        

        for (int i = 0; i < amount; i++)
        {
            float randomRange = Random.Range(minDistance, maxDistance);
            Vector3 spawnPosition = new Vector3(this.transform.position.x + randomRange, this.transform.position.y + 6f, this.transform.position.z + randomRange);
            GameObject.Instantiate(MimicPrefab, spawnPosition, Quaternion.identity, mimicsParent.transform);
            Debug.LogWarning("spawned spider");
        }
    }

    private void SpawnSpidersCircle(int numberOfSpiders, float radius)
    {
        float angle = 2 * Mathf.PI / numberOfSpiders;
        float spawnRadius = radius;

        for (int i = 0; i < numberOfSpiders; i++)
        {
            // Get the x and y coordinates of a point on the circle with a given angle and radius
            float x = Mathf.Cos(angle * i) * spawnRadius;
            float y = Mathf.Sin(angle * i) * spawnRadius;

            // Add these coordinates to the player position to get the spawn position
            Vector3 spawnPos = player.position + new Vector3(x, y, 0);

            // Instantiate the enemy prefab at the spawn position
            GameObject enemy = Instantiate(MimicPrefab, spawnPos, Quaternion.identity);

            // Assign a tag or a name to the enemy
            enemy.tag = "Enemy";
        }
    }

    private void DespawnSpiders()
    {
        GameObject mimicsParent = GameObject.Find("Mimics");
        for(int i = 0; i < mimicsParent.transform.childCount; i++)
        {
            GameObject.Destroy(mimicsParent.transform.GetChild(i).gameObject);
            Debug.LogWarning("despawned spider");
        }
    }

    void ChangeSkybox()
    {
        RenderSettings.skybox = otherSkybox;        
        //Debug.Log("skybox");
    }

    void SlowerPhysicsMaterial()
    {
        Collider physmaterialCollider = GameObject.Find("SeaFloor").GetComponent<Collider>();
        physmaterialCollider.material = otherPhysMaterial;
        //Debug.Log("phys");
    }

    void LerpPostProcessing()
    {
        //Debug.Log("post p");
        chrome.intensity.value = Mathf.Lerp(chrome.intensity.value, 1.0f, cameraZoomSpeed * Time.deltaTime);
        film.intensity.value = Mathf.Lerp(film.intensity.value, 1.0f, cameraZoomSpeed * 2f * Time.deltaTime);
        lens.intensity.value = Mathf.Lerp(lens.intensity.value, 0.5f, cameraZoomSpeed * Time.deltaTime);
        adj.contrast.value = Mathf.Lerp(adj.contrast.value, 100.0f, cameraZoomSpeed * Time.deltaTime);
        adj.saturation.value = Mathf.Lerp(adj.saturation.value, 100.0f, cameraZoomSpeed * Time.deltaTime);
        //vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, 0.6f, cameraZoomSpeed * Time.deltaTime); 
    }

    void LerpPostProcessingBackwards()
    {
        //Debug.Log("post p back");
        chrome.intensity.value = Mathf.Lerp(chrome.intensity.value, 0.0f, cameraZoomSpeed * Time.deltaTime);
        film.intensity.value = Mathf.Lerp(film.intensity.value, 0.0f, cameraZoomSpeed * 0.5f * Time.deltaTime);
        lens.intensity.value = Mathf.Lerp(lens.intensity.value, 0.0f, cameraZoomSpeed * Time.deltaTime);
        adj.contrast.value = Mathf.Lerp(adj.contrast.value, 0.0f, cameraZoomSpeed * Time.deltaTime);
        adj.saturation.value = Mathf.Lerp(adj.saturation.value, 0.0f, cameraZoomSpeed * Time.deltaTime);
        //vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, 0.0f, cameraZoomSpeed * Time.deltaTime);
    }

    public IEnumerator processDarkEnding()
    {
        yield return new WaitForSeconds(1);

        //Debug.Log("1 sec passed");
        water.SetFloat("_Smoothness", Mathf.Lerp(water.GetFloat("_Smoothness"), 0.0f, 1.0f));
        film.intensity.value = 1.0f;

        RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, Color.black, 1.0f);
        RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, 0.09f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, 0.09f, 1.0f);
        //vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, 0.6f, cameraZoomSpeed * Time.deltaTime);

        

        yield return new WaitForSeconds(2);

        // TODO: show a succumbing to darkness UI message or something in here

        yield return new WaitForSecondsRealtime(2);

        ExitGame();
    }

    public IEnumerator processConsumedEnding()
    {
        //Debug.Log("1 sec passed");
        water.SetFloat("_Smoothness", Mathf.Lerp(water.GetFloat("_Smoothness"), 0.0f, 1.0f));
        film.intensity.value = 1.0f;

        RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, Color.black, 1.0f);
        RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, 0.09f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, 0.09f, 1.0f);
        //vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, 0.6f, cameraZoomSpeed * Time.deltaTime);

        yield return new WaitForSeconds(1);

        //spawn spiders around player in a circle and swarm player

        SpawnSpidersCircle(20, 5f);

        yield return new WaitForSecondsRealtime(2);

        QuitGame();
    }

    //todo: 
    // check volume sounds ? audio manager in global settings

    
    /*
    void setupAudio()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClips[0];
        //StartCoroutine(playAudioSequentially());
        
        countDownTime = 0;
        foreach (AudioClip clip in audioClips)
        {
            countDownTime += clip.length;
        }
        Debug.Log("length of music: " + countDownTime);
        //isTimerRunning = true;
        startTime = countDownTime;
    }
    */
    public float GetFOV()
    {
        return cam.fieldOfView;
    }
    public bool GetHasDarknessStarted()
    {
        return hasDarknessStarted;
    }
    public float GetMAXFOV()
    {
        return initialFOV;
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("Menu");
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
