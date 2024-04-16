using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Audio;
using Random=UnityEngine.Random;
using System.IO;

public class UserControls : MonoBehaviour
{


    //references to some sliders
    public Slider sliderMinBandwidth;
    public Slider sliderMaxBandwidth;
    public Slider sliderBeamRot;
    public Slider sliderBeamWidth;
    //references to some classes
    public Overlay overlay;

    //minimum distance betweenSliders
    private float minDistBetweenSlidersSpectrogram = 0.01f;

    private float sonarHalfOfBeamWidth;

    //check if the sliders are interactedWith
    private bool isBeamWidthInteractable;
    private bool isBeamRotInteractable;

    public GameObject beamWidthHandle;
    public GameObject beamRotHandle;




    void Start(){
        sliderMinBandwidth.onValueChanged.AddListener(SliderMinBandwithChange);
        sliderMaxBandwidth.onValueChanged.AddListener(SliderMaxBandwithChange);
        sliderBeamRot.onValueChanged.AddListener(SliderBeamRotChange);
        sliderBeamWidth.onValueChanged.AddListener(SliderBeamWidthChange);
        isBeamWidthInteractable = true;
        isBeamRotInteractable = true;
        UpdateSonarBeamWidth();
    }

    public float getSonarBeamWidth(){
        return sonarHalfOfBeamWidth * 2f;
        //return sliderBeamWidth.value;
    }

    public float getMinBandwidth(){
        return sliderMinBandwidth.value;
    }

    public float getMaxBandwidth(){
        return sliderMaxBandwidth.value;
    }

    public float getBeamRotation(){
        return -1 * sliderBeamRot.value;
    }

     private void SliderBeamRotChange(float value)
    {
        if (isBeamRotInteractable){
            //Debug.Log("ROT");
            isBeamWidthInteractable = false;

            if(sliderBeamRot.value - sonarHalfOfBeamWidth >= -180f){
                sliderBeamWidth.value = sliderBeamRot.value - sonarHalfOfBeamWidth;
            }
            else{
                sliderBeamWidth.value = 180f - (sonarHalfOfBeamWidth - (float)Math.Abs(sliderBeamRot.value - (-180f)) );
            }

            overlay.VertexAdjustmentBearing();
            overlay.UpdateMapOverlay();
            isBeamWidthInteractable = true;
        }

    }

    private void SliderBeamWidthChange(float value)
    {   
        if (isBeamWidthInteractable){
            //Debug.Log("WIDTH");
            isBeamRotInteractable = false;

            if(sonarHalfOfBeamWidth > 180){
                sonarHalfOfBeamWidth = 179;
                if(sliderBeamRot.value - sonarHalfOfBeamWidth >= -180f){
                    sliderBeamWidth.value = sliderBeamRot.value - sonarHalfOfBeamWidth;
                }
                else{
                    sliderBeamWidth.value = 180f - (sonarHalfOfBeamWidth - (float)Math.Abs(sliderBeamRot.value - (-180f)) );
                }
            }
            else if(sonarHalfOfBeamWidth < 1){
                sonarHalfOfBeamWidth = 1;
                if(sliderBeamRot.value - sonarHalfOfBeamWidth >= -180f){
                    sliderBeamWidth.value = sliderBeamRot.value - sonarHalfOfBeamWidth;
                }
                else{
                    sliderBeamWidth.value = 180f - (sonarHalfOfBeamWidth - (float)Math.Abs(sliderBeamRot.value - (-180f)) );
                }
            }

            UpdateSonarBeamWidth();
            overlay.VertexAdjustmentBearing();
            overlay.UpdateMapOverlay();
            isBeamRotInteractable = true;
        }

    }

    private void UpdateSonarBeamWidth(){
        if(sliderBeamWidth.value <= sliderBeamRot.value){
            sonarHalfOfBeamWidth = (float)Math.Abs(sliderBeamRot.value - sliderBeamWidth.value);
        }
        else{
            sonarHalfOfBeamWidth = (float)Math.Abs(sliderBeamRot.value - (-180f)) + (float)Math.Abs(180f - sliderBeamWidth.value);
        }
        //Debug.Log("Current sonar beam width: " + sonarHalfOfBeamWidth);
    }

    /*
     private void SliderBeamWidthChange(float value)
    {
        if(sliderBeamWidth.value < 1){
            sliderBeamWidth.value = 1;
        }
        overlay.VertexAdjustmentBearing();
        overlay.UpdateMapOverlay();
    }
    */

    private void SliderMinBandwithChange(float value)
    {
        overlay.VertexAdjustmentSpectrogram();

        //makes sure that the minimum slider stays below the maximum one
        if((sliderMaxBandwidth.value - sliderMinBandwidth.value < minDistBetweenSlidersSpectrogram) & (sliderMaxBandwidth.value == 1) ){
            sliderMinBandwidth.value = sliderMaxBandwidth.value - minDistBetweenSlidersSpectrogram;
        }
        else if(sliderMaxBandwidth.value - sliderMinBandwidth.value < minDistBetweenSlidersSpectrogram){
            sliderMaxBandwidth.value = sliderMinBandwidth.value + minDistBetweenSlidersSpectrogram;
        }
    }

    private void SliderMaxBandwithChange(float value)
    {
        overlay.VertexAdjustmentSpectrogram();

        //makes sure that the minimum slider stays below the maximum one
        if((sliderMaxBandwidth.value - sliderMinBandwidth.value < minDistBetweenSlidersSpectrogram) & (sliderMinBandwidth.value == 0) ){
            sliderMaxBandwidth.value = sliderMinBandwidth.value + minDistBetweenSlidersSpectrogram;
        }
        else if(sliderMaxBandwidth.value - sliderMinBandwidth.value < minDistBetweenSlidersSpectrogram){
            sliderMinBandwidth.value = sliderMaxBandwidth.value - minDistBetweenSlidersSpectrogram;
        }
        
    }

}
