using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoomControls : MonoBehaviour
{
    //Cameras used to zoom in
    public Camera mainCamera;
    public Camera trackersCamera;
    public Camera spectrogramCamera;
    public Camera bearingCamera;
    public Camera mapCamera;

    //Zoom in buttons
    public Button trackersCameraButton;
    public Button spectrogramCameraButton;
    public Button bearingCameraButton;
    public Button mapCameraButton;

    //Zoom out buttons
    public Button trackersCameraButtonOut;
    public Button spectrogramCameraButtonOut;
    public Button bearingCameraButtonOut;
    public Button mapCameraButtonOut;

    void Start()
    {
        // Ensure the button has a click listener
        trackersCameraButton.onClick.AddListener(SwitchTrackersCamera);
        spectrogramCameraButton.onClick.AddListener(SwitchSpectrogramCamera);
        bearingCameraButton.onClick.AddListener(SwitchBearingCamera);
        mapCameraButton.onClick.AddListener(SwitchMapCamera);

        trackersCameraButtonOut.onClick.AddListener(SwitchMainCamera);
        spectrogramCameraButtonOut.onClick.AddListener(SwitchMainCamera);
        bearingCameraButtonOut.onClick.AddListener(SwitchMainCamera);
        mapCameraButtonOut.onClick.AddListener(SwitchMainCamera);

        // Start with camera1 active
        mainCamera.enabled = true;
        trackersCamera.enabled = false;
        spectrogramCamera.enabled = false;
        bearingCamera.enabled = false;
        mapCamera.enabled = false;

        DisableZoomOutButtons();
    }

    void TurnOnCamera(Camera cameraToTurnOn)
    {
        // Toggle the enabled state of both cameras
        mainCamera.enabled = false;
        trackersCamera.enabled = false;
        spectrogramCamera.enabled = false;
        bearingCamera.enabled = false;
        mapCamera.enabled = false;

        cameraToTurnOn.enabled = true;
        //Debug.Log("Hello2");
    }

    void DisableZoomInButtons(){
        trackersCameraButton.interactable = false;
        Image buttonImage = trackersCameraButton.GetComponent<Image>();
        buttonImage.enabled = false;

        spectrogramCameraButton.interactable = false;
        buttonImage = spectrogramCameraButton.GetComponent<Image>();
        buttonImage.enabled = false;

        bearingCameraButton.interactable = false;
        buttonImage = bearingCameraButton.GetComponent<Image>();
        buttonImage.enabled = false;

        mapCameraButton.interactable = false;
        buttonImage = mapCameraButton.GetComponent<Image>();
        buttonImage.enabled = false;

        //++

        trackersCameraButtonOut.interactable = true;
        buttonImage = trackersCameraButtonOut.GetComponent<Image>();
        buttonImage.enabled = true;

        spectrogramCameraButtonOut.interactable = true;
        buttonImage = spectrogramCameraButtonOut.GetComponent<Image>();
        buttonImage.enabled = true;

        bearingCameraButtonOut.interactable = true;
        buttonImage = bearingCameraButtonOut.GetComponent<Image>();
        buttonImage.enabled = true;

        mapCameraButtonOut.interactable = true;
        buttonImage = mapCameraButtonOut.GetComponent<Image>();
        buttonImage.enabled = true;

    }

    void DisableZoomOutButtons(){
        trackersCameraButton.interactable = true;
        Image buttonImage = trackersCameraButton.GetComponent<Image>();
        buttonImage.enabled = true;

        spectrogramCameraButton.interactable = true;
        buttonImage = spectrogramCameraButton.GetComponent<Image>();
        buttonImage.enabled = true;

        bearingCameraButton.interactable = true;
        buttonImage = bearingCameraButton.GetComponent<Image>();
        buttonImage.enabled = true;

        mapCameraButton.interactable = true;
        buttonImage = mapCameraButton.GetComponent<Image>();
        buttonImage.enabled = true;

        //++

        trackersCameraButtonOut.interactable = false;
        buttonImage = trackersCameraButtonOut.GetComponent<Image>();
        buttonImage.enabled = false;

        spectrogramCameraButtonOut.interactable = false;
        buttonImage = spectrogramCameraButtonOut.GetComponent<Image>();
        buttonImage.enabled = false;

        bearingCameraButtonOut.interactable = false;
        buttonImage = bearingCameraButtonOut.GetComponent<Image>();
        buttonImage.enabled = false;

        mapCameraButtonOut.interactable = false;
        buttonImage = mapCameraButtonOut.GetComponent<Image>();
        buttonImage.enabled = false;
    }

    void SwitchTrackersCamera()
    {
        //Debug.Log("Hello");
        TurnOnCamera(trackersCamera);
        DisableZoomInButtons();
    }

    void SwitchSpectrogramCamera()
    {
        TurnOnCamera(spectrogramCamera);
        DisableZoomInButtons();
    }

    void SwitchBearingCamera()
    {
        TurnOnCamera(bearingCamera);
        DisableZoomInButtons();
    }

    void SwitchMapCamera()
    {
        TurnOnCamera(mapCamera);
        DisableZoomInButtons();
    }

    void SwitchMainCamera()
    {
        //Debug.Log("Hellodad");
        TurnOnCamera(mainCamera);
        DisableZoomOutButtons();
    }
    
}
