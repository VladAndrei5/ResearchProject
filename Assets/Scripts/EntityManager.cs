using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Audio;
using Random=UnityEngine.Random;
using System.IO;

public class EntityManager : MonoBehaviour
{   

    //Prefab game objects which will be instantiated from scenario file, trackers and audio sources
    public GameObject PrefabMapTracker;
    public GameObject PrefabBearingTracker;
    public GameObject PrefabAudioSource;
    //Parent game objects to group the instantiated objects accordingly
    public GameObject ParentMapTrackers;
    public GameObject ParentBearingTrackers;
    public GameObject ParentAudioSources;
    private GameObject audioSource;
    
    //public GameObject[] soundSourcesArray;
    public PersistentData persistentData;
    public Utilities utilities;

    public float[] spawnEntitiesInterval;
    public BearingTrackerBehaviour[] bearingTrackersArray;

    public List<GameObject> audioSourceObjList = new List<GameObject>();
    public List<BearingTrackerBehaviour> bearingTrackerList = new List<BearingTrackerBehaviour>();

    //lists which holds and updates the relative angle of all soundsources to the beam
    public List<float> relativeAngleList = new List<float>();
    public List<float> angleList = new List<float>();

    private Movement movement;
    private AI behaviourAI;
    string audioClip;
    string realClass;
    string id;

    public int IDCounter;

    public float timer;

    void Awake(){
        IDCounter = 0;
        CreateInitialEntities();
        StartCoroutine(SpawnEntities());
    }

    //spawns all the audio sources
    private GameObject SpawnPrefabsAudioSource(AudioClip audioClip, string realClass, string id){
        IDCounter++;

        Vector3 localPosition = Vector3.zero; // Local position of the spawned prefabs relative to the parentObject
        Vector3 localRotation = Vector3.zero; // Local rotation of the spawned prefabs relative to the parentObject
        Vector3 localScale = Vector3.one;

        //Initalise Sound Source Object
        GameObject audioSource = Instantiate(PrefabAudioSource, ParentAudioSources.transform);
        audioSource.transform.localPosition = localPosition;
        //Initalise its behaviour
        audioSource.GetComponent<SoundSourceBehaviour>().InitaliseBehaviour(audioClip, realClass, id, IDCounter);
        return audioSource;
    }

    //spawns all the bearing trackers
    private BearingTrackerBehaviour SpawnPrefabsBearingTracker(GameObject audioSource, string realClass){
        Vector3 localPosition = Vector3.zero; // Local position of the spawned prefabs relative to the parentObject
        Vector3 localRotation = Vector3.zero; // Local rotation of the spawned prefabs relative to the parentObject
        Vector3 localScale = Vector3.one;

        //change this back to accurate
        localPosition = new Vector3(Random.Range(-50f, 50), 56.5f, 0f);
        localScale = new Vector3(7.5f, 7.5f, 1f);
        localRotation = new Vector3(0f, 0f, 0f);

        //Initalise bearing tracker
        GameObject bearingTracker = Instantiate(PrefabBearingTracker, ParentBearingTrackers.transform);
        bearingTracker.transform.localPosition = localPosition;
        bearingTracker.transform.localRotation = Quaternion.Euler(localRotation);
        bearingTracker.transform.localScale = localScale;
        //Initalise its behaviour
        bearingTracker.GetComponent<BearingTrackerBehaviour>().InitaliseBehaviour(realClass, audioSource.GetComponent<SoundSourceBehaviour>(), id, IDCounter);
        return bearingTracker.GetComponent<BearingTrackerBehaviour>();
    }


    public void CreateInitialEntities(){
        //int numberOfEntities = utilities.GenerateRandomNumber(utilities.initialNumberOfEntitiesDistribution);
        //go through all classes
        //except last class which is unknown class
        for(int j = 0; j < persistentData.classes.Length - 1; j++){
            //here we should perhaps generate the initial number of entities to be spawned depnding on the class
            //for now keep it 2 for each
            for(int i = 0; i < 1; i++){
                string classType = persistentData.classes[j];
                SpawnEntity(classType);
            }
        }
    }

    private void SpawnEntity(string classType){
        AudioClip audioClip = ChooseAudioClip(classType);
        realClass = classType;
        id = "11";
        //spawns all the prefabs;
        GameObject audioSource = SpawnPrefabsAudioSource(audioClip, classType, id);
        BearingTrackerBehaviour bearingTracker = SpawnPrefabsBearingTracker(audioSource, classType);


        //add them to their respective lists
        audioSourceObjList.Add(audioSource);
        bearingTrackerList.Add(bearingTracker);
        relativeAngleList.Add(0f);
        angleList.Add(0f);
    }

    public void DespawnEntity(GameObject audioSource, BearingTrackerBehaviour tracker){

        audioSourceObjList.Remove(audioSource);
        bearingTrackerList.Remove(tracker);
        
        /*
        audioSourceObjList.Remove(audioSource);
        bearingTrackerList.Remove(tracker);
        relativeAngleList.RemoveAt(audioSourceObjList.IndexOf(audioSource));
        angleList.RemoveAt(audioSourceObjList.IndexOf(audioSource));
        */
        
    }

    public AudioClip ChooseAudioClip(string classType)
    {
        string folderPath = "Sounds/" + classType;
        AudioClip[] audioClips = Resources.LoadAll<AudioClip>(folderPath);
        if (audioClips.Length > 0)
        {
            int randomIndex = Random.Range(0, audioClips.Length);
            return audioClips[randomIndex];
        }
        return null;
    }

    private IEnumerator SpawnEntities()
    {
        timer = 0f;

        spawnEntitiesInterval = new float[persistentData.classes.Length];
        for(int i = 0; i < persistentData.classes.Length - 1; i ++){
            spawnEntitiesInterval[i] = utilities.GenerateRandomNumber(persistentData.classesSpawnRate[persistentData.classes[i]]);
        }

        while(true){

            for(int j = 0; j < persistentData.classes.Length - 1; j++){

                if(timer > spawnEntitiesInterval[j]){
                    string classType = persistentData.classes[j];
                    SpawnEntity(classType);
                    spawnEntitiesInterval[j] = spawnEntitiesInterval[j] + utilities.GenerateRandomNumber(persistentData.classesSpawnRate[persistentData.classes[j]]);
                }
                yield return null;
            }
            yield return new WaitForSeconds(1f);
        }

    }

    void Update(){
        timer += Time.deltaTime;
    }


    /*
    public void PopulateEntities(Scenario scenario){
        //get the number of entities in first scenario
        int numberOfEntities = scenario.numberEntities;

        //for each entity it spawns its audio source and trackers
        for(int i = 0; i < numberOfEntities; i++){
            //take audio file name from scenarioData
            audioClip = scenario.entities[i].audio;
            //same for classType
            realClass = scenario.entities[i].realClass;
            id = scenario.entities[i].id;
            movement = scenario.entities[i].movement;
            behaviourAI = scenario.entities[i].AI;
            //spawns all the prefabs;
            GameObject audioSource = SpawnPrefabsAudioSource(audioClip, id, movement);
            //SpawnPrefabsMapTrackers();
            SpawnPrefabsBearingTracker(audioSource);
        }
        //after creating the objects, add them to their respective lists
        foundObjects = GameObject.FindGameObjectsWithTag("AudioSourceObj");
        audioSourcesObjArray = new GameObject[foundObjects.Length];
        audioSourcesObjArray = foundObjects;
        
        Utilities.Instance.CopyArray(audioSourcesObjArray, foundObjects);

        foundObjects = GameObject.FindGameObjectsWithTag("BTrackers");
        bearingTrackersArray = new BearingTrackerBehaviour[foundObjects.Length];
        //create the bearing trackers list
        for (int i = 0; i < foundObjects.Length; i++){
            bearingTrackersArray[i] = foundObjects[i].GetComponent<BearingTrackerBehaviour>();
        }
        
    }
    */

    public GameObject getSoundSourceObj(int i){
        return audioSourceObjList[i];
    }

    public List<GameObject> getSoundSourcesList(){
        return audioSourceObjList;
    }

}
