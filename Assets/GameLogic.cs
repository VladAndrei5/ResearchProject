using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider slider1;
    public Slider slider2;
    public Slider sliderBeamRot;
    public Slider sliderSensor;
    public float lengthSensor;
    public float speedOfSound;
    public float beamRotation;
    public float beamW;
    private float minDecebels = -144f;
    private float maxDecebels = 0f;
    private int minFreq = 0;
    private int maxFreq = 20000;


    public int getSliderPos1(int arrayLength)
    {
        return (int)((slider1.value / 100) * arrayLength);
    }
    public int getSliderPos2(int arrayLength)
    {
        return (int)((slider2.value / 100) * arrayLength);
    }

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
        beamW = speedOfSound / (frequency * lengthSensor);
        return speedOfSound / (frequency * lengthSensor);
    }

    public float getLineThickness(float f)
    {
        return ( (100 - slider1.value) / 100) * getBeamWidth(f);
    }

    public float getSoundIntensity(float theta, float frequency){
        return (float) Math.Exp( (-1 * Math.Pow(theta, 2) ) / (2 * Math.Pow( (speedOfSound / (frequency * lengthSensor)) , 2) ) );
    }

    void Start(){
        beamW = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (sliderBeamRot.value > 180)
        {
            sliderBeamRot.value = 0;
        }
        else if (sliderBeamRot.value < -180)
        {
            sliderBeamRot.value = 180;
        }

        if(sliderBeamRot.value < 0)
        {
            beamRotation = sliderBeamRot.value * -1;
        }
        else
        {
            beamRotation = sliderBeamRot.value;
        }

        lengthSensor = sliderSensor.value;
    }
}
