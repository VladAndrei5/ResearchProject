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

    }
}
