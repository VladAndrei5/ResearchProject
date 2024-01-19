using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float totalTime = 300f; // Total time in seconds (adjust as needed)
    private float currentTime;
    public Text timerText;

    void Start()
    {
        currentTime = totalTime;
    }

    void Update()
    {
        if (currentTime > 0)
        {
            if (currentTime < 10)
            {
                timerText.color = Color.red;
            }

            currentTime -= Time.deltaTime;

            // Calculate minutes, seconds, and milliseconds
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            int milliseconds = Mathf.FloorToInt((currentTime * 1000) % 1000);

            // Display the timer in mm:ss:msms format
            timerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        }
        else
        {
            // Timer has reached zero, you can handle the timer expiration logic here
            timerText.text = "00:00:000";
        }
    }
}
