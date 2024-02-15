using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=UnityEngine.Random;

[RequireComponent(typeof(MeshFilter))]
public class SpectrogramGenerator : MonoBehaviour
{
    public GameLogic gameLogic;
    public BearingOverlay bearingOverlay;
    public GameObject[] soundSourcesArr;
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    List<int> trianglesTemp = new List<int>();
    Color[] colors;
    public Gradient gradient;
    private int numberOfBins;
    public float spectrogramHeight;
    public float spectrogramWidth;
    private int numberPixelsX;
    public int numberPixelsY = 400;
    private float pixelHeight;
    private float pixelWidth;
    public float[] spectrumAudioSource;
    public float[] spectrumSpectrogram;
    public float[] beamWidthsArr;
    public float[] frequencyNumberArr;

    //creates mesh
    void CreateMesh(){

        int c1 = 0;
        for(float y = 0; y < spectrogramHeight; y+= pixelHeight){
            for(float x = 0; x < spectrogramWidth; x+= pixelWidth){
                vertices[c1] = new Vector3(x,y,0);
                colors[c1] = gradient.Evaluate(0);
                c1++;
            }
        }

        int c2 = 0;
        for(int y = 0; y < numberPixelsY - 1 ; y++){
            for(int x = 0; x < numberPixelsX - 1 ; x++){
                trianglesTemp.Add(c2);
                trianglesTemp.Add(c2+numberPixelsX);
                trianglesTemp.Add(c2+1);

                trianglesTemp.Add(c2+numberPixelsX);
                trianglesTemp.Add(c2+numberPixelsX+1);
                trianglesTemp.Add(c2+1);

                c2++;
            }

            c2++;
        }
        triangles = trianglesTemp.ToArray();

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

    }

    //updates the colors
    void UpdateColors(){
        //updates the bottom row
        for(int x = 0; x < numberPixelsX; x++){
                colors[x] = gradient.Evaluate(gameLogic.normaliseSoundDecebels(gameLogic.convertToDecebels(spectrumSpectrogram[x]))  +  Random.Range(0f, (gameLogic.sliderMaxFreqBound.value - gameLogic.sliderMinFreqBound.value) / 40));
        }

        //replaces each upper row with the one below it, starts with the top one
        int c = (colors.Length - 1);
        for(int y = (numberPixelsY - 1); y >= 1; y--){
            for(int x = 0; x < numberPixelsX; x++){
                //shifts every row up a column
                colors[c] = colors[c - numberPixelsX];
                c--;
            }
        }

        mesh.colors = colors;
    }

    void Start(){
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;

        
        numberOfBins = gameLogic.numberOfBinsSpectrogram;
        soundSourcesArr = gameLogic.soundSourcesArr;

        numberPixelsX = (int)(numberOfBins / gameLogic.divideFrequencyBins);
        pixelHeight = spectrogramHeight / numberPixelsY;
        pixelWidth = spectrogramWidth / numberPixelsX;
        vertices = new Vector3[numberPixelsX * numberPixelsY];
        colors = new Color[numberPixelsX * numberPixelsY];
        triangles = new int[(numberPixelsX - 1) * (numberPixelsY - 1) * 6];
        spectrumAudioSource  = new float[numberOfBins];
        spectrumSpectrogram = new float[numberPixelsX];
        beamWidthsArr = new float[numberPixelsX];

        frequencyNumberArr = gameLogic.createFreqNumArr(spectrumSpectrogram.Length);
        for(int i = 0; i < spectrumSpectrogram.Length; i++){
            spectrumSpectrogram[i] = 0f;
            beamWidthsArr[i] = gameLogic.getBeamWidth(frequencyNumberArr[i]);
        }


        CreateMesh();     
    }


    void FixedUpdate(){
        soundSourcesArr = gameLogic.soundSourcesArr;

        for(int i = 0; i < spectrumSpectrogram.Length; i++){
            spectrumSpectrogram[i] = 0f;
        }

        //Go through each sound source in the scene
        for(int i = 0; i < soundSourcesArr.Length; i++){

            //and get its spectrum data
            soundSourcesArr[i].GetComponent<AudioSource>().GetSpectrumData(spectrumAudioSource, 0, FFTWindow.BlackmanHarris);
            float soundRotRelative = gameLogic.getRelativeRotationToBeam(soundSourcesArr[i]);

            for(int j = 0; j < spectrumSpectrogram.Length; j++){
                
                //binary mask
                
                if( (soundRotRelative < (bearingOverlay.getScanningWidth() /2) ||  ( soundRotRelative < (beamWidthsArr[j] * Mathf.Rad2Deg / 2)) )){

                    spectrumSpectrogram[j] += spectrumAudioSource[j];  
                
                }
                

                
                //mask using the formula given by the sound intensity

                /*
                if(soundRotRelative < (bearingOverlay.getScanningWidth() /2 )){
                    spectrumSpectrogram[j] += spectrumAudioSource[j] * gameLogic.getSoundIntensity(soundRotRelative , frequencyNumberArr[i]); 
                }
                */

            }
            
        }

        UpdateColors();
    }
}
