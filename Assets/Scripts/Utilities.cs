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
    private int maxUnityFrequency = 44000;
    public int maxSonarFrequency = 10000;

    //set the limits for the decebels
    //used in normalisation
    private float minDecebels = -144f;
    private float maxDecebels = 0f;

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



    /*
    // Static reference to the instance of the singleton
    private static Utilities _instance;

    // Public property to access the singleton instance
    public static Utilities Instance
    {
        get { return _instance; }
    }

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Check if an instance already exists
        if (_instance != null && _instance != this)
        {
            // If an instance already exists and it's not this one, destroy this instance
            Destroy(this.gameObject);
            return;
        }

        // Set the instance to this instance
        _instance = this;

        // Ensure that the instance persists between scenes
        DontDestroyOnLoad(this.gameObject);
    }

    // Example method of the singleton
    public void CopyArray<T>(T[] destination, T[] source){
        // Check if arrays are of the same length
        if (destination.Length != source.Length){
            Debug.LogError("Arrays must be of the same length.");
            return;
        }

        // Copy elements from source to destination
        for (int i = 0; i < source.Length; i++){
            destination[i] = source[i];
        }
    }
    */

}
