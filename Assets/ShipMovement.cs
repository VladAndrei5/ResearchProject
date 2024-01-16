using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public Vector3 destination;
    public float speed = 15;
    public float arriveCheck = 0.1f;
    public float distance;
    void Start()
    {
        destination = new Vector3(Random.Range(-50f, 50.0f) , Random.Range(-50f, 50.0f), 0 );
        distance = Vector3.Distance(transform.position, destination);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        distance = Vector3.Distance(transform.position, destination);
        if(distance < arriveCheck){
            destination = new Vector3(Random.Range(-50f, 50.0f) , Random.Range(-50f, 50.0f), 0 );
            distance = Vector3.Distance(transform.position, destination);
        }
    }
}
