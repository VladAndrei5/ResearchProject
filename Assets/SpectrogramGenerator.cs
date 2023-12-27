using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class SpectrogramGenerator : MonoBehaviour
{
    public GameLogic gameLogic;

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    List<int> trianglesTemp = new List<int>();
    Color[] colors;
    public Gradient gradient;
    public int numberOfBins;
    public float spectrogramHeight;
    public float spectrogramWidth;
    private int numberPixelsX;
    public int numberPixelsY = 400;
    private float pixelHeight;
    private float pixelWidth;
    public float[] spectrum;
    public float minDecebels;
    public float maxDecebels;


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
                colors[x] = gradient.Evaluate(gameLogic.normaliseSoundDecebels(gameLogic.convertToDecebels(spectrum[x])));
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

        numberPixelsX = numberOfBins;
        pixelHeight = spectrogramHeight / numberPixelsY;
        pixelWidth = spectrogramWidth / numberPixelsX;
        vertices = new Vector3[numberPixelsX * numberPixelsY];
        colors = new Color[numberPixelsX * numberPixelsY];
        triangles = new int[(numberPixelsX - 1) * (numberPixelsY - 1) * 6];
        spectrum  = new float[numberOfBins];
        
        CreateMesh();       
    }


    void FixedUpdate(){

        AudioListener.GetSpectrumData(spectrum , 0, FFTWindow.BlackmanHarris);
        UpdateColors();

    }
}
