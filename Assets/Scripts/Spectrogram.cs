using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Random=UnityEngine.Random;

[RequireComponent(typeof(MeshFilter))]
public class Spectrogram : MonoBehaviour
{
    public float spectrogramHeight;
    public float spectrogramWidth;
    public int resolutionPixelsHeight;
    public int numbSecondDisplayed;
    private GameObject[] soundSourcesArr;
    public EntityManager entityManager;
    public Utilities utilities;
    public Generator generator;




    void Start(){
        //we create a mesh
        Mesh meshSpectrogram = new Mesh();

        //we need this line for large meshes
        meshSpectrogram.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        //the number of rows (lines) of pixels which will be updated in a second
        float resPerSec = resolutionPixelsHeight / numbSecondDisplayed;

        // the interval at each a line is rendered
        float interval = 1f / resPerSec;

        //the number of bins the spectrum will have (has to be a power of 2)
        int numberOfBins = 512;
        float[] spectrumAudioSource  = new float[numberOfBins];

        //the number of pixels horizontally and vertically
        // for the x axis, each pixel will correspond to a bin
        //however we need to make sure that there are fewer pixels than bins since we 
        // want only 0-10,000 hz instead of the maximum range which is 0-20,000
        // I assing the number of pixels horizontally to the number of bins devided by some number
        // in this case 2, but this can be done in any way. We effectively "shave" off the last 10,000 bins
        // since we dont need them
        int numberPixelsX = (int)(numberOfBins / utilities.divideFrequencyBins);
        int numberPixelsY = resolutionPixelsHeight;

        //we create an array which will hold the intensity values of each bin corresponding to the number of pixels
        //this does not hold the colour of the pixel
        float[] pixelsLineSpectrogram = new float[numberPixelsX];
        //beamWidthsArr = new float[numberPixelsX];

        //we get the list of sound sources
        soundSourcesArr = entityManager.getSoundSourcesArray();

        //we create the array which holds the "label" aka the frequency of each bin corresponding to the line of pixels
        //frequencyNumberArr = utilities.createFreqArray(pixelsLineSpectrogram.Length);

        meshSpectrogram = generator.CreateMesh(meshSpectrogram, spectrogramHeight, spectrogramWidth, numberPixelsX, numberPixelsY); 
        GetComponent<MeshFilter>().mesh = meshSpectrogram;
        //StartCoroutine(DrawColors());
    }

    /*
    IEnumerator DrawColors(float interval, Color[] colors){
        while(true){
            pixelsLineSpectrogram = DrawLine();
            colors = Generator.UpdateColorHistory(colors, pixelsLineSpectrogram);
            yield return new WaitForSeconds(interval);
        }
    }

    private void DrawLine(float[] pixelsLineSpectrogram){
        for (int i = 0; i < pixelsLineSpectrogram.Length; i++){
            pixelsLineSpectrogram[i] = 0f;
        }

        //Go through each sound source in the scene
        for (int i = 0; i < soundSourcesArr.Length; i++){
            //and get its spectrum data
            soundSourcesArr[i].GetComponent<AudioSource>().GetSpectrumData(spectrumAudioSource, 0, FFTWindow.BlackmanHarris);
            float soundRotRelative = gameLogic.getSoundSourceAngleRelativeToBeam(soundSourcesArr[i]);
            for (int j = 0; j < pixelsLineSpectrogram.Length; j++){
                //binary mask
                if ((soundRotRelative < (gameLogic.getSonarBeamWidth() / 2) || (soundRotRelative < (beamWidthsArr[j] * Mathf.Rad2Deg / 2)))){
                    pixelsLineSpectrogram[j] += spectrumAudioSource[j];
                }
            }

            float scalingFactor = 0.2f;
            noise.GetComponent<AudioSource>().GetSpectrumData(spectrumAudioSource, 0, FFTWindow.BlackmanHarris);
            for (int j = 0; j < pixelsLineSpectrogram.Length; j++){
                pixelsLineSpectrogram[j] += (spectrumAudioSource[j] * scalingFactor * gameLogic.beamWidth.value * 2);
            }
            
        }

        return pixelsLineSpectrogram;
    }
    */
    
}