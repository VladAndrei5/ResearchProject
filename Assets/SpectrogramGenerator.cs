using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class SpectrogramGenerator : MonoBehaviour
{
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
    private float timer;
    private float drawCycle;

    private float[] prevColors;
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

    
    void UpdateColors(){
        for(int x = 0; x < numberPixelsX; x++){
                colors[x] = gradient.Evaluate(spectrum[x] * 5000);
        }

        int c = (colors.Length - 1);
        for(int y = (numberPixelsY - 1); y >= 1; y--){
            for(int x = 0; x < numberPixelsX; x++){
                colors[c] = colors[c - numberPixelsX];
                c--;
            }
        }

        mesh.colors = colors;
        //yield return null;
    }

    void Start(){
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;
        //change these values
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
        pixelHeight = spectrogramHeight / numberPixelsY;
        pixelWidth = spectrogramWidth / numberPixelsX;
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
