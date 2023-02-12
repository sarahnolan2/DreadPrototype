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

    private Vector3 offset;
    private Camera cam;
    private float initialFOV;

    private bool isTimerRunning;
    private float startTime;
    private bool didWeChangeMood;
    

    private ChromaticAberration chrome;
    private LensDistortion lens;
    private FilmGrain film;
    
    private int flip = 0;
    private AudioSource audioSources = new AudioSource();
    public AudioClip[] otherclips = new AudioClip[2];
    private int audioIndex;

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
        
        //post p
        volume.profile.TryGet<ChromaticAberration>(out chrome);
        volume.profile.TryGet<FilmGrain>(out film);

        //audio 
          
        audioSources = GetComponent<AudioSource>();
        audioIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimerRunning) //makes sure we dont run "timer ran out" case infinitely and only once
        {
            if (countDownTime > 0)
            {
                //here we reduce the camera distance from the player until the timer runs out                
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, finalFOV, cameraZoomSpeed * Time.deltaTime);

                

                //change the mood when the field of view is halfway completed its trajectory
                if(cam.fieldOfView / initialFOV <= 0.5 && !didWeChangeMood)
                {
                    ChangeSkybox();

                    SlowerPhysicsMaterial();

                    didWeChangeMood = true;
                }
                else if (cam.fieldOfView / initialFOV <= 0.25)
                {

                }
                else if (cam.fieldOfView / initialFOV <= 0.10)
                {
                    
                }
                else if (didWeChangeMood) //changed mood true and want to repeatedly call stuff
                {
                    //lerp the fog
                    RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, new Color(41f/255f,17f/255f,16f/255f), cameraZoomSpeed * 2 * Time.deltaTime);
                    RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, 0.05f, cameraZoomSpeed * Time.deltaTime);

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



            //audio
            if(!audioSources.isPlaying)
            {
                audioSources.clip = otherclips[audioIndex];
                audioSources.Play();
                audioIndex += 1;
            }

            // audio help: https://docs.unity3d.com/ScriptReference/AudioSource.PlayScheduled.html
        }
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
