using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random=UnityEngine.Random;
using TMPro;


[RequireComponent(typeof(MeshFilter))]
public class BearingGenerator : MonoBehaviour
{

    public GameLogic gameLogic;

    //create sound sources array
    public GameObject[] soundSourcesArr;

    //for mesh
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    List<int> trianglesTemp = new List<int>();
    Color[] colors;

    Color[] historyCol1;
    Color[] historyCol2;

    public Gradient gradient;
    private int numberOfBins;
    public float bearingHeight;
    public float bearingWidth;
    public int resolution;
    public int numberPixelsX;
    private int numberPixelsY;
    private float resPerSec1;
    private float resPerSec2;
    public float numbSeconds1;
    public float numbSeconds2;
    private float pixelHeight;
    private float pixelWidth;
    public float[] spectrumAudioSource;
    public float[] spectrumBearing;
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

    void CreateMesh(){
        
        timeTextMeasure.text = "TIME (m)";
        timeTextAxis1.text = "0";
        timeTextAxis2.text = (numbSeconds1 / 60f / 2f).ToString("0.00");
        timeTextAxis3.text = (numbSeconds1 / 60f).ToString("0.00");


        mesh.Clear();
        numberPixelsY = resolution;
        pixelHeight = bearingHeight / numberPixelsY;
        pixelWidth = bearingWidth / numberPixelsX;
        vertices = new Vector3[numberPixelsX * numberPixelsY];

        historyCol1 = new Color[numberPixelsX * numberPixelsY];
        historyCol2 = new Color[numberPixelsX * numberPixelsY];
        colors = new Color[numberPixelsX * numberPixelsY];
        triangles = new int[(numberPixelsX - 1) * (numberPixelsY - 1) * 6];

        int c1 = 0;
        for(float y = 0; y < bearingHeight; y+= pixelHeight){
            for(float x = 0; x < bearingWidth; x+= pixelWidth){
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

    void UpdateColors(Color[] col){
        mesh.colors = col;
    }

    Color[] UpdateColorHistory(Color[] cols){
        //updates the bottom row
        for(int x = 0; x < numberPixelsX; x++){
            cols[x] = gradient.Evaluate(gameLogic.normaliseSoundDecebels(gameLogic.convertToDecebels(spectrumBearing[x])));
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
    

    int Remap(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (int)((x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min);
    }

    void Start(){
        mesh = new Mesh();
        //need this for meshes with large number of triangles
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;

        //get distance between pixels
        numberOfBins = gameLogic.numberOfBinsBearing;
        soundSourcesArr = gameLogic.soundSourcesArr;

        spectrumAudioSource  = new float[numberOfBins];
        spectrumBearing = new float[numberPixelsX]; //Note that for the bearing plot, this spectrum is independent from the number of frequency bins, so it can be of any length
        beamWidthsArr = new float[(int)(numberOfBins / gameLogic.divideFrequencyBins)];

        soundSourcesArr = gameLogic.soundSourcesArr;

        frequencyNumberArr = gameLogic.createFreqNumArr(beamWidthsArr.Length);
        for(int i = 0; i < frequencyNumberArr.Length; i++){
            beamWidthsArr[i] = (float)(gameLogic.getBeamWidthAtFreq(frequencyNumberArr[i]) % 3.1415926535897931);
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
    }

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

    // Perform nearest neighbour
    static float LookupErfInverse(float x)
    {

        int index = 0;
        for (int i = 0; i < erfInverseTableInput.Length; i++)
        {
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
        else
        {
            // Linear interpolation
            float x0 = erfInverseTableInput[index - 1];
            float x1 = erfInverseTableInput[index];
            float y0 = erfInverseTableOutput[index - 1];
            float y1 = erfInverseTableOutput[index];

            return y0 + (y1 - y0) * (x - x0) / (x1 - x0);
        }

    }

    float Gaussian (float mean) {
        float c = Random.Range(0f, 1f);
        float sigma = getSigma();
        return sigma * (float)Math.Sqrt(2) * LookupErfInverse(2*c - 1);

    }   

    //wrong, implement it later based on physics
    float getSigma(){
        float low = gameLogic.getLowerBoundFreq();
        float high = gameLogic.getHigherBoundFreq();
        return (gameLogic.getScanningWidth()/360) * (high - low) / 10000f;
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

    void AddSourceToSpectrum(int lineCenter, int lineThick, float amp)
    {
        int arrCutoff = spectrumBearing.Length - 1;
        int[] temp = new int[lineThick * 2 + 1];

        int lineStart = lineCenter - lineThick;
        if(lineStart < 0)
        {
            lineStart = arrCutoff + 1 + lineStart;
        }

        for (int i = 0; i < temp.Length; i++)
        {
            if(lineStart + i > arrCutoff)
            {
                temp[i] = lineThick*2 - i;
            }
            else
            {
                temp[i] = lineStart + i;
            }
        }

        for (int i = 0; i < temp.Length; i++)
        {
            spectrumBearing[temp[i]] = spectrumBearing[temp[i]] + amp; ;
        }
    }

    void UpdateSpectrum(){

        for(int i = 0; i < spectrumBearing.Length; i++){
            spectrumBearing[i] = 0f;
        }

        //for each sound source in the scene
        for(int i = 0; i < soundSourcesArr.Length; i++){
            soundSourcesArr[i].GetComponent<AudioSource>().GetSpectrumData(spectrumAudioSource, 0, FFTWindow.BlackmanHarris);
            float soundRot = gameLogic.getRotationSoundSource(soundSourcesArr[i]);

            float pixelsPerDegree = numberPixelsX / 360f;
            int lineCenterPoint = Remap(soundRot, -180f, 180f, (float)numberPixelsX - 1, 0f);

            int firstPos = gameLogic.getSliderPos1(spectrumAudioSource.Length / gameLogic.divideFrequencyBins);
            int secondPos = gameLogic.getSliderPos2(spectrumAudioSource.Length / gameLogic.divideFrequencyBins);

            for (int j = firstPos; j < secondPos; j++)
            {
                int lineThickness = (int)(beamWidthsArr[j] * Mathf.Rad2Deg * pixelsPerDegree);
                float amplitude = spectrumAudioSource[j] * spectrumAudioSource[j];

                AddSourceToSpectrum(lineCenterPoint, lineThickness, amplitude);
            }

        }

        //add gausian instead of normal
        float diminishFactor = 0.1f;
        for (int i = 0; i < spectrumBearing.Length; i++){
            //spectrumBearing[i] = spectrumBearing[i] + Random.Range(0f, getScalingFactor() * diminishFactor);
            spectrumBearing[i] = spectrumBearing[i] + (Gaussian(0.5f) * diminishFactor);
        }

        /*
        for (int i = 0; i < spectrumBearing.Length; i++){
            spectrumBearing[i] = gameLogic.normaliseSoundDecebels(gameLogic.convertToDecebels(spectrumBearing[i]));
        }
        */

    }
}
