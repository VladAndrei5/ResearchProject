using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trackers : MonoBehaviour
{
    public GameObject[] wTrackers;

    public GameObject[] bTrackers;

    private GameObject[] audSources;
    public GameLogic gameLogic;

    // Start is called before the first frame update
    void Start()
    {
        audSources = gameLogic.soundSourcesArr;
        GameObject[] foundObjects = GameObject.FindGameObjectsWithTag("WTracker");
        wTrackers = new GameObject[foundObjects.Length];
        for (int i = 0; i < foundObjects.Length; i++){
            wTrackers[i] = foundObjects[i];
        }
        foundObjects = GameObject.FindGameObjectsWithTag("BTracker");
        bTrackers = new GameObject[foundObjects.Length];
        for (int i = 0; i < foundObjects.Length; i++){
            bTrackers[i] = foundObjects[i];
        }
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
        for(int i = 0; i < wTrackers.Length; i++){
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
                    }
                }
            }
        }

    }
}
