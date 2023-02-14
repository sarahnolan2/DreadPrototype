using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(AudioSource))]
public class CameraController : MonoBehaviour
{
    
    public Transform player;

    private float countDownTime;

    //how far the camera is from the player at the start
    public float cameraInitOffsetY = 4.0f;
    public float cameraInitOffsetZ = 15.0f;

    public float cameraRotationSpeed = 4.0f;
    public float cameraZoomSpeed; //0.01 is a good speed?
    public float finalFOV; //final distance from player

    public Material otherSkybox;
    public PhysicMaterial otherPhysMaterial;
    public Material water;

    private Vector3 offset;
    private Camera cam;
    private float initialFOV;

    private bool isTimerRunning;
    private float startTime;
    private bool didWeChangeMood;
    private float speedModifier;

    private ChromaticAberration chrome;
    private LensDistortion lens;
    private FilmGrain film;
    private ColorAdjustments adj;
    private SplitToning split;
    private Tonemapping tone;
    
    private int flip = 0;
    private AudioSource audioSource = new AudioSource();
    public AudioClip[] audioClips = new AudioClip[2];

    private float endTimerStart;

    // Start is called before the first frame update
    void Start()
    {
        // camera position
        cam = GetComponent<Camera>();
        offset = new Vector3(player.position.x + cam.transform.position.x, player.position.y + cam.transform.position.y, player.position.z + cam.transform.position.z); //(player.position.x, player.position.y + 8.0f, player.position.z + 7.0f);
        //offset = new Vector3(player.position.x, player.position.y + cameraInitOffsetY, player.position.z + cameraInitOffsetZ); //(player.position.x, player.position.y + 8.0f, player.position.z + 7.0f);
        
        didWeChangeMood = false;
        initialFOV = cam.fieldOfView;
        //isCameraRotationFree = true; //can use this to switch from free roaming camera to the single axis camera.
        speedModifier = 2.5f; //faster
              
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

        setupAudio();

        resetVariables();
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimerRunning) //makes sure we dont run "timer ran out" case infinitely and only once
        {

            if (countDownTime > 0)
            {
                //here we reduce the camera distance from the player until the timer runs out                
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, finalFOV, cameraZoomSpeed * speedModifier * Time.deltaTime);


                //change the mood when the field of view is halfway completed its trajectory
                //only runs once
                if (cam.fieldOfView / initialFOV <= 0.5 && !didWeChangeMood)
                {
                    ChangeSkybox();

                    SlowerPhysicsMaterial();

                    speedModifier = 2.0f; //slower


                    split.active = true; //show split toning
                    tone.mode.Override(TonemappingMode.ACES);                    

                    didWeChangeMood = true;
                    
                }/*
                else if (cam.fieldOfView / initialFOV <= 0.3 && didWeChangeMood) {
                    water.SetFloat("_Smoothness", 0.0f);
                }*/
                else if (didWeChangeMood) //changed mood true and want to repeatedly call stuff
                {
                    water.color = Color.Lerp(water.color, Color.black, cameraZoomSpeed * 2 * Time.deltaTime);

                    //lerp the fog
                    if (cam.fieldOfView / initialFOV > 0.25)
                    {
                        RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, new Color(41.0f / 255.0f, 17.0f / 255.0f, 16.0f / 255.0f), cameraZoomSpeed * 0.5f * Time.deltaTime); //new Color(41.0f / 255.0f, 17.0f / 255.0f, 16.0f / 255.0f)
                    }
                    else
                    {
                        RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, Color.black, cameraZoomSpeed * 1.0f * Time.deltaTime);
                    }
                    
                    RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, 0.05f, cameraZoomSpeed * 0.5f  * Time.deltaTime);

                    //lerp post-p 
                    LerpPostProcessing();
                }
                //Debug.Log(cam.fieldOfView);

                countDownTime -= Time.deltaTime;
            }
            else
            {
                Debug.Log("Time has run out!");
                countDownTime = 0;
                StartCoroutine(processEnding());
                isTimerRunning = false;
            }

            //allows for camera rotation with mouse on X axis
            offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * cameraRotationSpeed, Vector3.up) * offset;
            transform.position = player.position + offset; 
            transform.LookAt(player.position + new Vector3(0, 0.9f,0)); //we add y height for the face distance offset

        }
        
    }

    IEnumerator playAudioSequentially()
    {
        //audio
        yield return null;

        //1.Loop through each AudioClip
        for (int i = 0; i < audioClips.Length; i++)
        {
            //2.Assign current AudioClip to audiosource
            audioSource.clip = audioClips[i];

            //3.Play Audio
            audioSource.Play();

            //4.Wait for it to finish playing
            while (audioSource.isPlaying)
            {
                yield return null;
            }

            //5. Go back to #2 and play the next audio in the audiodClips array
        }
    }

    
    void ChangeSkybox()
    {
        RenderSettings.skybox = otherSkybox;        
        Debug.Log("skybox");
    }

    void SlowerPhysicsMaterial()
    {
        Collider physmaterialCollider = GameObject.Find("SeaFloor").GetComponent<Collider>();
        physmaterialCollider.material = otherPhysMaterial;
        Debug.Log("phys");
    }

    void LerpPostProcessing()
    {
        //Debug.Log("post p");
        chrome.intensity.value = Mathf.Lerp(chrome.intensity.value, 1.0f, cameraZoomSpeed * Time.deltaTime);
        film.intensity.value = Mathf.Lerp(film.intensity.value, 1.0f, cameraZoomSpeed * 0.5f * Time.deltaTime);
        lens.intensity.value = Mathf.Lerp(lens.intensity.value, 0.5f, cameraZoomSpeed * Time.deltaTime);
        adj.contrast.value = Mathf.Lerp(adj.contrast.value, 100.0f, cameraZoomSpeed * Time.deltaTime);
        adj.saturation.value = Mathf.Lerp(adj.saturation.value, 100.0f, cameraZoomSpeed * Time.deltaTime);
    }

    IEnumerator processEnding()
    {
        yield return new WaitForSeconds(1);

        Debug.Log("1 sec passed");
        water.SetFloat("_Smoothness", Mathf.Lerp(water.GetFloat("_Smoothness"), 0.0f, 1.0f));
        film.intensity.value = 1.0f;

        RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, Color.black, 1.0f);
        RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, 0.09f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, 0.09f, 1.0f);

        yield return new WaitForSeconds(2);


        QuitGame();
    }

    //todo: 
    // sun rotation to lower - but also in front of player's face?
    //adjust camera zoom speed
    // check volume sounds ?

    void resetVariables()
    {
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
    }

    void setupAudio()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClips[0];
        StartCoroutine(playAudioSequentially());

        countDownTime = 0;
        foreach (AudioClip clip in audioClips)
        {
            countDownTime += clip.length;
        }
        Debug.Log("length of music: " + countDownTime);
        isTimerRunning = true;
        startTime = countDownTime;
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
