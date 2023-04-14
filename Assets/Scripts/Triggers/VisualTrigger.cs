using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualTrigger : MonoBehaviour
{
    bool isPlayerInRange; //keep track of whether the player is in range

    [SerializeField]
    private GameObject visualCue; //holds all of the visual cue content

    public bool isPickedUp;
    void Awake()
    {
        isPickedUp = false;
        isPlayerInRange = false;
    }

    private void Start()
    {
        visualCue.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (visualCue != null)
        {

            if (isPlayerInRange && !isPickedUp)
            {
                //show visual cue if player is in range
                visualCue.SetActive(true);
                
            }
            else
            {
                //don't show visual cue if player is not in range
                visualCue.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !isPickedUp)
        {
            isPlayerInRange = true;
            //Debug.Log("IN RANGE");

            if (visualCue != null)
            {
                //show visual cue if player is in range
                visualCue.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && !isPickedUp)
        {
            isPlayerInRange = false;
            //Debug.Log("OUT OF RANGE");

            if (visualCue != null)
            {
                //don't show visual cue if player is not in range
                visualCue.SetActive(false);
            }
        }
    }
}
