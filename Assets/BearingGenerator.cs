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
    public GameObject soundSource1;
    public GameObject soundSource2;
    public GameObject[] soundSourcesArray;

    //for mesh
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    List<int> trianglesTemp = new List<int>();
    Color[] colors;

    public Gradient gradient;
    public int numberOfBins;
    public float spectrogramHeight;
    public float spectrogramWidth;
    public int numberPixelsX;
    public int numberPixelsY;
    private float pixelHeight;
    private float pixelWidth;
    private float[] toPaint;
    public float[] spectrum;

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

    void UpdateColors(){

        for(int x = 0; x < numberPixelsX; x++){
                colors[x] = gradient.Evaluate(toPaint[x]);
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

        //create sound source array
        soundSourcesArray = new GameObject[2];
        soundSourcesArray[0] = soundSource1;
        soundSourcesArray[1] = soundSource2;

        //keep unchanged

        //get distance between pixels
        pixelHeight = spectrogramHeight / numberPixelsY;
        pixelWidth = spectrogramWidth / numberPixelsX;
        //create arrays to hold vertices positions, triangles and the colors
        vertices = new Vector3[numberPixelsX * numberPixelsY];
        colors = new Color[numberPixelsX * numberPixelsY];
        toPaint = new float[numberPixelsX];
        triangles = new int[(numberPixelsX - 1) * (numberPixelsY - 1) * 6];
        spectrum  = new float[numberOfBins];
        //-------------------
        
        CreateMesh();
    }

    void updateToPaint(int lineCenter, int lineThick, float amp)
    {
        int arrCutoff = toPaint.Length - 1;
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
            toPaint[temp[i]] = toPaint[temp[i]] + amp; ;
        }

        //toPaint[lineCenter] = 0.5f;


    }

    void FixedUpdate(){

        //random noise
        for(int i = 0; i < toPaint.Length; i++){
            toPaint[i] = Random.Range(0f,0.1f);
        }

        //for each sound source in the scene
        for(int i = 0; i < soundSourcesArray.Length; i++){
            soundSourcesArray[i].GetComponent<AudioSource>().GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

            //get the rotation in degrees of sound source
            float soundRot = gameLogic.getRotationSoundSource(soundSourcesArray[i]);
            int lineThickness = (int)gameLogic.getBeamWidth(10f);
            int lineCenterPoint = Remap(soundRot, -180f, 180f, (float)numberPixelsX - 1, 0f);
            float amplitude = 0f;

            for (int j = 0; j < spectrum.Length; j++)
            {
                amplitude = (float)Math.Pow(spectrum[j], 2) + amplitude;
            }

            amplitude = gameLogic.normaliseSoundDecebels( gameLogic.convertToDecebels((float)Math.Sqrt(amplitude)));
            //Debug.Log(amplitude);
            updateToPaint(lineCenterPoint, lineThickness, amplitude);

            //toPaint[lineCenterPoint] = toPaint[lineCenterPoint] + amplitude;

        }
        UpdateColors();
    }
}
