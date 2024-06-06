using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearingTrackerBehaviour : MonoBehaviour
{


    //strings
    public string realClass;
    public string AIEstimationClass;
    public string userClass;
    public string displayedClass;
    public string id;

    //float
    public float timer = 0f;
    public float isProducingSoundProgressTimer = 0f;
    public float timeToReach;
    public float AIEstimationConfidence;
    public float totalVolume;
    public int counter;

    //flags
    public bool isTrackerSelected;
    public bool isTrackerCaptured;
    public bool isAIactive;
    public bool isSoundSourceActive;
    public bool isTrackerAvailable;
    public bool isProducingSound;
    public bool isProducingSoundProgress;

    //other
    private SpriteRenderer spriteRenderer;
    public Sprite newSprite;
    public Color userColor;
    public Color AIColor;


    //references to classes
    public LedgerUI ledgerUI;
    public Utilities utilities;
    public SoundSourceBehaviour soundSourcePair;
    public AI behaviourAI;


    public void ToggleTrackerOutline(bool isEnabled){
        GameObject child = transform.GetChild(0).gameObject;
        child.GetComponent<SpriteRenderer>().enabled = isEnabled;
    }

    public void InitaliseBehaviour(string realClass, AI behaviourAI, SoundSourceBehaviour audioSource, string id){

        //
        GameObject obj = GameObject.FindWithTag("Utilities");
        utilities = obj.GetComponent<Utilities>();

        //these variables never change
        this.behaviourAI = behaviourAI;
        this.realClass = realClass;
        this.soundSourcePair = audioSource;
        this.id = id;
        //this.isAIactive = isAIactive;

        //keeps track if the user overrides the AI estimation
        isTrackerSelected = false;
        isSoundSourceActive = true;
        isAIactive = true;
        isProducingSound = false;
        isProducingSoundProgress = false;

        //set up sprite renderer and colours of trackers
        spriteRenderer = GetComponent<SpriteRenderer>();
        userColor = Color.yellow;
        AIColor = Color.white;

        //create ledger reference
        GameObject ledgerOBJ = GameObject.FindWithTag("LedgerUI");
        ledgerUI = ledgerOBJ.GetComponent<LedgerUI>();

        counter = 0;
        UpdateAIEstimation();
        //StartCoroutine(UpdateVisibility(soundSourcePair.getGameObject()));
    }
    void Start()
    {
        if(behaviourAI != null){
            timer = 0f;
            counter = 0;
        }

    }

     void Update(){
        //check if the tracker is clicked and inform the TabManager
        if (Input.GetMouseButtonDown(0)) // if mouse is down
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
                    if(isSoundSourceActive && isProducingSound){
                        Debug.Log("Pressed");
                        //if the tracker was pressed, then it tells the ledger that this script instance is the one selected
                        ledgerUI.SelectTracker(this);
                        isTrackerSelected = true;
                    }
                }
            }
        }

        StartCoroutine(CheckAIUpdates());
        StartCoroutine(CheckSoundLevels());
        StartCoroutine(CheckVisibility());
        UpdatePosition(soundSourcePair.getGameObject()); // always update its position
    }

    private IEnumerator CheckVisibility(){
        if(isProducingSound && isSoundSourceActive && !isTrackerCaptured && displayedClass != "none" ){
            TurnOnTracker();
        }
        else{
            TurnOffTracker();
        }
        yield return null;

    }

    private IEnumerator CheckSoundLevels(){
        //checking the dB levles
        float[] spectrumAudioSource = new float[64];
        spectrumAudioSource = soundSourcePair.RetrieveSpectrumData();
        float totalAmplitude = 0f;
        for(int i = 0; i < 64; i++){
            totalAmplitude = spectrumAudioSource[i] + totalAmplitude;
        }
        totalVolume = utilities.convertToDecebels(totalAmplitude);

        //creating the flip
        if(isProducingSound){
            if(totalVolume < -60f){
                isProducingSoundProgressTimer += Time.deltaTime;
                if(isProducingSoundProgressTimer >= 2f){
                    isProducingSound = false;
                    isProducingSoundProgressTimer = 0f;
                }
                //TurnOffTracker();
            }
        }
        else{
            if(totalVolume > -55f){
                isProducingSoundProgressTimer += Time.deltaTime;
                if(isProducingSoundProgressTimer >= 2f){
                    isProducingSound = true;
                    isProducingSoundProgressTimer = 0f;
                }
                //TurnOnTracker();
            }
        }
        
        yield return null;

    }

    private void TurnOffTracker()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        if(id == "2"){
            Debug.Log("off");
        }

    }

    private void TurnOnTracker()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        if(id == "2"){
            Debug.Log("on");
        }
    }




    public void UpdatePosition(GameObject soundSource){
        float soundRot = utilities.getSoundSourceAngle(soundSource);
        soundRot = utilities.Remap(soundRot, -180f, 180f, -50f, 50f);

        Vector3 currentPosition = transform.position;
        transform.localPosition = new Vector3(soundRot * -1f, currentPosition.y, currentPosition.z);
    }


    private IEnumerator CheckAIUpdates(){
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

            if(isAIactive){
                DisplayAIEstimation();
            }
        }
    }

    public void UpdateAIEstimation(){
        AIEstimationClass = behaviourAI.classType[counter];
        AIEstimationConfidence = behaviourAI.confidence[counter];
    }

    public void DisplayAIEstimation(){
        isAIactive = true;
        displayedClass = AIEstimationClass;
        UpdateDisplaySymbol();

        if(displayedClass == "none"){
            return;
        }
        
        if(isTrackerSelected){
            ledgerUI.UpdateLedgerSelectedClassText();
            ledgerUI.UpdateLedgerSelectedClassSymbol();
            ledgerUI.UpdateAITab();
        }
        

    }

    public void DisplayUserClass(string selectedClass){
        isAIactive = false;
        displayedClass = selectedClass;
        
        UpdateDisplaySymbol();
        
        if(isTrackerSelected){
            ledgerUI.UpdateLedgerSelectedClassText();
            ledgerUI.UpdateLedgerSelectedClassSymbol();
            ledgerUI.UpdateAITab();
        }
        
    }

    //update the shown bearing symbol
    public void UpdateDisplaySymbol(){

        if(!isSoundSourceActive){
            return;
        }

        if(displayedClass == "none"){
            return;
        }
        
        spriteRenderer.enabled = true;
        string spritePath = "trackSprites/" + displayedClass;
        newSprite = Resources.Load<Sprite>(spritePath);
        spriteRenderer.sprite = newSprite;
        if(isAIactive){
            spriteRenderer.color = AIColor;
        }
        else{
            spriteRenderer.color = userColor;
        }
        
    }

    public void Capture(){
        if(isSoundSourceActive){
            if(isTrackerSelected){
                ledgerUI.Unselect();
                isTrackerSelected = false;
            }
            float timeCaptured = 25f;

            soundSourcePair.Capture(timeCaptured);

            StartCoroutine(Hide(timeCaptured));
        }
    }

    private IEnumerator Hide(float timeCaptured)
    {
        isSoundSourceActive = false;
        GetComponent<SpriteRenderer>().enabled = false;

        yield return new WaitForSeconds(timeCaptured);

        GetComponent<SpriteRenderer>().enabled = true;
        isSoundSourceActive = true;

        isAIactive = true;
        DisplayAIEstimation();
        //CheckVisibility(soundSourcePair.getGameObject());
    }
    
    public string getRealClass(){
        return realClass;
    }


    /*

    [SerializeField]
    public float[] spectrumAudioSource;

    //creates an outline around the selected tracker
    public void ToggleTrackerOutline(bool isEnabled){
        GameObject child = transform.GetChild(0).gameObject;
        child.GetComponent<SpriteRenderer>().enabled = isEnabled;
    }

    //update the shown bearing symbol
    public void UpdateDisplaySymbol(){
        if(displayedClass == "none"){
            GetComponent<SpriteRenderer>().enabled = false;
            return;
        }

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
    
        if(displayedClass == "none"){
            GetComponent<SpriteRenderer>().enabled = false;
            return;
        }

        //displayedConfidence = AIEstimationConfidence;
        UpdateDisplaySymbol();
        if(isTrackerSelected){
            ledgerUI.UpdateDisplayedText();
            ledgerUI.UpdateDisplaySymbol();
            ledgerUI.UpdateDisplayAI();
        }
    }

    //Updates the visuals to match the user classification
    public void UpdateDisplayUser(string selectedClass){
        Debug.Log("User");
        Debug.Log(selectedClass);
        isClassChosenByUser = true;
        displayedClass = selectedClass;
        //displayedConfidence = AIEstimationConfidence;
        UpdateDisplaySymbol();
        if(isTrackerSelected){
            ledgerUI.UpdateDisplayedText();
            ledgerUI.UpdateDisplaySymbol();
            ledgerUI.UpdateDisplayAI();
        }
    }

    //initialises the tracker when it is first spawned in
    public void InitaliseBehaviour(string realClass, AI behaviourAI, SoundSourceBehaviour audioSource){

        //
        GameObject obj = GameObject.FindWithTag("Utilities");
        utilities = obj.GetComponent<Utilities>();

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
        AIColor = Color.white;

        //create ledger reference
        GameObject ledgerOBJ = GameObject.FindWithTag("LedgerUI");
        ledgerUI = ledgerOBJ.GetComponent<LedgerUI>();

        counter = 0;
        UpdateAIEstimation();
        //StartCoroutine(UpdateVisibility(soundSourcePair.getGameObject()));
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
        UpdatePosition(soundSourcePair.getGameObject());
    }
    
    private IEnumerator UpdateVisibility(GameObject soundSource){
        while(true){
            CheckVisibility(soundSource);
            yield return null;
        }
    }

    private void CheckVisibility(GameObject soundSource){
        spectrumAudioSource = new float[64];
        soundSource.GetComponent<AudioSource>().GetSpectrumData(spectrumAudioSource, 0, FFTWindow.BlackmanHarris);
        float highestApmlitude = 0f;
        for (int j = 0; j < spectrumAudioSource.Length / 2; j++){
            if(highestApmlitude < spectrumAudioSource[j]){
                highestApmlitude =spectrumAudioSource[j];
                spectrumAudioSource[j] = 0f;
            }
        }
        //Debug.Log(highestApmlitude);
        //Debug.Log(utilities.normaliseSoundDecebels(utilities.convertToDecebels(highestApmlitude)));
        if( utilities.normaliseSoundDecebels(utilities.convertToDecebels(highestApmlitude))  < 0.24){
            GetComponent<SpriteRenderer>().enabled = false;
            if(isTrackerSelected){
                ledgerUI.Unselect();
            }
        }
        else{
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }
    
    public void UpdatePosition(GameObject soundSource){
        float soundRot = utilities.getSoundSourceAngle(soundSource);
        soundRot = utilities.Remap(soundRot, -180f, 180f, -50f, 50f);

        Vector3 currentPosition = transform.position;
        transform.localPosition = new Vector3(soundRot * -1f, currentPosition.y, currentPosition.z);
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

    public void Capture(){
        if(GetComponent<SpriteRenderer>().enabled == true){
            if(isTrackerSelected){
                ledgerUI.Unselect();
            }
            float timeCaptured = 25f;
            soundSourcePair.Capture(timeCaptured);
            StartCoroutine(Hide(timeCaptured));
            isClassChosenByUser = false;
             UpdateDisplayAI();
        }
    }

    private IEnumerator Hide(float timeCaptured)
    {
        GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(timeCaptured);
        GetComponent<SpriteRenderer>().enabled = true;
        //CheckVisibility(soundSourcePair.getGameObject());

    }
    
    public string getRealClass(){
        return realClass;
    }

    */
}
