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
    public Vector2 destination;
    public Vector2 initialPosition;
    public float timeToReachDestination;

    public float speed = 5f;
    private Vector2 direction;
    private float timeToChangeDirection;
   
    public PersistentData persistentData;
    public EntityManager entityManager;

    
    public void UpdateAudioFilePlaying(AudioClip audioClip){
        //string audioPath = "Sounds/" + audioFileName;
        //AudioClip clip = Resources.Load<AudioClip>(audioPath);
        Random.InitState(persistentData.seedRandom);
        AudioSource audioSource = this.GetComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.Play();
    }
    

    public void InitaliseBehaviour(AudioClip audioClip, string id){
        this.id = id;

        // play the sound
        UpdateAudioFilePlaying(audioClip);
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
        //if no sound is playing then play some
        if(!GetComponent<AudioSource>().isPlaying){
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


    public void Capture(float timeCaptured){
        StartCoroutine(Mute(timeCaptured));
    }

    private IEnumerator Mute(float timeCaptured)
    {
        GetComponent<AudioSource>().volume = 0f;
        yield return new WaitForSeconds(timeCaptured);
        GetComponent<AudioSource>().volume = 1f;

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
