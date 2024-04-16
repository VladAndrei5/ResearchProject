using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Overlay : MonoBehaviour
{
    //bearing overlay components
    public GameObject quadL;
    public GameObject quadR;
    public GameObject quadC;
    MeshFilter meshQuadL;
    MeshFilter meshQuadR;
    MeshFilter meshQuadC;
    Vector3[] originalVerticesQL;
    Vector3[] originalVerticesQR;
    Vector3[] originalVerticesQC;

    //spectrogram overlay components
    public GameObject quadSpectro;
    MeshFilter meshQuadSpectro;
    Vector3[] originalVerticesQuadSpectro;

    //references to classes
    public Utilities utilities;
    public UserControls userControls;
    public PlotManager plotManager;

    //sliders used to display the beamwidth over the map window
    public GameObject mapOverlayBeamDirection;
    public GameObject beamDirection;
    public Slider rot1;
    public Slider rot2;

    void Start(){
        meshQuadL = quadL.GetComponent<MeshFilter>();
        originalVerticesQL = meshQuadL.mesh.vertices;

        meshQuadR = quadR.GetComponent<MeshFilter>();
        originalVerticesQR = meshQuadR.mesh.vertices;

        meshQuadC = quadC.GetComponent<MeshFilter>();
        originalVerticesQC = meshQuadC.mesh.vertices;
        VertexAdjustmentBearing();

        meshQuadSpectro = quadSpectro.GetComponent<MeshFilter>();
        originalVerticesQuadSpectro = meshQuadSpectro.mesh.vertices;
        VertexAdjustmentSpectrogram();
        UpdateMapOverlay();
    }

    public void UpdateMapOverlay(){
        rot1.value = userControls.getSonarBeamWidth() / 2;
        rot2.value = userControls.getSonarBeamWidth() / 2;
        mapOverlayBeamDirection.transform.rotation = Quaternion.Euler(0f, 0f, userControls.getBeamRotation());
        beamDirection.transform.rotation = Quaternion.Euler(0f, 0f, userControls.getBeamRotation());
    }

    public void VertexAdjustmentSpectrogram(){
        Vector3[] vertices = meshQuadSpectro.mesh.vertices;
        vertices[0].x = userControls.getMinBandwidth() - 0.5f;
        vertices[2].x = userControls.getMinBandwidth() - 0.5f;

        vertices[1].x = userControls.getMaxBandwidth() - 0.5f;
        vertices[3].x = userControls.getMaxBandwidth() - 0.5f;

        meshQuadSpectro.mesh.vertices = vertices;

        meshQuadSpectro.mesh.RecalculateNormals();
        meshQuadSpectro.mesh.RecalculateBounds();
    }

    //adjusts the rectangle overlays on top of the bearing plot
    public void VertexAdjustmentBearing(){
        //center
        float c = 0f;
        float dist = 0f;
        c = utilities.NormaliseValue(userControls.getBeamRotation() * -1, -180f, 180f);
        dist = utilities.NormaliseValue(userControls.getSonarBeamWidth() / 2, 0f, 360f);

        Vector3[] vertices = meshQuadC.mesh.vertices;
        float l = 0f;
        float r = 0f;
        if(c - dist < 0){
            l = 0f;
        }
        else{
            l = c - dist;
        }

        if(c + dist > 1f){
            r = 1f;
        }
        else{
            r = c + dist;
        }

        vertices[0].x = l - 0.5f;
        vertices[2].x = l - 0.5f;

        vertices[1].x = r - 0.5f;
        vertices[3].x = r - 0.5f;

        meshQuadC.mesh.vertices = vertices;

        meshQuadC.mesh.RecalculateNormals();
        meshQuadC.mesh.RecalculateBounds();

        float leftMost = 0f;
        float rightMost = 0f;

        if(c - dist < 0){
            rightMost = 1f - (dist - c);
        }
        else{
            rightMost = 1f;
        }

        if(c + dist > 1f){
            leftMost = dist - (1f - c);
        }
        else{
            leftMost = 0f;
        }
        vertices = meshQuadL.mesh.vertices;

        vertices[0].x = 0f - 0.5f;
        vertices[2].x = 0f - 0.5f;

        vertices[1].x = leftMost - 0.5f;
        vertices[3].x = leftMost - 0.5f;

        meshQuadL.mesh.vertices = vertices;

        meshQuadL.mesh.RecalculateNormals();
        meshQuadL.mesh.RecalculateBounds();
        
        vertices = meshQuadR.mesh.vertices;
        vertices[0].x = rightMost - 0.5f;
        vertices[2].x = rightMost - 0.5f;

        vertices[1].x = 1f- 0.5f;
        vertices[3].x = 1f- 0.5f;

        meshQuadR.mesh.vertices = vertices;

        meshQuadR.mesh.RecalculateNormals();
        meshQuadR.mesh.RecalculateBounds();
    }

}
