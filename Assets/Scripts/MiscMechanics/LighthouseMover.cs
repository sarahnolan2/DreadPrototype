using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LighthouseMover : MonoBehaviour
{
    
    //The position calculated from the masterObject
    private Vector3 relativePosition;
    //What position do we want the lighthouseObject to have in local space?
    private Vector3 wantedPosition = new Vector3(-410f, 0f, 0f);
    //The lighthouseObject(child)
    public Transform lighthouseObject;
    //The playerObject(parent)
    public Transform playerObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Calculate the position
        //What this does:
        //Take the parent object and use the local space "wantedPosition" to
        //calculate a world space position. Assign this to "relative position"

        relativePosition = new Vector3(playerObject.TransformPoint(wantedPosition).x,0f,0f);

        //Set the position of the child object to this relative position
        lighthouseObject.position = new Vector3(Mathf.Lerp(lighthouseObject.position.x, relativePosition.x, Time.deltaTime),0f,0f);
    }

}
