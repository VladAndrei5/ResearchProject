using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LedgerUI : MonoBehaviour
{
    public float displayedConfidence;
    public string displayedClass;
    public string selectedClass;
    public bool isChosenByUser;

    public TextMeshProUGUI textClass;
    public TextMeshProUGUI textConfidence;

    public Button buttonOverrideAI;
    public Toggle shipToggle;
    public Toggle pirateToggle;
    public Toggle mammalToggle;
    public Toggle AIEstimationToggle;

    public BearingTrackerBehaviour selectedTracker;

    //handles the toggles
    //! To update description !
    private void shipToggleChanged(bool isOn){
        selectedClass = "ship";
        isChosenByUser = true;
    }

    private void pirateToggleChanged(bool isOn){
        selectedClass = "pirate";
        isChosenByUser = true;
    }

    private void mammalToggleChanged(bool isOn){
        selectedClass = "mammal";
        isChosenByUser = true;
    }

    private void AIEstimationToggleChanged(bool isOn){
        isChosenByUser = false;
    }

    private void Start()
    {
        isChosenByUser = false;
        selectedClass = "AI";
        buttonOverrideAI.onClick.AddListener(ButtonOverrideClicked);
        shipToggle.onValueChanged.AddListener(shipToggleChanged);
        pirateToggle.onValueChanged.AddListener(pirateToggleChanged);
        mammalToggle.onValueChanged.AddListener(mammalToggleChanged);
        AIEstimationToggle.onValueChanged.AddListener(AIEstimationToggleChanged);
        textClass.text = "";
        textConfidence.text = "";
    }

    //it takes a tracker and sets it as selected
    public void SelectTracker(BearingTrackerBehaviour t){
        //turn off the outline of previous selected tracker
        if(selectedTracker != null){
            selectedTracker.ToggleTrackerOutline(false);
            selectedTracker.isTrackerSelected = false;
        }
        selectedTracker = t;
        selectedTracker.isTrackerSelected = true;
        //turn on the outline for the current selected tracker
        selectedTracker.ToggleTrackerOutline(true);
        UpdateDisplayedText();
    }

    //updates the text of the ledger to match the selected tracker
    public void UpdateDisplayedText(){
        if(selectedTracker != null){
            textClass.text = selectedTracker.displayedClass;
            textConfidence.text = selectedTracker.displayedConfidence.ToString();
        }

    }

    //The ovrride buttons updates the tracker
    private void ButtonOverrideClicked()
    {
        if(selectedTracker != null){
            if(isChosenByUser){
                selectedTracker.UpdateDisplayUser(selectedClass);
            }
            else{
                selectedTracker.UpdateDisplayAI();
            }
        }
    }

    void Update(){
        //if the AI estimation is the one displayed then update the displayed text in the ledger in case it changed
        if(!isChosenByUser){
            UpdateDisplayedText();
        }

    }
}
