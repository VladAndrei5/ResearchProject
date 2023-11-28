using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingSmth : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localPosition = transform.localPosition + new Vector3(0, 20 , 0);
    }
}
