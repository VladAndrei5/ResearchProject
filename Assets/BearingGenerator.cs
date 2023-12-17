using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=UnityEngine.Random;

[RequireComponent(typeof(MeshFilter))]
public class BearingGenerator : MonoBehaviour
{

    //debuggingObject
    public float[] debugColor;

    //Add more later
    public GameObject bearingReference;


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
    private int numberPixelsX;
    public int numberPixelsY;
    private float pixelHeight;
    private float pixelWidth;
    private float[] toPaint;
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
        soundSourcesArray = new GameObject[2];
        soundSourcesArray[0] = soundSource1;
        soundSourcesArray[1] = soundSource2;
        //keep unchanged
        numberPixelsX = numberOfBins;
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

    float getRotationBearingReference(){
        float angle;
        angle = bearingReference.transform.localRotation.z;
        return angle;
    }

    float getRotationSoundSource(GameObject soundSource){
        float angle;
        angle = 0f;
        float x = soundSource.transform.localPosition.x;
        float y = soundSource.transform.localPosition.y;
        Vector2 targerDir = new Vector2(x, y);
        
        if(x == 0 ){
            angle = 0f;
        }
        else if(x < 0){
            angle = Vector2.Angle(targerDir, new Vector2(0,1));
        }
        else{
            angle = -1f * Vector2.Angle(targerDir, new Vector2(0,1));
        }

        if(y < 0 && angle == 0){
            angle = 180;
        }
        
        return angle;
    }
    
    int Remap(float x, float in_min, float in_max, float out_min, float out_max){

        return (int)((x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min);
    }

    void FixedUpdate(){

        
        for(int i = 0; i < toPaint.Length; i++){
            toPaint[i] = Random.Range(0f,0.1f);
        }
        
        for(int i = 0; i < soundSourcesArray.Length; i++){
            float theta = getRotationSoundSource(soundSourcesArray[i]) - getRotationBearingReference();
            int remap = Remap(theta ,-180f, 180f, 0f, (float)numberPixelsX);
            toPaint[remap] = 1f;

        }
        
        UpdateColors();

    }
}
