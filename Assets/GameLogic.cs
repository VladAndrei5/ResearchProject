using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Slider1;
    public GameObject Slider2;
    public float lengthSensor;
    public float speedOfSound;
    public float beamRotation;
    private float minDecebels = -144f;
    private float maxDecebels = 0f;
    private int minFreq = 0;
    private int maxFreq = 20000;

    public float getFrequencyBin(int arrayLength, int currentPos)
    {
        return ((maxFreq / arrayLength) * (currentPos + 1));
    }

    //returns rot angle in degrees
    public float getRotationSoundSource(GameObject soundSource)
    {
        float angle;
        angle = 0f;
        float x = soundSource.transform.localPosition.x;
        float y = soundSource.transform.localPosition.y;
        Vector2 targerDir = new Vector2(x, y);

        if (x == 0)
        {
            angle = 0f;
        }
        else if (x < 0)
        {
            angle = Vector2.Angle(targerDir, new Vector2(0, 1));
        }
        else
        {
            angle = -1f * Vector2.Angle(targerDir, new Vector2(0, 1));
        }

        if (y < 0 && angle == 0)
        {
            angle = 180;
        }

        return angle;
    }

    public float convertToDecebels(float number)
    {

        float dB;
        if (number != 0)
            dB = 20.0f * Mathf.Log10(number);
        else
            dB = minDecebels;

        return dB;
    }

    public float normaliseSoundDecebels( float value)
    {
        float result = Mathf.InverseLerp(minDecebels, maxDecebels, value);
        return result;
    }

    public float getBeamRot()
    {
        return Math.Abs(beamRotation % 360);
    }

    public float getBeamWidth(float frequency){
        return speedOfSound / (frequency * lengthSensor);
    }

    public float getSoundIntensity(float theta, float frequency){
        return (float) Math.Exp( (-1 * Math.Pow(theta, 2) ) / (2 * Math.Pow( (speedOfSound / (frequency * lengthSensor)) , 2) ) );
    }

    void Start(){

    }

    // Update is called once per frame
    void Update()
    {

    }
}
