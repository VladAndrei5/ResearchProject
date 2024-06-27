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
    public float timerAIChangeEstimation;
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
    public Color colorUser;
    public Color colorAI;


    //references to classes
    public TabManager tabManager;
    public Utilities utilities;
    public SoundSourceBehaviour soundSourcePair;
    public PersistentData persistentData;
    public EntityManager entityManager;

    public int IDCounter;



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

        //create ledger reference
        GameObject tabManagerOBJ = GameObject.FindWithTag("TabManager");
        tabManager = tabManagerOBJ.GetComponent<TabManager>();
        
        UpdateAIEstimation();
        timerAIChangeEstimation = utilities.GenerateRandomNumber(persistentData.AITimeDistribution[realClass]);
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
        
        StartCoroutine(CheckAIUpdates());
        StartCoroutine(CheckSoundLevels());
        StartCoroutine(CheckVisibility());
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

    private IEnumerator CheckVisibility(){
        if(isProducingSound && displayedClass != "none" ){
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
        //Debug.Log("turn off");
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

    
    private IEnumerator CheckAIUpdates(){
        if(timer <= timerAIChangeEstimation){
            timer += Time.deltaTime; // Increment the timer
            yield return null; // Wait for the next frame
        }
        else{
            UpdateAIEstimation();
            if(isAIactive){
                DisplayAIEstimation();
            }
            timerAIChangeEstimation = timerAIChangeEstimation + utilities.GenerateRandomNumber(persistentData.AITimeDistribution[realClass]);
        }
    }

    public void UpdateAIEstimation(){
        AIEstimationClass = utilities.SelectRandomWeighted(persistentData.classes, persistentData.AIClassWeights[realClass]);
        string key = realClass + "-" + AIEstimationClass;
        AIEstimationConfidence = utilities.GenerateRandomNumber(persistentData.AIConfidenceDistribution[key]);
        
    }

    public void DisplayAIEstimation(){
        Debug.Log("DisplayAI");
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
