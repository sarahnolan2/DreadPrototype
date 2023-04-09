using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateTrigger : MonoBehaviour
{

    [SerializeField]
    CrankController crankController;

    


    // Start is called before the first frame update
    void Start()
    {
        
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
            }
            else
            {
                // "I already have a crank."
            }
        }
    }
}
