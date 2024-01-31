using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
public class BearingOverlay : MonoBehaviour
{

    public GameObject quadL;
    public GameObject quadR;
    public GameObject quadC;
    MeshFilter meshQuadL;
    MeshFilter meshQuadR;
    MeshFilter meshQuadC;
    Vector3[] originalVerticesQL;
    Vector3[] originalVerticesQR;
    Vector3[] originalVerticesQC;
    public Slider beamWidth;
    public Slider sliderCenter;

    public Slider rot1;

    public Slider rot2;

    private float minDist = 0.01f;

    void Start()
    {
        meshQuadL = quadL.GetComponent<MeshFilter>();
        originalVerticesQL = meshQuadL.mesh.vertices;

        meshQuadR = quadR.GetComponent<MeshFilter>();
        originalVerticesQR = meshQuadR.mesh.vertices;

        meshQuadC = quadC.GetComponent<MeshFilter>();
        originalVerticesQC = meshQuadC.mesh.vertices;
        VertexAdjustment();

        //AddEventTriggerListener(sliderMinFreqBound.gameObject, EventTriggerType.PointerDown, () => OnSliderPressed(1));
        //AddEventTriggerListener(sliderMinFreqBound.gameObject, EventTriggerType.PointerUp, () => OnSliderReleased(1));

        //AddEventTriggerListener(sliderMaxFreqBound.gameObject, EventTriggerType.PointerDown, () => OnSliderPressed(2));
        //AddEventTriggerListener(sliderMaxFreqBound.gameObject, EventTriggerType.PointerUp, () => OnSliderReleased(2));

    }

/*
    void AddEventTriggerListener(GameObject go, EventTriggerType eventType, UnityEngine.Events.UnityAction callback)
    {
        EventTrigger trigger = go.GetComponent<EventTrigger>();
        
        if (trigger == null)
        {
            trigger = go.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback.AddListener((data) => callback.Invoke());
        trigger.triggers.Add(entry);
    }

    void OnSliderPressed(int sliderID)
    {
        if (sliderID == 1)
        {
            isSliderMinFreqPressed = true;
        }
        else if (sliderID == 2)
        {
            isSliderMaxFreqPressed = true;
        }
    }

    void OnSliderReleased(int sliderID)
    {
        if (sliderID == 1)
        {
            isSliderMinFreqPressed = false;
        }
        else if (sliderID == 2)
        {
            isSliderMaxFreqPressed = false;
        }
    }

*/

    void FixedUpdate()
    {
        /*
        if(isSliderMinFreqPressed){
            if((sliderMaxFreqBound.value - sliderMinFreqBound.value < minDist) & (sliderMaxFreqBound.value == 1) ){
                sliderMinFreqBound.value = sliderMaxFreqBound.value - minDist;
            }
            else if(sliderMaxFreqBound.value - sliderMinFreqBound.value < minDist){
                sliderMaxFreqBound.value = sliderMinFreqBound.value + minDist;
            }
        }

        if(isSliderMaxFreqPressed){
            if((sliderMaxFreqBound.value - sliderMinFreqBound.value < minDist) & (sliderMinFreqBound.value == 0) ){
                sliderMaxFreqBound.value = sliderMinFreqBound.value + minDist;
            }
            else if(sliderMaxFreqBound.value - sliderMinFreqBound.value < minDist){
                sliderMinFreqBound.value = sliderMaxFreqBound.value - minDist;
            }
        }
        */
        rot1.value = beamWidth.value;
        rot2.value = beamWidth.value;

        VertexAdjustment();
    }

    private float normaliseSlider(float value){
        float minValue = -180f;
        float maxValue = 180f;
        return (value - minValue) / (maxValue - minValue);
    }

    private float undoNormaliseSlider(float normalizedValue){
        float newMin = 0;
        float newMax = 360;
        return (normalizedValue * (newMax - newMin)) + newMin;
    }

    
    
    //returns half of the width of the scanning area in degrees
    public float getScanningWidth(){
        return undoNormaliseSlider(beamWidth.value * 2);
    }

    void VertexAdjustment ()
    {
        float c = 0f;
        float dist = 0f;
        c = normaliseSlider(sliderCenter.value);
        dist = beamWidth.value;

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

        float ll = 0f;
        float rr = 0f;

        if(c - dist < 0){
            rr = 1f - (dist - c);
        }
        else{
            rr = 1f;
        }

        if(c + dist > 1f){
            ll = dist - (1f - c);
        }
        else{
            ll = 0f;
        }


        vertices = meshQuadL.mesh.vertices;

        
        vertices[0].x = 0f - 0.5f;
        vertices[2].x = 0f - 0.5f;

        vertices[1].x = ll - 0.5f;
        vertices[3].x = ll - 0.5f;

        meshQuadL.mesh.vertices = vertices;

        meshQuadL.mesh.RecalculateNormals();
        meshQuadL.mesh.RecalculateBounds();

        
        vertices = meshQuadR.mesh.vertices;
        vertices[0].x = rr - 0.5f;
        vertices[2].x = rr - 0.5f;

        vertices[1].x = 1f- 0.5f;
        vertices[3].x = 1f- 0.5f;

        meshQuadR.mesh.vertices = vertices;

        meshQuadR.mesh.RecalculateNormals();
        meshQuadR.mesh.RecalculateBounds();
    }
}
