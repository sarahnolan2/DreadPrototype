using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    GameObject player;

    [Range(1,30)]
    public float speed = 5f; // The speed of the enemy
    public float minDist = 10f; // The minimum distance from the player
    public float maxDist = 2f; // The maximum distance from the player

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //this.GetComponent<Transform>().LookAt(player.transform);

        //this.transform.Translate(new Vector3(0,0,0.01f),Space.Self);


        // Get the distance between the enemy and the player
        float dist = Vector3.Distance(transform.position, player.transform.position);

        // If the distance is greater than or equal to the minimum distance
        if (dist >= minDist)
        {
            // Move the enemy towards the player at a given speed
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }

        // If the distance is less than or equal to the maximum distance
        if (dist <= maxDist)
        {
            // Do something else, such as attack or stop
            Debug.Log("Enemy reached player");
        }

        // Make the enemy look towards the direction of the player
        transform.LookAt(player.transform);
    }
}
