using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Audio;
using Random=UnityEngine.Random;
using System.IO;

public class Utilities : MonoBehaviour
{
    public float divideFrequencyBins = 2f;
    //maximum frequencies from unity and user defined

    //VERY IMPORTANT: The maximum frequency in Unity for the spectrum data is 24000 hz.
    //Lets say we allocate 1024 bins as resolution
    //If we want to use only a range from 0 - 8000 hz for example, we would have to use only 
    //the first 1/3 of the bins as the line of pixels we want to update, at the top of the plot
    //I haven't thought about how to automate this in an intuitive matter yet, so for now we will plug
    //the correct values
    //This whould only bee needed for the spectrogram type of plots however

    private int maxUnityFrequency = 24000;
    public int maxSonarFrequency = 12000;

    //set the limits for the decebels
    //used in normalisation
    private float minDecebels = -144f;
    private float maxDecebels = 0f;

    //sound speed in water measured in meters per second
    private float speedOfSound = 1400;
    private float lengthSensor = 100;

    //references to classes
    public UserControls userControls;
    public EntityManager entityManager;
    public PlotManager plotManager;
    GameObject[] soundSourcesArr;

    //array which holds and updates the relative angle of all soundsources to the beam
    [SerializeField]
    float[] relativeAngleArray;
    //array which simply holds the angles of all the soundsources
    float[] angleArray;

    private static float[] erfInverseTableInput = new float[]
    {
        -2.0f, -1.9f, -1.8f, -1.7f, -1.6f, -1.5f, -1.4f, -1.3f, -1.2f, -1.1f, -1.0f,
        -0.9f, -0.8f, -0.7f, -0.6f, -0.5f, -0.4f, -0.3f, -0.2f, -0.1f, 0.0f,
        0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1.0f,
        1.1f, 1.2f, 1.3f, 1.4f, 1.5f, 1.6f, 1.7f, 1.8f, 1.9f, 2.0f
    };

    private static float[] erfInverseTableOutput = new float[]
    {
        -1.0f, -0.967421566101701f, -0.906193802436823f, -0.845062911059928f, -0.784390259558739f, -0.724219961849578f, -0.664879207283284f, -0.606567141441516f, -0.549497375276214f, -0.493891452682922f, 0.0f,
        0.493891452682922f, 0.549497375276214f, 0.606567141441516f, 0.664879207283284f, 0.724219961849578f, 0.784390259558739f, 0.845062911059928f, 0.906193802436823f, 0.967421566101701f, 1.0f,
        1.0f, 1.032578541973500f, 1.09380620563838f, 1.15493709701527f, 1.21560974045797f, 1.27578003816713f, 1.33512079273342f, 1.39343285857518f, 1.45050262474048f, 1.50610854733377f,
        1.50610854733377f, 1.50610854733377f, 1.50610854733377f, 1.50610854733377f, 1.50610854733377f, 1.50610854733377f, 1.50610854733377f, 1.50610854733377f, 1.50610854733377f, 1.50610854733377f
    };

    void Start(){
        soundSourcesArr = entityManager.getSoundSourcesArray();
        relativeAngleArray = new float[soundSourcesArr.Length];
        angleArray = new float[soundSourcesArr.Length];
        for (int i = 0; i < soundSourcesArr.Length; i++){
            relativeAngleArray[i] = 0f;
            angleArray[i] = 0f;
        }
    }


    //update the relative angle off all sound sources to the beam
    //they are all updated here in order to not have each plot make its own calculations
    void Update(){
        for (int i = 0; i < soundSourcesArr.Length; i++){
            relativeAngleArray[i] = getSoundSourceAngleRelativeToBeam(soundSourcesArr[i]);
            angleArray[i] =  getSoundSourceAngle(soundSourcesArr[i]);
        }
    }

    public float getRelativeAngleAtSoundSource(int i){
        return relativeAngleArray[i];
    }

    public float getAngleAtSoundSource(int i){
        return angleArray[i];
    }

    /*returns the angle of the object in relation to the sub's vector facing North
    North directly above origin is 0 degrees, and South is 180 degrees.
    If the object is on the right side of the map, the angle will be given clockwise from 0 to -180
    If the object is on the left side of the map, the angle will be given anti-clockwise from 0 to +180
             0

    +90      Sub     -90

            +180
    */
    public float getSoundSourceAngle(GameObject soundSource){
        float angle;
        angle = 0f;
        float x = soundSource.transform.localPosition.x;
        float y = soundSource.transform.localPosition.y;
        Vector2 targerDir = new Vector2(x, y);
        if (x == 0){
            angle = 0f;
        }
        else if (x < 0){
            angle = Vector2.Angle(targerDir, new Vector2(0, 1));
        }
        else{
            angle = -1f * Vector2.Angle(targerDir, new Vector2(0, 1));
        }
        if (y < 0 && angle == 0){
            angle = 180;
        }
        return angle;
    }

    /*returns the magnitude of the angle between the vector pointing
     towards the soundsource and the beam itself in degrees
    */
    public float getSoundSourceAngleRelativeToBeam(GameObject soundSource){
        float angle = getSoundSourceAngle(soundSource);
        if(angle < 0){
            angle = (180 - ( -1 *angle)) + 180;
        }
        float result = Math.Abs(angle - userControls.getBeamRotation());
        if(result <= 180){
            return result;
        }
        else{
            return Math.Abs(360 - result);
        }
    }

    //creates the array of ranges measured in radians from which a frequency can be picked up 
    //by the sonar
    public float[] CreateDetectableRangesArray(float[] frequencyNumberArr){
        float[] rangesArr = new float[frequencyNumberArr.Length];
        for(int i = 0; i < frequencyNumberArr.Length; i++){
            rangesArr[i] = getDetectableRangeOfFrequency(frequencyNumberArr[i]);
        }
        return rangesArr;
    }

    /*for a given frequency returns the range width from which it can be heard
    in radians
    */
    public float getDetectableRangeOfFrequency(float frequency){
        return speedOfSound / (frequency * lengthSensor);
    }

    /*converts amplitude stored in spectrum data to decebels
    */
    public float convertToDecebels(float number){
        float dB;
        if (number != 0)
            dB = 20.0f * Mathf.Log10(number);
        else
            dB = minDecebels;
        return dB;
    }

    /*normalises a value given in decebels
    */
    public float normaliseSoundDecebels( float value){
        float result = Mathf.InverseLerp(minDecebels, maxDecebels, value);
        return result;
    }

    /*for a given array index and array length
    it returns the frequency bin for said cell
    */
    public float getFrequencyBin(int arrayLength, int i){
        return ((maxSonarFrequency / arrayLength) * (i + 1));
    }

    /*creates an array where each cell holds the frequency of corresponding bin,
     given a user defined frequency upperbound
    */
    public float[] createFreqArray(int arrayLength){
        float[] arr = new float[arrayLength];
        float increment = maxSonarFrequency / arrayLength;
        float multiplier = 1;
        for(int i = 0; i < arrayLength; i++){
            arr[i] = increment * multiplier;
            multiplier++;
        }
        return arr;
    }

    /*returns an index in the freqquency number array
    correspondingto the minimum frequency bound
    */
    public int getFrequencyArrayIndexLowBound(int arrayLength){
        return (int)(userControls.getMinBandwidth() * arrayLength);
    }
    
    /*returns an index in the freqquency number array
    correspondingto to the maximum frequency bound
    */
    public int getFrequencyArrayIndexUpperBound(int arrayLength){
        return (int)(userControls.getMaxBandwidth() * arrayLength) - 1;
    }

    public float NormaliseValue(float value, float minValue, float maxValue){
        return (value - minValue) / (maxValue - minValue);
    }

    public float getLowerBandwithBoundFreq(int arrayLen){
        return getFrequencyBin(arrayLen, getFrequencyArrayIndexLowBound(arrayLen));
    }

    public float getHigherBandwithBoundFreq(int arrayLen){
        return getFrequencyBin(arrayLen, getFrequencyArrayIndexUpperBound(arrayLen));
    }

    //wrong, implement it later based on physics
    //standard diviation of the gaussian distributed white noise on the time bearing plot
    public float calculateStandardDeviationNoiseBearing(int arrayLen){
        float low = getLowerBandwithBoundFreq(arrayLen);
        float high = getHigherBandwithBoundFreq(arrayLen);
        return (high - low) / 100000000f;
    }

    //produces a gaussian distributed random number
    //mean neglected
    public float Gaussian (float mean, float standardDeviation) {
        float c = Random.Range(0f, 1f);
        return standardDeviation * (float)Math.Sqrt(2) * LookupErfInverse(2*c - 1);
    }

    public float getSoundIntensity(float theta, float frequency){
        return (float) Math.Exp( (-1 * Math.Pow(theta, 2) ) / (2 * Math.Pow( (speedOfSound / (frequency * lengthSensor)) , 2) ) );
    }

    static float LookupErfInverse(float x){

        int index = 0;
        for (int i = 0; i < erfInverseTableInput.Length; i++){
            if (erfInverseTableInput[i] > x)
            {
                index = i;
                break;
            }
        }

        if (index == 0)
            return erfInverseTableOutput[0];
        else if (index == erfInverseTableInput.Length)
            return erfInverseTableOutput[erfInverseTableInput.Length - 1];
        else{
            // Linear interpolation
            float x0 = erfInverseTableInput[index - 1];
            float x1 = erfInverseTableInput[index];
            float y0 = erfInverseTableOutput[index - 1];
            float y1 = erfInverseTableOutput[index];

            return y0 + (y1 - y0) * (x - x0) / (x1 - x0);
        }
    }

    public int Remap(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (int)((x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min);
    }   

}
