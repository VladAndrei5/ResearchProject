using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearingTrackerBehaviour : MonoBehaviour
{
    public string realClass;
    public AI behaviourAI;
    public float timer = 0f;
    public float timeToReach;
    public SoundSourceBehaviour soundSourcePair;
    public float AIEstimationConfidence;
    public string AIEstimationClass;
    public string userChosenClass;
    public bool isClassChosenByUser;
    public bool isTrackerSelected;
    public string displayedClass;
    public float displayedConfidence;
    private SpriteRenderer spriteRenderer;
    public Sprite newSprite;
    public int counter;
    public Color userColor;
    public Color AIColor;
    public LedgerUI ledgerUI;

    //creates an outline around the selected tracker
    public void ToggleTrackerOutline(bool isEnabled){
        GameObject child = transform.GetChild(0).gameObject;
        child.GetComponent<SpriteRenderer>().enabled = isEnabled;
    }

    //update the shown bearing symbol
    public void UpdateDisplaySymbol(){
        string spritePath = "trackSprites/" + displayedClass;
        newSprite = Resources.Load<Sprite>(spritePath);
        spriteRenderer.sprite = newSprite;
        if(isClassChosenByUser){
            spriteRenderer.color = userColor;
        }
        else{
            spriteRenderer.color = AIColor;
        }
    }

    //AI updates its estimation of the class and confidence
    public void UpdateAIEstimation(){
        AIEstimationClass = behaviourAI.classType[counter];
        AIEstimationConfidence = behaviourAI.confidence[counter];
    }

    //Updates the visuals to match the AI classification
    public void UpdateDisplayAI(){
        isClassChosenByUser = false;
        displayedClass = AIEstimationClass;
        displayedConfidence = AIEstimationConfidence;
        UpdateDisplaySymbol();
        if(isTrackerSelected){
            ledgerUI.UpdateDisplayedText();
        }
    }

    //Updates the visuals to match the user classification
    public void UpdateDisplayUser(string selectedClass){
        isClassChosenByUser = true;
        displayedClass = selectedClass;
        displayedConfidence = 100;
        UpdateDisplaySymbol();
        if(isTrackerSelected){
            ledgerUI.UpdateDisplayedText();
        }
    }

    //initialises the tracker when it is first spawned in
    public void InitaliseBehaviour(string realClass, AI behaviourAI, SoundSourceBehaviour audioSource){

        //these variables never change
        this.behaviourAI = behaviourAI;
        this.realClass = realClass;
        this.soundSourcePair = audioSource;

        //keeps track if the user overrides the AI estimation
        isClassChosenByUser = false;
        isTrackerSelected = false;

        //set up sprite renderer and colours of trackers
        spriteRenderer = GetComponent<SpriteRenderer>();
        userColor = Color.yellow;
        AIColor = Color.black;

        //create ledger reference
        GameObject ledgerOBJ = GameObject.FindWithTag("UI");
        ledgerUI = ledgerOBJ.GetComponent<LedgerUI>();

        counter = 0;
        UpdateAIEstimation();
    }
    void Start()
    {
        if(behaviourAI != null){
            timer = 0f;
            counter = 0;
        }
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Change 0 to 1 for right mouse button, 2 for middle mouse button
        {
            // Cast a ray from the mouse position into the scene
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // Check if the ray hits any collider
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the hit collider belongs to this GameObject
                if (hit.collider.gameObject == gameObject)
                {
                    //if the tracker was pressed, then it tells the ledger that this script instance is the one selected
                    ledgerUI.SelectTracker(this);
                }
            }
        }
        StartCoroutine(UpdateTrackBehaviour());
    }
    
    
    //the tracker continues to update new estimations when the time is correct
    private IEnumerator UpdateTrackBehaviour()
    {
        timeToReach = behaviourAI.time[counter];
        if(timer <= timeToReach){
            timer += Time.deltaTime; // Increment the timer
            yield return null; // Wait for the next frame
        }
        else{
            UpdateAIEstimation();
            if(counter < behaviourAI.time.Length - 1){
                counter++;
            }

            if(!isClassChosenByUser){
                UpdateDisplayAI();
            }
        }
        
        
    }
    
}