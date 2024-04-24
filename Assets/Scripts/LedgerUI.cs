using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LedgerUI : MonoBehaviour
{
    //Displayed on the UI what the AI think
    public float displayedAIConfidence;
    public string displayedAIClass;

    //Display on the UI what is chosen eiter by user or AI
    public string displayedClass;
    public GameObject displayedClassSymbol;
    public Color userColor;
    public Color AIColor;

    //what is the 
    public string selectedClass;
    public bool AIEstimation;
    
    //references to the text objects which will be updated
    public TextMeshProUGUI textDisplayedClass;
    public TextMeshProUGUI textAIClass;
    public TextMeshProUGUI textAIConfidence;
    public TextMeshProUGUI textCurrentScore;


    //public Button captureShip;
    public Toggle shipToggle;
    public Toggle pirateToggle;
    public Toggle seaLifeToggle;
    public Toggle unknownToggle;
    public Toggle AIEstimationToggle;

    //current selected tracker
    public BearingTrackerBehaviour selectedTracker;

    //
    public PersistentData persistentData;
    public GameObject AITab;
    public GameObject ChooseClassificationTab;

    //handles the toggles
    //! To update description !
    private void shipToggleChanged(bool isOn){
        selectedClass = "ship";
        AIEstimation = false;
        Override();
    }

    private void pirateToggleChanged(bool isOn){
        selectedClass = "pirate";
        AIEstimation = false;
        Override();
    }

    private void seaLifeToggleChanged(bool isOn){
        selectedClass = "seaLife";
        AIEstimation = false;
        Override();
    }

    private void unknownToggleChanged(bool isOn){
        selectedClass = "unknown";
        AIEstimation = false;
        Override();
    }

    private void AIEstimationToggleChanged(bool isOn){
        AIEstimation = true;
        Override();
    }

    public void Override(){
        if(selectedTracker != null){
            if(AIEstimation){
                selectedTracker.UpdateDisplayAI();
            }
            else{
                selectedTracker.UpdateDisplayUser(selectedClass);
            }
        }
    }

    private void Start()
    {
        GameObject persistentDataOBJ = GameObject.FindWithTag("PersistentData");
        persistentData = persistentDataOBJ.GetComponent<PersistentData>();

        //disable the AI Tab based othe scenario file
        if(persistentData.GetCurrentScenarioData().AIType == "noAI"){
            AITab.SetActive(false);
        }

        userColor = Color.yellow;
        AIColor = Color.white;
        AIEstimation = true;
        selectedClass = "unknown";
        shipToggle.onValueChanged.AddListener(shipToggleChanged);
        pirateToggle.onValueChanged.AddListener(pirateToggleChanged);
        seaLifeToggle.onValueChanged.AddListener(seaLifeToggleChanged);
        unknownToggle.onValueChanged.AddListener(unknownToggleChanged);
        AIEstimationToggle.onValueChanged.AddListener(AIEstimationToggleChanged);
        textDisplayedClass.text = "";
        textAIConfidence.text = "";
    }

    //it takes a tracker and sets it as selected
    public void SelectTracker(BearingTrackerBehaviour t){
        ChooseClassificationTab.SetActive(true);
        //turn off the outline of previous selected tracker
        if(selectedTracker != null){
            selectedTracker.ToggleTrackerOutline(false);
            selectedTracker.isTrackerSelected = false;
        }
        t.isTrackerSelected = true;
        /*
        if(!t.isClassChosenByUser){
            AIEstimationToggleChanged(true);
        }
        else{
            string newTrackerClass = t.displayedClass;
            switch (newTrackerClass)
            {
                case "pirate":
                    //Debug.Log()
                    pirateToggleChanged(true);
                    break;
                case "seaLife":
                    seaLifeToggleChanged(true);
                    break;
                case "ship":
                    shipToggleChanged(true);
                    break;
                default:
                    unknownToggleChanged(true);
                    break;
            }
        }
        */

        selectedTracker = t;
        //selectedTracker.isTrackerSelected = true;
        //turn on the outline for the current selected tracker
        selectedTracker.ToggleTrackerOutline(true);
        UpdateDisplayedText();
        UpdateDisplaySymbol();
        UpdateDisplayAI();
    }

    //updates the text of the ledger to match the selected tracker
    public void UpdateDisplayedText(){
        if(selectedTracker != null){
            displayedClass = selectedTracker.displayedClass;
            string newText = "";
            switch (displayedClass)
            {
                case "pirate":
                    newText = "Pirate";
                    break;
                case "seaLife":
                    newText = "Sea Life";;
                    break;
                case "ship":
                    newText = "Ship";
                    break;
                default:
                    newText = "Unknown";
                    break;
            }
            textDisplayedClass.text = newText;
        }
        else{
            textDisplayedClass.text = "";
        }

    }

    public void UpdateDisplayAI(){
        if(selectedTracker != null){
            displayedAIClass = selectedTracker.AIEstimationClass;
            string newText = "";
            switch (displayedAIClass)
            {
                case "pirate":
                    newText = "Pirate";
                    break;
                case "seaLife":
                    newText = "Sea Life";;
                    break;
                case "ship":
                    newText = "Ship";
                    break;
                default:
                    newText = "Unknown";
                    break;
            }

            textAIClass.text = newText;
            textAIConfidence.text = selectedTracker.AIEstimationConfidence.ToString() + "%";
        }
        else{
            textAIClass.text = "";
            textAIConfidence.text = "";
        }

    }

    //The ovrride buttons updates the tracker
    private void ButtonOverrideClicked()
    {
        if(selectedTracker != null){
            if(AIEstimation){
                selectedTracker.UpdateDisplayUser(selectedClass);
            }
            else{
                selectedTracker.UpdateDisplayAI();
            }
        }
    }

    void Update(){
        //if the AI estimation is the one displayed then update the displayed text in the ledger in case it changed
        if(!AIEstimation){
            UpdateDisplayedText();
        }

    }

    public void Unselect(){
        if(selectedTracker != null){
            selectedTracker.ToggleTrackerOutline(false);
            selectedTracker.isTrackerSelected = false;
        }
        selectedTracker = null;
        UpdateDisplayedText();
        UpdateDisplaySymbol();
        UpdateDisplayAI();
        ChooseClassificationTab.SetActive(false);
    }


    public void UpdateDisplaySymbol(){
        SpriteRenderer spriteRenderer = displayedClassSymbol.GetComponent<SpriteRenderer>();
        if(selectedTracker != null){
            spriteRenderer.enabled = true;
            string spritePath = "trackSprites/" + displayedClass;
            Sprite newSprite = Resources.Load<Sprite>(spritePath);
            spriteRenderer.sprite = newSprite;
            if(AIEstimation){
                spriteRenderer.color = AIColor;
            }
            else{
                spriteRenderer.color = userColor;
            }
        }
        else{
            spriteRenderer.color = AIColor;
            spriteRenderer.enabled = false;
        }
    }

    public void UpdateScoreDisplay(){
        textCurrentScore.text = persistentData.GetScore().ToString();
    }

    public void Capture(){
        if(selectedTracker != null){
            Debug.Log(selectedTracker.getRealClass());
            if(selectedTracker.getRealClass() == "pirate"){
                persistentData.UpdateScore(10);
            }
            else{
                persistentData.UpdateScore(-10);
            }
            UpdateScoreDisplay();
            selectedTracker.Capture();
        }
    }
}
