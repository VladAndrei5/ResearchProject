using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabManager : MonoBehaviour
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
    public bool isAIestimationOn;
    
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
        Debug.Log("Ship1");
        if(!isOn){
            return;
        }
        if(!selectedTracker.isProducingSound){
            return;
        }
        Debug.Log("Ship2");
        selectedClass = "ship";
        isAIestimationOn = false;
        Override();
    }

    private void pirateToggleChanged(bool isOn){
        if(!isOn){
            return;
        }
        if(!selectedTracker.isProducingSound){
            return;
        }
        selectedClass = "pirate";
        isAIestimationOn = false;
        Override();
    }

    private void seaLifeToggleChanged(bool isOn){
        if(!isOn){
            return;
        }
        if(!selectedTracker.isProducingSound){
            return;
        }
        selectedClass = "seaLife";
        isAIestimationOn = false;
        Override();
    }

    private void unknownToggleChanged(bool isOn){
        if(!isOn){
            return;
        }
        if(!selectedTracker.isProducingSound){
            return;
        }
        selectedClass = "unknown";
        isAIestimationOn = false;
        Override();
    }

    private void AIEstimationToggleChanged(bool isOn){
        if(!isOn){
            return;
        }
        if(!selectedTracker.isProducingSound){
            return;
        }
        isAIestimationOn = true;
        Override();
    }

    public void Override(){
        if(selectedTracker != null){
            if(isAIestimationOn){
                selectedTracker.DisplayAIEstimation();
            }
            else{
                selectedTracker.DisplayUserClass(selectedClass);
            }
        }
    }

    public void SetToggleOn(Toggle toggle){
        toggle.isOn = true;
    }

    private void Start()
    {
        GameObject persistentDataOBJ = GameObject.FindWithTag("PersistentData");
        persistentData = persistentDataOBJ.GetComponent<PersistentData>();

        //disable the AI Tab based othe scenario file
        if(persistentData.AIType == "none"){
            AITab.SetActive(false);
        }

        userColor = Color.yellow;
        AIColor = Color.white;
        isAIestimationOn = true;
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
        Debug.Log("Selected2");
        if(t.isProducingSound == false){
            return;
        }
        //turn off the outline of previous selected tracker
        if(selectedTracker != null){
            selectedTracker.ToggleTrackerOutline(false);
            selectedTracker.isTrackerSelected = false;
            Unselect();
        }

        ChooseClassificationTab.SetActive(true);
        selectedTracker = t;

        selectedTracker.isTrackerSelected = true;

        if(selectedTracker.isAIactive){
            SetToggleOn(AIEstimationToggle);
            isAIestimationOn = true;
        }
        else{
            string newTrackerClass = selectedTracker.displayedClass;
            switch (newTrackerClass)
            {
                case "pirate":
                    SetToggleOn(pirateToggle);
                    break;
                case "seaLife":
                    SetToggleOn(seaLifeToggle);
                    break;
                case "ship":
                    SetToggleOn(shipToggle);
                    break;
                case "unknown":
                    SetToggleOn(unknownToggle);
                    break;    
                default:
                    break;
            }
        }
        //selectedTracker.isTrackerSelected = true;
        //turn on the outline for the current selected tracker
        selectedTracker.ToggleTrackerOutline(true);
        UpdateLedgerSelectedClassText();
        UpdateLedgerSelectedClassSymbol();
        UpdateAITab();
    }

    //updates the text of the ledger to match the selected tracker
    public void UpdateLedgerSelectedClassText(){
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
                case "none":
                    newText = "none";
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

    public void UpdateAITab(){
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
                case "none":
                    newText = "none";
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
            if(isAIestimationOn){
                selectedTracker.DisplayUserClass(selectedClass);
            }
            else{
                selectedTracker.DisplayAIEstimation();
            }
        }
    }

    void Update(){
        if(selectedTracker == null){
            Unselect();
        }
        //if the AI estimation is the one displayed then update the displayed text in the ledger in case it changed
        /*
        if(selectedTracker != null){
            if(isAIestimationOn){
                UpdateLedgerSelectedClassText();
                UpdateLedgerSelectedClassSymbol();
            }
        }
        */

    }

    public void Unselect(){
        if(selectedTracker != null){
            selectedTracker.ToggleTrackerOutline(false);
            selectedTracker.isTrackerSelected = false;
        }
        selectedTracker = null;
        UpdateLedgerSelectedClassText();
        UpdateLedgerSelectedClassSymbol();
        UpdateAITab();
        ChooseClassificationTab.SetActive(false);
    }


    public void UpdateLedgerSelectedClassSymbol(){
        SpriteRenderer spriteRenderer = displayedClassSymbol.GetComponent<SpriteRenderer>();
        if(selectedTracker != null){
            spriteRenderer.enabled = true;
            string spritePath = "trackSprites/" + displayedClass;
            Sprite newSprite = Resources.Load<Sprite>(spritePath);
            spriteRenderer.sprite = newSprite;
            if(isAIestimationOn){
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
        if(selectedTracker == null){
            return;
        }

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
