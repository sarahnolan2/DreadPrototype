using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LighthouseTrigger : MonoBehaviour
{
    [SerializeField]
    CameraController gameController;

    [SerializeField] AudioSource ConsumedEndingSrc;
    private void OnColliderEnter(Collider collision)
    {
        StartCoroutine(gameController.processConsumedEnding());
        ConsumedEndingSrc.Play();
    }
}
