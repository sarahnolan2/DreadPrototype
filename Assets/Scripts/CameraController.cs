using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(AudioSource))]
public class CameraController : MonoBehaviour
{
    
    public Transform player;

    public float countDownTime;

    //how far the camera is from the player at the start
    public float cameraInitOffsetY = 4.0f;
    public float cameraInitOffsetZ = 15.0f;

    public float cameraRotationSpeed = 4.0f;
    public float cameraZoomSpeed; //0.01 is a good speed?
    public float finalFOV; //final distance from player

    public Material otherSkybox;
    public PhysicMaterial otherPhysMaterial;
    public Volume volume;
    public Material water;

    private Vector3 offset;
    private Camera cam;
    private float initialFOV;

    private bool isTimerRunning;
    private float startTime;
    private bool didWeChangeMood;
    private int speedModifier;

    private ChromaticAberration chrome;
    private LensDistortion lens;
    private FilmGrain film;
    
    private int flip = 0;
    private AudioSource audioSource = new AudioSource();
    public AudioClip[] audioClips = new AudioClip[2];

    // Start is called before the first frame update
    void Start()
    {
        offset = new Vector3(player.position.x, player.position.y + cameraInitOffsetY, player.position.z + cameraInitOffsetZ); //(player.position.x, player.position.y + 8.0f, player.position.z + 7.0f);
        cam = GetComponent<Camera>();
        isTimerRunning = true;
        startTime = countDownTime;
        didWeChangeMood = false;
        initialFOV = cam.fieldOfView;
        //isCameraRotationFree = true; //can use this to switch from free roaming camera to the single axis camera.
        speedModifier = 1;
        water.color = new Color(0.0f / 255.0f, 79.0f/255.0f, 190.0f/255.0f); //reset it to blue

        //post p
        volume.profile.TryGet<ChromaticAberration>(out chrome);
        volume.profile.TryGet<FilmGrain>(out film);

        //audio 
          
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClips[0];
        StartCoroutine(playAudioSequentially());
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

                //lerp the fog
                RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, new Color(41.0f / 255.0f, 17.0f / 255.0f, 16.0f / 255.0f), cameraZoomSpeed * speedModifier * Time.deltaTime);
                RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, 0.05f, cameraZoomSpeed * speedModifier * Time.deltaTime);

                //change the mood when the field of view is halfway completed its trajectory
                if (cam.fieldOfView / initialFOV <= 0.5 && !didWeChangeMood)
                {

                    ChangeSkybox();

                    SlowerPhysicsMaterial();

                    speedModifier = 2;

                    didWeChangeMood = true;
                }
                else if (didWeChangeMood) //changed mood true and want to repeatedly call stuff
                {
                    water.color = Color.Lerp(water.color, Color.black, cameraZoomSpeed * speedModifier * Time.deltaTime);

                    

                    //lerp post-p (doesn't seem to work at runtime)
                    //chrome.intensity.value = Mathf.Lerp(0, 1, cameraZoomSpeed * Time.deltaTime);
                    //film.intensity.value = Mathf.Lerp(0, 1, cameraZoomSpeed * Time.deltaTime);
                }
                //Debug.Log(cam.fieldOfView);

                countDownTime -= Time.deltaTime;
            }
            else
            {
                Debug.Log("Time has run out!");
                countDownTime = 0;
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

    private void OnAnimatorIK(int layerIndex)
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    void LateUpdate()
    {
        
        //Debug.Log(transform.position);
        /* //attempted to rotate camera in all directions but cant make it work right
        if (isCameraRotationFree)
        {
            //get free roam rotation for the camera
            Vector3 movementAmount = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0) * cameraRotationSpeed * Time.deltaTime;

            offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offset;
            transform.position = player.position + offset;
            transform.LookAt(player.position);

            offset = Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * turnSpeed, Vector3.right) * offset;
            transform.position = player.position + offset;
            transform.LookAt(player.position);

            //transform.Rotate(movementAmount);
            //transform.LookAt(player.position);
        }
        else */

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

    //todo: 
    // audio? - eerie ambience, ambience4, horror drone , das kabinet at 7min, 9min-10min
    // sun rotation to lower - but also in front of player's face?
    //remove cylinder & add different navigation points (far away things could be seen?), 
    //adjust camera zoom speed
    // check volume sounds ?

}
