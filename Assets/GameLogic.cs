using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Audio;

public class GameLogic : MonoBehaviour
{
    // Start is called before the first frame update

    
    public GameObject beamVector;
    public GameObject[] soundSourcesArr;

    public GameObject noise;


    public int numberOfBinsSpectrogram;
    public int numberOfBinsBearing;

    public Slider sliderMinFreqBound;
    public Slider sliderMaxFreqBound;
    public Slider sliderBeamRot;
    public Slider beamWidth;
    public float lengthSensor;
    public float speedOfSound;
    public float beamRotation;
    private float minDecebels = -144f;
    private float maxDecebels = 0f;
    public float[] beamWidthsArr;
    public float[] frequencyNumberArr;
    //private int minFreq = 0;
    private int maxFreq = 10000;

    public int divideFrequencyBins;

 
    //returns the area being scanneded in degrees
    public float getScanningWidth(){
        float newMin = 0;
        float newMax = 360;
        return (beamWidth.value * 2 * (newMax - newMin)) + newMin;
    }

    /*returns a position in the array
    proportional to the minimum frequency bound
    */
    public int getSliderPos1(int arrayLength)
    {
        return (int)(sliderMinFreqBound.value * arrayLength);
    }
    
    /*returns a position in the array
    proportional to the maximum frequency bound
    */
    public int getSliderPos2(int arrayLength)
    {
        return (int)(sliderMaxFreqBound.value * arrayLength);
    }

    public float[] createFreqNumArr(int arrayLength)
    {
        float[] arr = new float[arrayLength];
        float increment = maxFreq / arrayLength;
        float multiplier = 1;
        for(int i = 0; i < arrayLength; i++){
            arr[i] = increment * multiplier;
            multiplier++;
        }
        return arr;
    }

    public float getLowerBoundFreq(){
        return getFrequencyBin(frequencyNumberArr.Length, getSliderPos1(frequencyNumberArr.Length));
    }

       public float getHigherBoundFreq(){
        return getFrequencyBin(frequencyNumberArr.Length, getSliderPos2(frequencyNumberArr.Length));
    }


    /*for a given array position and array length
    it returns the frequency for said bin
    */
    public float getFrequencyBin(int arrayLength, int i)
    {
        return ((maxFreq / arrayLength) * (i + 1));
    }

    /*returns the angle of the object in relation to the vector facing North
    North directly above origin is 0 degrees, and South is 180 degrees.
    If the object is on the right side of the map, the angle will be given clockwise from 0 to -180
    If the object is on the left side of the map, the angle will be given anti-clockwise from 0 to +180
    */
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

    /*returns the magnitude of the angle between the vector pointing
     towards the soundsource and the beam itself in degrees
    */
    public float getRelativeRotationToBeam(GameObject soundSource)
    {
        float angle = getRotationSoundSource(soundSource);
        if(angle < 0){
            angle = (180 - ( -1 *angle)) + 180;
        }

        float result = Math.Abs(angle - getBeamRot());

        if(result <= 180){
            return result;
        }
        else{
            return Math.Abs(360 - result);
        }
    }

    /*converts amplitude stored in spectrum data to decebels
    */
    public float convertToDecebels(float number)
    {

        float dB;
        if (number != 0)
            dB = 20.0f * Mathf.Log10(number);
        else
            dB = minDecebels;

        return dB;
    }

    /*normalises a value given in decebels
    */
    public float normaliseSoundDecebels( float value)
    {
        float result = Mathf.InverseLerp(minDecebels, maxDecebels, value);
        return result;
    }

    /*returns the angle of the beam in relation to the vector facing North
    going anti-clockwise from 0 to 360
    */
    public float getBeamRot()
    {
        if(sliderBeamRot.value < 0)
        {
            beamRotation = sliderBeamRot.value * -1;
        }
        else
        {
            beamRotation = 360 - sliderBeamRot.value;
        }
      
        return Math.Abs(beamRotation % 360);
    }

    /*for a given frequency returns the beam width from which it can be heard
    in radians
    */
    public float getBeamWidthAtFreq(float frequency){
        return speedOfSound / (frequency * lengthSensor);
    }


    //returns the number of pixels that should be added to the bearing  waterfall
    public float getLineThickness(float f, float beamWidth)
    {
        return ( (100 - sliderMinFreqBound.value) / 100) * beamWidth;
    }

    public float getSoundIntensity(float theta, float frequency){
        return (float) Math.Exp( (-1 * Math.Pow(theta, 2) ) / (2 * Math.Pow( (speedOfSound / (frequency * lengthSensor)) , 2) ) );
    }

    public void SetCutoffValues(GameObject source, float lp, float hp)
    {
        AudioLowPassFilter lowPassFilter = source.GetComponent<AudioLowPassFilter>();
        AudioHighPassFilter highPassFilter = source.GetComponent<AudioHighPassFilter>();
        lowPassFilter.cutoffFrequency = lp;
        highPassFilter.cutoffFrequency = hp;

    }

    void Start(){
        GameObject[] foundObjects = GameObject.FindGameObjectsWithTag("AudioSourceObj");
        soundSourcesArr = new GameObject[foundObjects.Length];

        // Copy found objects to the gameObjectsArray
        for (int i = 0; i < foundObjects.Length; i++){
            soundSourcesArr[i] = foundObjects[i];
        }


        frequencyNumberArr = createFreqNumArr((int)(numberOfBinsSpectrogram / divideFrequencyBins));
        beamWidthsArr = new float[(int)(numberOfBinsSpectrogram / divideFrequencyBins)];

        for(int i = 0; i < frequencyNumberArr.Length; i++){
            beamWidthsArr[i] = getBeamWidthAtFreq(frequencyNumberArr[i]);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {


        float lowPassCutoffValue = 10000;
        float highPassCutoffValue = 10;
        
        for(int i = 0; i < soundSourcesArr.Length; i++){
            float rot = getRelativeRotationToBeam(soundSourcesArr[i]);
            highPassCutoffValue = 10000;
            lowPassCutoffValue = 10;

            for (int j = 0; j < beamWidthsArr.Length; j++){
                 if ((rot < (getScanningWidth() / 2) || (rot < (beamWidthsArr[j] * Mathf.Rad2Deg / 2)))){
                    highPassCutoffValue = getFrequencyBin(numberOfBinsSpectrogram / divideFrequencyBins, getSliderPos1(numberOfBinsSpectrogram / divideFrequencyBins));
                    lowPassCutoffValue = getFrequencyBin(numberOfBinsSpectrogram / divideFrequencyBins, getSliderPos2(numberOfBinsSpectrogram / divideFrequencyBins));
                }

            }
        
            SetCutoffValues(soundSourcesArr[i], lowPassCutoffValue, highPassCutoffValue);
        }
        highPassCutoffValue = getFrequencyBin(numberOfBinsSpectrogram / divideFrequencyBins, getSliderPos1(numberOfBinsSpectrogram / divideFrequencyBins));
        lowPassCutoffValue = getFrequencyBin(numberOfBinsSpectrogram / divideFrequencyBins, getSliderPos2(numberOfBinsSpectrogram / divideFrequencyBins));
        SetCutoffValues(noise, lowPassCutoffValue, highPassCutoffValue);
        

    }
}
