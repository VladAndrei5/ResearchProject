using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    private Vector3[] vertices;
    private int[] triangles;
    private List<int> trianglesTemp = new List<int>();

    public int numberOfBinsSpectrogram = 512;
    public int numberOfBinsBearing = 512;


    void Awake(){
        //numberOfBinsSpectrogram = 512;
        //numberOfBinsBearing = 512;
    }


    //function to create a mesh by specifying the height and width of the plot, and its resolution
    //I followed Brackeys tutorial on : https://www.youtube.com/watch?v=eJEpeUH1EMg
    public Mesh CreateMesh(Mesh mesh, float Height, float Width, int numberPixelsX, int numberPixelsY ){

        //assign the distance between pixels (the name is slightly unaccurate)
        //both horizontally and vertically
        float pixelHeight = Height / (numberPixelsY - 1);
        float pixelWidth = Width / (numberPixelsX - 1);

        //create array to hold the triangles of the mesh
        triangles = new int[(numberPixelsX - 1) * (numberPixelsY - 1) * 6];

        //counter
        int c1 = 0;
        for(float y = 0; y <= pixelHeight * (numberPixelsY); y+= pixelHeight){
            for(float x = 0; x < pixelWidth * (numberPixelsX); x+= pixelWidth){
                //Debug.Log(c1);
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

        return mesh;
    }

    //it update the mesh's vertex colors with a given array of colors
    //effectively updating the plot the user will see
    void UpdateScreen(Mesh mesh, Color[] col){
        mesh.colors = col;
        return mesh;
    }

    //it updates the array of colors used by the plot
    //by inserting the updated line of pixels at the top
    Color[] UpdateColorHistory(Color[] colors, float[] linePixels){
        //updates the bottom row
        for(int x = 0; x < linePixels; x++){
            colors[x] = gradient.Evaluate(gameLogic.normaliseSoundDecebels(gameLogic.convertToDecebels(linePixels[x])));
        }

        //replaces each upper row with the one below it, starts with the top one
        //efectively shifts the plot upwards
        int c = (colors.Length - 1);
        for(int y = (numberPixelsY - 1); y >= 1; y--){
            for(int x = 0; x < linePixels; x++){
                //shifts every row up a column
                colors[c] = colors[c - linePixels];
                c--;
            }
        }
        
        //return the new array of colors
        return colors;
    }
}
