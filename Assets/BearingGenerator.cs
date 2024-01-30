using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=UnityEngine.Random;

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

    public Gradient gradient;
    public int numberOfBins;
    public float bearingHeight;
    public float bearingWidth;
    public int numberPixelsX;
    public int numberPixelsY;
    private float pixelHeight;
    private float pixelWidth;
    public float[] spectrumAudioSource;
    public float[] spectrumBearing;
    public float[] beamWidthsArr;
    public float[] frequencyNumberArr;

    void CreateMesh(){

        int c1 = 0;
        for(float y = 0; y < bearingHeight; y+= pixelHeight){
            for(float x = 0; x < bearingWidth; x+= pixelWidth){
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

    void UpdateColors(){

        for(int x = 0; x < numberPixelsX; x++){
                colors[x] = gradient.Evaluate(spectrumBearing[x]);
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

        pixelHeight = bearingHeight / numberPixelsY;
        pixelWidth = bearingWidth / numberPixelsX;
        //create arrays to hold vertices positions, triangles and the colors
        vertices = new Vector3[numberPixelsX * numberPixelsY];
        colors = new Color[numberPixelsX * numberPixelsY];
        triangles = new int[(numberPixelsX - 1) * (numberPixelsY - 1) * 6];

        spectrumAudioSource  = new float[numberOfBins];
        spectrumBearing = new float[numberPixelsX]; //Note that for the bearing plot, this spectrum is independent from the number of frequency bins, so it can be any length
        beamWidthsArr = new float[(int)(numberOfBins / gameLogic.divideFrequencyBins)];

        frequencyNumberArr = gameLogic.createFreqNumArr(beamWidthsArr.Length);
        for(int i = 0; i < frequencyNumberArr.Length; i++){
            beamWidthsArr[i] = (float)(gameLogic.getBeamWidth(frequencyNumberArr[i]) % 3.1415926535897931);
        }
        
        CreateMesh();
    }

    void updateSpectrumBearing(int lineCenter, int lineThick, float amp)
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

    void FixedUpdate(){
        soundSourcesArr = gameLogic.soundSourcesArr;

        for(int i = 0; i < spectrumBearing.Length; i++){
            spectrumBearing[i] = 0f;
        }

        Debug.Log(soundSourcesArr.Length);
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
                //int lineThickness = 5;
                float amplitude = spectrumAudioSource[j] * spectrumAudioSource[j];
                //float amplitude = 10000;
                updateSpectrumBearing(lineCenterPoint, lineThickness, amplitude);
            }

        }

        for (int i = 0; i < spectrumBearing.Length; i++){
            spectrumBearing[i] = gameLogic.normaliseSoundDecebels(gameLogic.convertToDecebels(spectrumBearing[i] / 100)) + Random.Range(0f,0.05f);
        }

        UpdateColors();
    }
}
