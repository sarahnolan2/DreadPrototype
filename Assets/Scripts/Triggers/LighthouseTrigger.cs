using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LighthouseTrigger : MonoBehaviour
{
    [SerializeField]
    CameraController gameController;

    [SerializeField] AudioSource ConsumedEndingSrc;
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(gameController.processConsumedEnding());
            ConsumedEndingSrc.Play();
        }        
    }
}
