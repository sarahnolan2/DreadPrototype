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
            gameController.getLightLife(this.gameObject);

        }
    }
}
