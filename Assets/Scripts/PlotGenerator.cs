using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=UnityEngine.Random;

public class PlotGenerator : MonoBehaviour
{
    //used to create the mesh
    private Vector3[] vertices;
    private int[] triangles;
    private List<int> trianglesTemp = new List<int>();

    //refrences to classes
    public EntityManager entityManager;
    public Utilities utilities;
    public UserControls userControls;

    //array to hold the sound sources game objects
    private GameObject[] soundSourcesArr;
    public GameObject noise;

    //plot specifications and attributes
    private int numberOfBins;
    private int resolutionPixelsHeight;
    private int numbSecondsDisplayed;
    public Gradient gradient;

    //plot variables
    private Mesh mesh;
    private Color[] plotColors;
    private int numberPixelsX;
    private int numberPixelsY;
    float scalingFactor = 0.1f;

    //the first line of pixels, it will be updated in a way which is dependent on the type of plot
    [SerializeField]
    private float[] pixelsLineAmplitude;
    [SerializeField]
    private float[] spectrumAudioSource;
    //used to hold the 
    [SerializeField]
    private float[] frequenciesDetectableRange;
    [SerializeField]
    private float[] frequencyNumberArr;

    //delegate function used to update the first line of pixels
    // which changes based on the type of plot (bearing or spectrogram)
    private delegate void UpdateLine();
    private UpdateLine updateLineDelegateFunction;


    public void Initialise(float height, float width, int resolutionPixelsHeight, int resolutionPixelsWidth, int numbSecondDisplayed, int numberOfBins, string plotType){
        //we create a mesh
        mesh = new Mesh();
        //we need this line for large meshes
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;
        
        //the number of rows (lines) of pixels which will be updated in a second
        float resPerSec = resolutionPixelsHeight / numbSecondDisplayed;

        // the interval at each a line is rendered
        float interval = 1f / resPerSec;

        //the number of bins the spectrum will have (has to be a power of 2)
        this.numberOfBins = numberOfBins;
        float[] spectrumAudioSource  = new float[numberOfBins];

        //the number of pixels horizontally and vertically
        // for the x axis, each pixel will correspond to a bin
        //however we need to make sure that there are fewer pixels than bins since we 
        // want only 0-10,000 hz instead of the maximum range which is 0-20,000
        // I assing the number of pixels horizontally to the number of bins devided by some number
        // in this case 2, but this can be done in any way. We effectively "shave" off the last 10,000 bins
        // since we dont need them
        numberPixelsX = resolutionPixelsWidth;
        numberPixelsY = resolutionPixelsHeight;

        //create the colors array which will hold the color for each vertex/pixel
        plotColors = new Color[numberPixelsX * numberPixelsY];

        //we create an array which will hold the intensity values of each bin corresponding to the number of pixels
        //this does not hold the colour of the pixel
        pixelsLineAmplitude = new float[numberPixelsX];


        //we get the list of sound sources
        soundSourcesArr = entityManager.getSoundSourcesArray();

        //we create the array which holds the "label" aka the frequency of each bin corresponding to the line of pixels
        frequencyNumberArr = utilities.createFreqArray(pixelsLineAmplitude.Length);

        //create the range at which certain frequencies can be picked up
        frequenciesDetectableRange = new float[numberPixelsX];
        frequenciesDetectableRange = utilities.CreateDetectableRangesArray(frequencyNumberArr);

        //Create the mesh
        CreateMesh(height, width); 

        //allocate the functio which will deal with updating the first line of pixels;
        switch (plotType)
        {
            case "spectrogram":
                updateLineDelegateFunction = UpdateLineSpectrogram;
                break;
            case "bearing":
                //updateLineDelegateFunction = UpdateLineSpectrogram;
                updateLineDelegateFunction = UpdateLineBearing;
                break;
        }
        StartCoroutine(StartPlot(interval));
    }

    //continously updates the plot
    IEnumerator StartPlot(float interval){
        while(true){
            updateLineDelegateFunction();
            UpdateColors();
            UpdatePlot(plotColors);
            yield return new WaitForSeconds(interval);
        }
    }

    //it update the mesh's vertex colors with a given array of colors
    //effectively updating the plot the user will see
    public void UpdatePlot(Color[] colors){
        mesh.colors = colors;
    }

    //it updates the array of colors used by the plot
    //by inserting the updated line of pixels at the top
    public void UpdateColors(){
        //updates the bottom row
        for(int x = 0; x < numberPixelsX; x++){
            plotColors[x] = gradient.Evaluate(utilities.normaliseSoundDecebels(utilities.convertToDecebels(pixelsLineAmplitude[x])));
        }

        //replaces each upper row with the one below it, starts with the top one
        //efectively shifts the plot upwards
        int c = (plotColors.Length - 1);
        for(int y = (numberPixelsY - 1); y >= 1; y--){
            for(int x = 0; x < numberPixelsX; x++){
                //shifts every row up a column
                plotColors[c] = plotColors[c - numberPixelsX];
                c--;
            }
        }
    }

    private void UpdateLineSpectrogram(){
        spectrumAudioSource = new float[numberOfBins];
        for (int i = 0; i < pixelsLineAmplitude.Length; i++){
            pixelsLineAmplitude[i] = 0f;
        }
         //Go through each sound source in the scene
        for (int i = 0; i < soundSourcesArr.Length; i++){
            //and get its spectrum data
            soundSourcesArr[i].GetComponent<AudioSource>().GetSpectrumData(spectrumAudioSource, 0, FFTWindow.BlackmanHarris);
            //find the relative angle of the sound source to the beam
            float soundRotRelative = utilities.getRelativeAngleAtSoundSource(i);
            for (int j = 0; j < pixelsLineAmplitude.Length; j++){
                //binary mask
                //check if the relative angle of the sound source is within the user defined beam
                //or if the frequency can be detected from said angle
                if ((soundRotRelative < (userControls.getSonarBeamWidth() / 2) || (soundRotRelative < (frequenciesDetectableRange[j] * Mathf.Rad2Deg / 2)))){
                    pixelsLineAmplitude[j] += spectrumAudioSource[j];
                }
                
                //mask using the formula given by the sound intensity
                /*
                if((soundRotRelative < (userControls.getSonarBeamWidth() / 2) || (soundRotRelative < (frequenciesDetectableRange[j] * Mathf.Rad2Deg / 2)))){
                    pixelsLineAmplitude[j] += spectrumAudioSource[j] * utilities.getSoundIntensity(soundRotRelative , frequencyNumberArr[i]); 
                }
                */
            }
            
            //noise function will have to look into
            noise.GetComponent<AudioSource>().GetSpectrumData(spectrumAudioSource, 0, FFTWindow.BlackmanHarris);
            for (int j = 0; j < pixelsLineAmplitude.Length; j++){
                //pixelsLineAmplitude[j] += (spectrumAudioSource[j] * scalingFactor * (userControls.getSonarBeamWidth() * Mathf.Deg2Rad));
                pixelsLineAmplitude[j] += spectrumAudioSource[j];
            }
        }
    }

    private void UpdateLineBearing(){
        spectrumAudioSource = new float[numberOfBins];
        for(int i = 0; i < pixelsLineAmplitude.Length; i++){
            pixelsLineAmplitude[i] = 0f;
        }

        //for each sound source in the scene
        for(int i = 0; i < soundSourcesArr.Length; i++){
            soundSourcesArr[i].GetComponent<AudioSource>().GetSpectrumData(spectrumAudioSource, 0, FFTWindow.BlackmanHarris);
            float soundRot =  utilities.getAngleAtSoundSource(i);

            //how many pixels are drawn per degree
            float pixelsPerDegree = numberPixelsX / 360f;
            //get the center index for the sound source on the bearing plot
            int lineCenterPoint = utilities.Remap(soundRot, -180f, 180f, (float)numberPixelsX - 1, 0f);

            //get the frequency indencies for the user defined limits for the bandwith
            int firstPos = utilities.getFrequencyArrayIndexLowBound(pixelsLineAmplitude.Length);
            int secondPos = utilities.getFrequencyArrayIndexUpperBound(pixelsLineAmplitude.Length);
            for (int j = firstPos; j < secondPos; j++)
            {
                //for a frequency, get the range in degrees, from which it can be picked up when it is emmitied from 
                //some sound source in relation to the submarine
                int lineLength = (int)(frequenciesDetectableRange[j] * Mathf.Rad2Deg / 2 * pixelsPerDegree);
                float amplitude = spectrumAudioSource[j] * spectrumAudioSource[j];

                AddSourceToLine(lineCenterPoint, lineLength, amplitude);
            }
        }

        //add gausian instead of normal
        for (int i = 0; i < pixelsLineAmplitude.Length; i++){
            //adds gaussian white noise to a time sample in the bearing plot
            pixelsLineAmplitude[i] = pixelsLineAmplitude[i] + (scalingFactor * utilities.Gaussian(0f, utilities.calculateStandardDeviationNoiseBearing(frequencyNumberArr.Length)));
        }
        /*
        for (int i = 0; i < pixelsLine.Length; i++){
            pixelsLine[i] = utilities.normaliseSoundDecebels(utilities.convertToDecebels(pixelsLine[i]));
        }
        */
    }

    private void AddSourceToLine(int lineCenter, int lineLength, float amp)
    {
        int arrCutoff = pixelsLineAmplitude.Length - 1;
        int[] temp = new int[lineLength * 2 + 1];

        int lineStart = lineCenter - lineLength;
        if(lineStart < 0){
            lineStart = arrCutoff + 1 + lineStart;
        }

        for (int i = 0; i < temp.Length; i++)
        {
            if(lineStart + i > arrCutoff){
                temp[i] = lineLength*2 - i;
            }
            else{
                temp[i] = lineStart + i;
            }
        }

        for (int i = 0; i < temp.Length; i++){
            pixelsLineAmplitude[temp[i]] = pixelsLineAmplitude[temp[i]] + amp;
        }
    }

    //function to create a mesh by specifying the height and width of the plot, and its resolution
    //I followed Brackeys tutorial on : https://www.youtube.com/watch?v=eJEpeUH1EMg
    public void CreateMesh(float height, float width){
        //assign the distance between pixels (the name is slightly unaccurate)
        //both horizontally and vertically
        float pixelHeight = 1f;
        float pixelWidth = 1f;
        float scaleX = height /numberPixelsX;
        float scaleY = (-1 * width) /numberPixelsY;

        //scale the plot accordingly so it fits the desired dimensions
        transform.localScale = new Vector3(scaleX, scaleY, 1f);

        //create array to hold the triangles of the mesh
        triangles = new int[(numberPixelsX - 1) * (numberPixelsY - 1) * 6];
        vertices = new Vector3[numberPixelsX * numberPixelsY];
        
        //counter
        int c1 = 0;
        for(float y = 0; y < pixelHeight * numberPixelsY; y+= pixelHeight){
            for(float x = 0; x < pixelWidth * numberPixelsX; x+= pixelWidth){
                if(c1 < vertices.Length){
                    vertices[c1] = new Vector3(x,y,0);
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
}

