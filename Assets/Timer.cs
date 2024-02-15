using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float totalTime = 300f; // Total time in seconds (adjust as needed)
    private float currentTime;
    public Text timerText;

    public Light spotlight1;
    public Light spotlight2;

    void Start()
    {
        currentTime = totalTime;
    }

    void Update()
    {
        if (currentTime > 0)
        {
            if (currentTime < 15)
            {
                timerText.color = Color.red;
                float rotationAmount = 300f * Time.deltaTime;

                // Create a rotation increment based on the calculated rotation amount
                Quaternion rotationIncrement = Quaternion.Euler(0f, rotationAmount, 0f);

                // Apply the rotation increment to the current rotation
                spotlight1.transform.rotation *= rotationIncrement;
                spotlight2.transform.rotation *= rotationIncrement;
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
