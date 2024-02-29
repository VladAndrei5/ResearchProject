using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Random=UnityEngine.Random;

[RequireComponent(typeof(MeshFilter))]
public class SpectrogramGenerator : MonoBehaviour
{
    public GameLogic gameLogic;
    public BearingOverlay bearingOverlay;
    public GameObject[] soundSourcesArr;
    public GameObject noise;
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    List<int> trianglesTemp = new List<int>();
    Color[] colors;
    Color[] historyCol1;
    Color[] historyCol2;
    public Gradient gradient;
    private int numberOfBins;
    public float spectrogramHeight;
    public float spectrogramWidth;
    public int resolution;
    private int numberPixelsX;
    private int numberPixelsY;
    private float resPerSec1;
    private float resPerSec2;
    public float numbSeconds1;
    public float numbSeconds2;
    private float pixelHeight;
    private float pixelWidth;
    public float[] spectrumAudioSource;
    public float[] spectrumSpectrogram;
    public float[] beamWidthsArr;
    public float[] frequencyNumberArr;

    public TextMeshProUGUI  timeTextAxis1;
    public TextMeshProUGUI  timeTextAxis2;   
    public TextMeshProUGUI  timeTextAxis3;
    public TextMeshProUGUI  timeTextMeasure;

    private float interval1;
    private float interval2;

    public Toggle toggle1;
    public Toggle toggle2;



    //creates mesh
    void CreateMesh(){
        
        timeTextMeasure.text = "TIME (m)";
        timeTextAxis1.text = "0";
        timeTextAxis2.text = (numbSeconds1 / 60f / 2f).ToString("0.00");
        timeTextAxis3.text = (numbSeconds1 / 60f).ToString("0.00");


        mesh.Clear();
        numberPixelsY = resolution;
        pixelHeight = spectrogramHeight / (numberPixelsY - 1);
        pixelWidth = spectrogramWidth / (numberPixelsX - 1);
        vertices = new Vector3[numberPixelsX * numberPixelsY];

        historyCol1 = new Color[numberPixelsX * numberPixelsY];
        historyCol2 = new Color[numberPixelsX * numberPixelsY];
        colors = new Color[numberPixelsX * numberPixelsY];
        triangles = new int[(numberPixelsX - 1) * (numberPixelsY - 1) * 6];

        int c1 = 0;
        for(float y = 0; y <= pixelHeight * (numberPixelsY); y+= pixelHeight){
            for(float x = 0; x < pixelWidth * (numberPixelsX); x+= pixelWidth){
                //Debug.Log(c1);
                if(c1 < vertices.Length){
                    vertices[c1] = new Vector3(x,y,0);
                    colors[c1] = gradient.Evaluate(0);
                    historyCol1[c1] = gradient.Evaluate(0);
                    historyCol2[c1] = gradient.Evaluate(0);
                    c1++; 
                }
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
        mesh.vertices = vertices;
        mesh.triangles = triangles;

    }

    //updates the colors
    Color[] UpdateColorHistory(Color[] cols){
        //updates the bottom row
        for(int x = 0; x < numberPixelsX; x++){
                cols[x] = gradient.Evaluate(gameLogic.normaliseSoundDecebels(gameLogic.convertToDecebels(spectrumSpectrogram[x])));
        }

        //replaces each upper row with the one below it, starts with the top one
        int c = (cols.Length - 1);
        for(int y = (numberPixelsY - 1); y >= 1; y--){
            for(int x = 0; x < numberPixelsX; x++){
                //shifts every row up a column
                cols[c] = cols[c - numberPixelsX];
                c--;
            }
        }

        return cols;
    }

    void UpdateColors(Color[] col){
        mesh.colors = col;
    }

    void Start(){
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;

        

        
        numberOfBins = gameLogic.numberOfBinsSpectrogram;
        numberPixelsX = (int)(numberOfBins / gameLogic.divideFrequencyBins);
        spectrumAudioSource  = new float[numberOfBins];
        spectrumSpectrogram = new float[numberPixelsX];
        beamWidthsArr = new float[numberPixelsX];

        soundSourcesArr = gameLogic.soundSourcesArr;

        frequencyNumberArr = gameLogic.createFreqNumArr(spectrumSpectrogram.Length);
        for(int i = 0; i < spectrumSpectrogram.Length; i++){
            spectrumSpectrogram[i] = 0f;
            beamWidthsArr[i] = gameLogic.getBeamWidthAtFreq(frequencyNumberArr[i]);
        }

        resPerSec1 = resolution / numbSeconds1;
        resPerSec2 = resolution / numbSeconds2;
        interval1 = 1f / resPerSec1;
        interval2 = 1f / resPerSec2;

        CreateMesh(); 

        toggle1.onValueChanged.AddListener(OnToggle1ValueChanged);
        toggle2.onValueChanged.AddListener(OnToggle2ValueChanged);
        

        StartCoroutine(UpdateScreenOne());
        StartCoroutine(UpdateScreenTwo());

        

        //numbSecSlider.onValueChanged.AddListener(OnSliderValueChanged);

        //timer = interval;

    }

    /*
    private void OnSliderValueChanged(float value)
    {
        isPaused = true;
        numbSeconds = (float)numbSecSlider.value;
        CreateMesh(); 
        isPaused = false;
    }*/

    private void OnToggle1ValueChanged(bool isOn)
    {
        if (isOn){
            timeTextMeasure.text = "TIME (m)";
            timeTextAxis1.text = "0";
            timeTextAxis2.text = (numbSeconds1 / 60f / 2f).ToString("0.00");
            timeTextAxis3.text = (numbSeconds1 / 60f).ToString("0.00");
            toggle2.isOn = false;
        }
        if((isOn == false) & (toggle2.isOn == false)){
            toggle1.isOn = true;
        }
    }

    private void OnToggle2ValueChanged(bool isOn)
    {
        if (isOn){
            timeTextMeasure.text = "TIME (m)";
            timeTextAxis1.text = "0";
            timeTextAxis2.text = (numbSeconds2 / 60f / 2f).ToString("0.00");
            timeTextAxis3.text = (numbSeconds2 / 60f).ToString("0.00");
            toggle1.isOn = false;
        }
        if((isOn == false) & (toggle1.isOn == false)){
            toggle2.isOn = true;
        }
    }

    IEnumerator UpdateScreenOne(){
        while(true){
            UpdateSpectrum();
            historyCol1 = UpdateColorHistory(historyCol1);
            if(toggle1.isOn == true){
                UpdateColors(historyCol1);
            }
            yield return new WaitForSeconds(interval1);
        }
    }

    IEnumerator UpdateScreenTwo(){
        while(true){

            historyCol2 = UpdateColorHistory(historyCol2);
            if(toggle2.isOn == true){
                UpdateColors(historyCol2);
            }
            yield return new WaitForSeconds(interval2);
        }
    }

    private void UpdateSpectrum(){
        for (int i = 0; i < spectrumSpectrogram.Length; i++){
            spectrumSpectrogram[i] = 0f;
        }

        //Go through each sound source in the scene
        for (int i = 0; i < soundSourcesArr.Length; i++){
            //and get its spectrum data
            soundSourcesArr[i].GetComponent<AudioSource>().GetSpectrumData(spectrumAudioSource, 0, FFTWindow.BlackmanHarris);
            float soundRotRelative = gameLogic.getRelativeRotationToBeam(soundSourcesArr[i]);
            for (int j = 0; j < spectrumSpectrogram.Length; j++){
                //binary mask
                if ((soundRotRelative < (gameLogic.getScanningWidth() / 2) || (soundRotRelative < (beamWidthsArr[j] * Mathf.Rad2Deg / 2)))){
                    spectrumSpectrogram[j] += spectrumAudioSource[j];
                }
                //mask using the formula given by the sound intensity
                /*
                if(soundRotRelative < (bearingOverlay.getScanningWidth() /2 )){
                    spectrumSpectrogram[j] += spectrumAudioSource[j] * gameLogic.getSoundIntensity(soundRotRelative , frequencyNumberArr[i]); 
                }
                */
            }

            float scalingFactor = 0.2f;
            noise.GetComponent<AudioSource>().GetSpectrumData(spectrumAudioSource, 0, FFTWindow.BlackmanHarris);
            for (int j = 0; j < spectrumSpectrogram.Length; j++){
                spectrumSpectrogram[j] += (spectrumAudioSource[j] * scalingFactor * gameLogic.beamWidth.value * 2);

            }
            
        }
    }
}
