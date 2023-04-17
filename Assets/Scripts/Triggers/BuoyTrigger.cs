using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuoyTrigger : MonoBehaviour
{
    [SerializeField]
    CameraController gameController;
    GameObject lightObj;
    //float lightIntensity;

    private AudioSource buoyAudioSource;

    GameObject buoyVisualTrigger;

    //private bool isTriggered;

    // Start is called before the first frame update
    void Start()
    {
        //isTriggered = false;
        lightObj = this.transform.Find("Point Light").gameObject;
        //lightIntensity = lightObj.GetComponent<Light>().intensity;
        buoyVisualTrigger = transform.Find("VisualTrigger").gameObject;
        buoyAudioSource = this.GetComponent<AudioSource>();
        buoyAudioSource.Play();
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            //isTriggered = true;
            //GameObject lightObj = transform.Find("Point Light").gameObject;

            if ( lightObj != null ) //get the light from the buoy
            {
                buoyAudioSource.loop = false;
                buoyAudioSource.volume = 0f;
                gameController.getLightLife(lightObj);
                GameObject.Destroy(buoyVisualTrigger); //remove the highlight object since we picked up the buoy's light
                
            }
            else
            {
                //light is already out
            }
        }
    }
}
