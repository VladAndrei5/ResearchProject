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
    public float despawnTimer;
    public float isProducingSoundProgressTimer = 0f;
    public float timerAIConfidence1;
    public float timerAIConfidence2;
    public float timerAIChangeEstimation;
    public float AIEstimationConfidence;
    public float previousAIEstimationConfidence;
    public float futureAIEstimationConfidence;
    public float totalVolume;
    public int counter;

    public float upperConfidenceLimit = 50f;
    public float lowerConfidenceLimit = 40f;

    //flags
    public bool isTrackerSelected;
    public bool isAIactive;
    public bool isTrackerAvailable;
    public bool isProducingSound;
    public bool isProducingSoundProgress;
    

    //other
    private SpriteRenderer spriteRenderer;
    public Sprite newSprite;
    public Color colorUser;
    public Color colorAI;
    public Color colorGrayedOut;

    //references to classes
    public TabManager tabManager;
    public Utilities utilities;
    public SoundSourceBehaviour soundSourcePair;
    public PersistentData persistentData;
    public EntityManager entityManager;

    public int IDCounter;


    public void CheckStressLevels(){
        float stress = 70f;
        if((stress>50) && (AIEstimationConfidence < lowerConfidenceLimit)){
            Color color = spriteRenderer.color;
            color.a = 0.1f;
            spriteRenderer.color = color;
        }
        else{
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }

    }

    public void ToggleTrackerOutline(bool isEnabled){
        GameObject child = transform.GetChild(0).gameObject;
        child.GetComponent<SpriteRenderer>().enabled = isEnabled;
    }

    public void InitaliseBehaviour(string actualClass, SoundSourceBehaviour audioSource, int IDCount){

        //create references
        GameObject obj = GameObject.FindWithTag("Utilities");
        utilities = obj.GetComponent<Utilities>();
        GameObject obj2 = GameObject.FindWithTag("PersistentData");
        persistentData = obj2.GetComponent<PersistentData>();
        GameObject obj3 = GameObject.FindWithTag("EntityManager");
        entityManager = obj3.GetComponent<EntityManager>();

        //these variables never change
        realClass = actualClass;
        this.soundSourcePair = audioSource;
        IDCounter = IDCount;

        //keeps track if the user overrides the AI estimation
        isTrackerSelected = false;
        

        isAIactive = true;
        isProducingSound = false;
        isProducingSoundProgress = false;


        //set up sprite renderer and colours of trackers
        spriteRenderer = GetComponent<SpriteRenderer>();
        colorUser = Color.yellow;
        colorAI = Color.white;
        colorGrayedOut = Color.grey;

        timerAIConfidence1 = 0f;
        timerAIConfidence2 = 0f;

        //create ledger reference
        GameObject tabManagerOBJ = GameObject.FindWithTag("TabManager");
        tabManager = tabManagerOBJ.GetComponent<TabManager>();
        
        //UpdateAIEstimation();
        //DisplayAIEstimation();

        timerAIChangeEstimation = 0f;
        timer = 0f;
        despawnTimer = utilities.GenerateRandomNumber(persistentData.classesDespawnRate[realClass]);
        //StartCoroutine(UpdateVisibility(soundSourcePair.getGameObject()));
    }
    void Start()
    {
        timer = 0f;
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
                    if(isProducingSound){
                        Debug.Log("Pressed");
                        //if the tracker was pressed, then it tells the ledger that this script instance is the one selected
                        tabManager.SelectTracker(this);
                        isTrackerSelected = true;
                    }
                }
            }
        }
        
        //StartCoroutine(CheckAIUpdates());
        CheckSoundLevels();
        CheckVisibility();
        CheckAIUpdates();
        if(isAIactive){
            DisplayAIEstimation();
        }
        else{
            //DisplayUserClass();
        }
        CheckStressLevels();
        UpdatePosition(soundSourcePair.getGameObject()); // always update its position
        CheckDespawn();
        
    }

    private void CheckDespawn(){
        if(despawnTimer <= timer){
            soundSourcePair.readyToDespawn = true;
        }
    }

    public void Despawn(){
        Debug.Log("Despawn");
        entityManager.DespawnEntity( soundSourcePair.getGameObject() ,this);
        tabManager.Unselect();
        Destroy(soundSourcePair.getGameObject());
        Destroy(gameObject);
    }

    private void CheckVisibility(){
        if(isProducingSound && displayedClass != "none" ){
            TurnOnTracker();
        }
        else{
            if(IDCounter == 3){
                //Debug.Log("Turn off");
            }
            TurnOffTracker();
        }

    }

    private void CheckSoundLevels(){
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
                if(isProducingSoundProgressTimer >= 3f){
                    isProducingSound = false;
                    isProducingSoundProgressTimer = 0f;
                }
                //TurnOffTracker();
            }
        }
        else{
            if(totalVolume > -55f){
                isProducingSoundProgressTimer += Time.deltaTime;
                if(isProducingSoundProgressTimer >= 3f){
                    isProducingSound = true;
                    isProducingSoundProgressTimer = 0f;
                }
                //TurnOnTracker();
            }
        }

    }

    private void TurnOffTracker()
    {
        //Debug.Log("turn off");
        if(isTrackerSelected){
            tabManager.Unselect();
        }
        GetComponent<SpriteRenderer>().enabled = false;

    }

    private void TurnOnTracker()
    {
        //Debug.Log("turn on");
        GetComponent<SpriteRenderer>().enabled = true;
    }




    public void UpdatePosition(GameObject soundSource){
        float soundRot = utilities.getSoundSourceAngle(soundSource);
        soundRot = utilities.Remap(soundRot, -180f, 180f, -50f, 50f);

        Vector3 currentPosition = transform.position;
        transform.localPosition = new Vector3(soundRot * -1f, currentPosition.y, currentPosition.z);
    }

    
    private void CheckAIUpdates(){
        if(timer <= timerAIChangeEstimation){
            timer += Time.deltaTime; // Increment the timer
        }
        else{
            timerAIChangeEstimation = timerAIChangeEstimation + utilities.GenerateRandomNumber(persistentData.AITimeDistribution[realClass]);
            UpdateAIEstimation();
            
        }

        //linearly interpolate between present confidence level and futre confidence level
        AIEstimationConfidence = utilities.Interpolate(previousAIEstimationConfidence, futureAIEstimationConfidence, timerAIConfidence1, timer, timerAIConfidence2);

    }

    public void UpdateAIEstimation(){
        //TODO implement previous and future estimation Class
        AIEstimationClass = utilities.SelectRandomWeighted(persistentData.classes, persistentData.AIClassWeights[realClass]);

        string key = realClass + "-" + AIEstimationClass;

        timerAIConfidence1 = timerAIConfidence2;
        timerAIConfidence2 = timer + timerAIChangeEstimation;
        if(futureAIEstimationConfidence == null){
            futureAIEstimationConfidence = utilities.GenerateRandomNumber(persistentData.AIConfidenceDistribution[key]);
        }
        previousAIEstimationConfidence = futureAIEstimationConfidence;
        futureAIEstimationConfidence = utilities.GenerateRandomNumber(persistentData.AIConfidenceDistribution[key]);

    }

    public void DisplayAIEstimation(){
        //Debug.Log("DisplayAI");
        isAIactive = true;
        displayedClass = AIEstimationClass;
        UpdateDisplaySymbol();

        if(displayedClass == "none"){
            return;
        }
        
        if(isTrackerSelected){
            tabManager.UpdateLedgerSelectedClassText();
            tabManager.UpdateLedgerSelectedClassSymbol();
            tabManager.UpdateAITab();
        }

    }

    public void DisplayUserClass(string selectedClass){
        isAIactive = false;
        displayedClass = selectedClass;
        
        UpdateDisplaySymbol();
        
        if(isTrackerSelected){
            tabManager.UpdateLedgerSelectedClassText();
            tabManager.UpdateLedgerSelectedClassSymbol();
            tabManager.UpdateAITab();
        }
        
    }

    //update the shown bearing symbol
    public void UpdateDisplaySymbol(){

        if(!isProducingSound){
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
            spriteRenderer.color = colorAI;
        }
        else{
            spriteRenderer.color = colorUser;
        }
        
    }

    public void Capture(){
        Despawn();
    }
    
    public string getRealClass(){
        return realClass;
    }

}
