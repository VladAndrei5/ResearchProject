using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
    private float changeDirectionIntervalMin = 1f; // Minimum time to change direction
    private float changeDirectionIntervalMax = 5f; // Maximum time to change direction

    public PersistentData persistentData;


    public void UpdateAudioFilePlaying(string audioFileName){
        string audioPath = "Sounds/" + audioFileName;
        AudioClip clip = Resources.Load<AudioClip>(audioPath);

        AudioSource audioSource = this.GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void InitaliseBehaviour(string audioFile, string id){
        this.id = id;
        this.audioFile = audioFile;

        //find the path to sound file and play the sound
        UpdateAudioFilePlaying(audioFile);
    }

    // Update is called once per frame
    void Start()
    {
        if(id != null){
            timer = 0f;
            float x = movement.x[0];
            float y = movement.y[0];
            initialPosition = new Vector2(x, y);
            transform.position = initialPosition;
            StartCoroutine(MoveToDestination());
        }
    }

    void Update()
    {
        // Move the object towards the direction vector
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private Vector2 GetRandomDirection()
    {
        // Generate a random direction
        float angle = Random.Range(0f, 360f);
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }
    
    
    private IEnumerator MoveToDestination()
    {
        direction = GetRandomDirection();

        while (true)
        {
            // Wait for a random time between the defined intervals
            timeToChangeDirection = Random.Range(changeDirectionIntervalMin, changeDirectionIntervalMax);
            yield return new WaitForSeconds(timeToChangeDirection);
            
            // Change direction
            direction = GetRandomDirection();
        }


        /*
        for(int i = 1; i < movement.time.Length; i++){
            timer = 0f;
            initialPosition = transform.position;
            float x =  movement.x[i];
            float y =  movement.y[i];
            destination = new Vector2(x, y);

            timeToReachDestination = movement.time[i] - movement.time[i-1];

            while (timer < timeToReachDestination)
            {
                float t = timer / timeToReachDestination; // Calculate the normalized time
                transform.position = Vector2.Lerp(initialPosition, destination, t); // Move towards the destination
                timer += Time.deltaTime; // Increment the timer
                yield return null; // Wait for the next frame
            }
            transform.position = destination;

            yield return null;
        }        
        */
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
