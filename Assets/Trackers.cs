using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trackers : MonoBehaviour
{

    public GameObject wTracker1;
    public GameObject wTracker2;
    public GameObject wTracker3;
    public GameObject bTracker1;
    public GameObject bTracker2;
    public GameObject bTracker3;
    public GameObject audSource1;
    public GameObject audSource2;
    public GameObject audSource3;


    public GameObject[] wTrackers;

    public GameObject[] bTrackers;

    public GameObject[] audSources;
    public GameLogic gameLogic;

    // Start is called before the first frame update
    void Start()
    {
        audSources = new GameObject[3];
        bTrackers = new GameObject[3];
        wTrackers = new GameObject[3];

        audSources[0] = audSource1;
        audSources[1] = audSource2;
        audSources[2] = audSource3;

        bTrackers[0] = bTracker1;
        bTrackers[1] = bTracker2;
        bTrackers[2] = bTracker3;

        wTrackers[0] = wTracker1;
        wTrackers[1] = wTracker2;
        wTrackers[2] = wTracker3;
    }

    private float remapRot(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        // Calculate the percentage of the value within the original range
        float percentage = (value - fromMin) / (fromMax - fromMin);

        // Map the percentage to the new range
        return Mathf.Lerp(toMin, toMax, percentage);
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < 3; i++){
            float soundRot = gameLogic.getRotationSoundSource(audSources[i]);
            soundRot = remapRot(soundRot, -180f, 180f, -50f, 50f);

            Vector3 currentPosition = bTrackers[i].transform.position;
            bTrackers[i].transform.localPosition = new Vector3(soundRot * -1f, currentPosition.y, currentPosition.z);

            soundRot = gameLogic.getRotationSoundSource(audSources[i]);
            soundRot = remapRot(soundRot, -180f, 180f, 0f, 360f);

            wTrackers[i].transform.rotation = Quaternion.Euler(0f, 0f, soundRot + 180f);
        }

        if (Input.GetMouseButtonDown(0)) // Check for left mouse button click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            //Debug.Log("Mouse clicked on " );

            for(int i = 0; i < bTrackers.Length; i++)
            {
                if (bTrackers[i] != null && bTrackers[i].activeInHierarchy) // Check if the object exists and is active
                {
                    if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == bTrackers[i])
                    {
                        
                        Color spriteColor = bTrackers[i].GetComponent<SpriteRenderer>().color;
                        Color spriteWColor = wTrackers[i].GetComponent<SpriteRenderer>().color;
                        if(spriteColor.a == 255f){
                            spriteColor.g = 0f;
                            spriteColor.b = 0f;

                            spriteWColor.g = 0f;
                            spriteWColor.b = 0f;
                        }
                        else{
                            spriteColor.a = 255f;

                            spriteWColor.a = 255f;
                        }
                        bTrackers[i].GetComponent<SpriteRenderer>().color = spriteColor;
                        wTrackers[i].GetComponent<SpriteRenderer>().color = spriteWColor;
                        Debug.Log("Mouse clicked on " + bTrackers[i].name);
                        //obj.SetActive(false);
                    }
                }
            }

            /*
            foreach (GameObject obj in bTrackers)
            {
                if (obj != null && obj.activeInHierarchy) // Check if the object exists and is active
                {
                    if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == obj)
                    {

                        Color spriteColor = obj.GetComponent<SpriteRenderer>().color;
                        if(spriteColor.a == 255f){
                            spriteColor.g = 0f;
                            spriteColor.b = 0f;
                        }
                        else{
                            spriteColor.a = 255f;
                        }
                        obj.GetComponent<SpriteRenderer>().color = spriteColor;
                        Debug.Log("Mouse clicked on " + obj.name);
                        //obj.SetActive(false);
                    }
                }
            }
            */
        }

    }
}
