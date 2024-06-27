using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random=UnityEngine.Random;

public class SoundSourceBehaviour : MonoBehaviour
{

    public string audioFile;
    public string id;
    public float timer = 0f;
    public string realClass;
    public Vector2 destination;
    public Vector2 initialPosition;
    public float timeToReachDestination;
    public AudioClip audClip;

    public float speed = 1f;
    private Vector2 direction;
    private float timeToChangeDirection;

    public bool readyToDespawn;
   
    public PersistentData persistentData;
    public EntityManager entityManager;
    public Utilities utilities;

    public BearingTrackerBehaviour bearingTracker;

    
    public void UpdateAudioFilePlaying(AudioClip audioClip){
        //string audioPath = "Sounds/" + audioFileName;
        //AudioClip clip = Resources.Load<AudioClip>(audioPath);
        
        AudioSource audioSource = this.GetComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.Play();
    }
    

    public void InitaliseBehaviour(AudioClip audioClip, string realClass, string id, int IDCounter){

        //create references
        GameObject obj = GameObject.FindWithTag("Utilities");
        utilities = obj.GetComponent<Utilities>();
        GameObject obj2 = GameObject.FindWithTag("PersistentData");
        persistentData = obj2.GetComponent<PersistentData>();
        GameObject obj3 = GameObject.FindWithTag("EntityManager");
        entityManager = obj3.GetComponent<EntityManager>();

        readyToDespawn = false;
        audClip = audioClip;
        this.id = id;
        this.realClass = realClass;
        // play the sound
        UpdateAudioFilePlaying(audioClip);
    }

    public void SetTrackerPair(BearingTrackerBehaviour trackerPair){
        bearingTracker = trackerPair;
    }

    // Update is called once per frame
    void Start()
    {
        if(id != null){
            direction = GetRandomDirection();
            timer = 0f;
            float x = Random.Range(-50f, 50f);
            float y = Random.Range(-50f, 50f);
            initialPosition = new Vector2(x, y);
            transform.position = initialPosition;
            StartCoroutine(MoveToDestination());
        }
    }

    void Update()
    {
        // Move the object towards the direction vector
        transform.Translate(direction * speed * Time.deltaTime);
  
        if(GetComponent<AudioSource>().isPlaying){
            return;
        }

        if(readyToDespawn){
            bearingTracker.Despawn();
        }
        else{
            UpdateAudioFilePlaying(entityManager.ChooseAudioClip(realClass));
        }
    }

    private Vector2 GetRandomDirection()
    {
        // Generate a random direction
        float angle = Random.Range(0f, 360f);
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }
    
    
    private IEnumerator MoveToDestination()
    {
        while (true)
        {
            // Wait for a random time between the defined intervals
            timeToChangeDirection = utilities.GenerateRandomNumber(persistentData.timeChangeDirectionIntervalDistribution[realClass]);
            yield return new WaitForSeconds(timeToChangeDirection);
            
            // Change direction
            direction = GetRandomDirection();
        }
    }


    public void setPosition(Vector3 newPos){
        transform.position = newPos;
    }

    public Vector3 getPosition(){
        return transform.position;
    }

    public GameObject getGameObject(){
        return gameObject;
    }
    
    public float[] RetrieveSpectrumData(){
        float[] spectrumAudioSource = new float[64];
        GetComponent<AudioSource>().GetSpectrumData(spectrumAudioSource, 0, FFTWindow.BlackmanHarris);
        return spectrumAudioSource;
    }
}
