using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuoyTrigger : MonoBehaviour
{
    [SerializeField]
    CameraController gameController;
    GameObject lightObj;
    float lightIntensity;

    private bool isTriggered;

    // Start is called before the first frame update
    void Start()
    {
        isTriggered = false;
        lightObj = this.transform.Find("Point Light").gameObject;
        lightIntensity = lightObj.GetComponent<Light>().intensity;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            isTriggered = true;
            GameObject lightObj = transform.Find("Point Light").gameObject;

            if ( lightObj != null ) //get the light from the buoy
            {
                gameController.getLightLife(lightObj);
            }
            else
            {
                //light is already out
            }
        }
    }
}
