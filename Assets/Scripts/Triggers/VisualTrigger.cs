using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualTrigger : MonoBehaviour
{
    bool isPlayerInRange; //keep track of whether the player is in range

    [SerializeField]
    private GameObject visualCue; //holds all of the visual cue content

    //for crates
    public bool isCrate;
    
    CrankController crankController;
    public CrateTrigger crateTrigger;

    void Awake()
    {
        isPlayerInRange = false;
    }

    private void Start()
    {
        visualCue.SetActive(false);

        if (crateTrigger != null && isCrate)
            crankController = crateTrigger.crankController;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isPlayerInRange = true;
            //Debug.Log("IN RANGE OF "+transform.parent.gameObject.name);

            if (visualCue != null)
            {
                //if this is a crate
                if (isCrate && !crankController.GetHasCrank())
                {
                    //Debug.Log("hasCrank: " + crankController.GetHasCrank());
                    //show visual cue if player is in range
                    visualCue.SetActive(true);
                    //Debug.Log("VISUAL CUE IS ON");
                }
                //if this is a buoy
                else if (!isCrate)
                {
                    //show visual cue if player is in range
                    visualCue.SetActive(true);
                    //Debug.Log("VISUAL CUE IS ON");
                }                
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isPlayerInRange = false;
            //Debug.Log("LEFT RANGE OF "+ transform.parent.gameObject.name);

            if (visualCue != null)
            {
                //if this is a crate
                if (isCrate && !crankController.GetHasCrank())
                {
                    //Debug.Log("hasCrank: " + crankController.GetHasCrank());
                    //show visual cue if player is not in range
                    visualCue.SetActive(false);
                    //Debug.Log("VISUAL CUE IS OFF");
                }
                //if this is a buoy
                else if(!isCrate)
                {
                //show visual cue if player is not in range
                visualCue.SetActive(false);
                    //Debug.Log("VISUAL CUE IS OFF");
                }
            }
        }
    }
}
