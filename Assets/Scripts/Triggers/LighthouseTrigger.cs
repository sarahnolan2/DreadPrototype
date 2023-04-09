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
        StartCoroutine(gameController.processDarkEnding());
        ConsumedEndingSrc.Play();
    }
}
