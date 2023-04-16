using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateTrigger : MonoBehaviour
{

    [SerializeField]
    public CrankController crankController;

    GameObject crateVisualTrigger;


    // Start is called before the first frame update
    void Start()
    {
        crateVisualTrigger = transform.Find("VisualTrigger").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(!crankController.GetHasCrank()) //if player doesn't currently have a crank
            {
                crankController.EarnCrank(this.gameObject);
                GameObject.Destroy(crateVisualTrigger);
            }
            else
            {
                // "I already have a crank."
            }
        }
    }
}
