using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class OverlaySpectrogram : MonoBehaviour
{
    MeshFilter meshFilter;
    Vector3[] originalVertices;

    public Slider sliderMinFreqBound;
    public Slider sliderMaxFreqBound;

    private bool isSliderMinFreqPressed = false;
    private bool isSliderMaxFreqPressed = false;

    private float minDist = 0.01f;

    public GameLogic gameLogic;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        originalVertices = meshFilter.mesh.vertices;
        VertexAdjustment();

        AddEventTriggerListener(sliderMinFreqBound.gameObject, EventTriggerType.PointerDown, () => OnSliderPressed(1));
        AddEventTriggerListener(sliderMinFreqBound.gameObject, EventTriggerType.PointerUp, () => OnSliderReleased(1));

        AddEventTriggerListener(sliderMaxFreqBound.gameObject, EventTriggerType.PointerDown, () => OnSliderPressed(2));
        AddEventTriggerListener(sliderMaxFreqBound.gameObject, EventTriggerType.PointerUp, () => OnSliderReleased(2));

    }

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


    void Update()
    {

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


        VertexAdjustment();
    }

    void VertexAdjustment ()
    {
        Vector3[] vertices = meshFilter.mesh.vertices;
        vertices[0].x = sliderMinFreqBound.value - 0.5f;
        vertices[2].x = sliderMinFreqBound.value - 0.5f;

        vertices[1].x = sliderMaxFreqBound.value - 0.5f;
        vertices[3].x = sliderMaxFreqBound.value - 0.5f;

        meshFilter.mesh.vertices = vertices;

        meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.RecalculateBounds();
    }
}
