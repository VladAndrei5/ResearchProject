using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class SpectrogramGenerator : MonoBehaviour
{

    //debuggingObject
    public float[] debugColor;

    //for mesh
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    List<int> trianglesTemp = new List<int>();
    Color[] colors;
    public Gradient gradient;
    //for spectrogram logic
    public int numberOfBins;
    //spectrogram window height and width
    public float spectrogramHeight;
    public float spectrogramWidth;
    //spectrogram number of vertices/pixels
    private int numberPixelsX;
    public int numberPixelsY = 400;
    //distance between pixels, horizontally and vertically
    private float pixelHeight;
    private float pixelWidth;
    //array to hold the previous frame's spectrogram colors
    private float[] prevColors;
    //array to update the bottom row of spectrogram
    public float[] spectrum;
    private float minDecebels;
    private float maxDecebels;

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

    float convertToDecebels(float number){

        float dB;
        if (number != 0)
            dB = 20.0f * Mathf.Log10(number);
        else
            dB = -144.0f;

        minDecebels = -100f;
        maxDecebels = 0;

        float ndB = Mathf.InverseLerp(minDecebels, maxDecebels, dB);
        
        return ndB;
    }


    void UpdateColors(){
        //updates the bottom row
        debugColor = new float[numberPixelsX];

        for(int x = 0; x < numberPixelsX; x++){
                colors[x] = gradient.Evaluate(convertToDecebels(spectrum[x]));
                debugColor[x] = convertToDecebels(spectrum[x]);
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
        //need this for meshes with large number of triangles
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;
        // (change these values)
        //numberOfBins = 512;
        //numberPixelsY = 400
        //spectrogramSecondsHistory = 128;
        //spectrogramHeight = 100;
        //spectrogramWidth = 100;
        //updatesPerSecond = 1;
        //minDecebels = 0;
        //maxDecebels = 20;
        //-------------------
        //keep unchanged
        numberPixelsX = numberOfBins;
        //get distance between pixels
        pixelHeight = spectrogramHeight / numberPixelsY;
        pixelWidth = spectrogramWidth / numberPixelsX;
        //create arrays to hold vertices positions, triangles and the colors
        vertices = new Vector3[numberPixelsX * numberPixelsY];
        colors = new Color[numberPixelsX * numberPixelsY];
        prevColors = new float[numberPixelsX * numberPixelsY];
        triangles = new int[(numberPixelsX - 1) * (numberPixelsY - 1) * 6];
        spectrum  = new float[numberOfBins];
        //-------------------
        
        CreateMesh();
        
    }


    void FixedUpdate(){

        AudioListener.GetSpectrumData(spectrum , 0, FFTWindow.BlackmanHarris);
        UpdateColors();

    }
}
