using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SoundSourceBehaviour : MonoBehaviour
{

    public string audioFile;
    public string id;
    public Movement movement;
    public float timer = 0f;
    public Vector2 destination;
    public Vector2 initialPosition;
    public float timeToReachDestination;

    public void UpdateAudioFilePlaying(string audioFileName){
        string audioPath = "Sounds/" + audioFileName;
        AudioClip clip = Resources.Load<AudioClip>(audioPath);

        AudioSource audioSource = this.GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void InitaliseBehaviour(string audioFile, string id, Movement movement){
        this.id = id;
        this.audioFile = audioFile;
        this.movement = movement;

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
    
    

    private IEnumerator MoveToDestination()
    {
        for(int i = 1; i < movement.time.Length; i++){
            timer = 0f;
            initialPosition = transform.position;
            float x =  movement.x[i];
            float y =  movement.y[i];
            destination = new Vector2(x, y);

            timeToReachDestination = movement.time[i] - (movement.time[i-1] / 3);

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
    
}
