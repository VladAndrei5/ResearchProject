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

    public GameObject[] audioSourcesObjArray;
    public GameObject[] foundObjects;
    public BearingTrackerBehaviour[] bearingTrackersArray;

    private Movement movement;
    private AI behaviourAI;
    string audioFile;
    string realClass;
    string id;



    void Awake(){
        GameObject persistentDataOBJ = GameObject.FindWithTag("PersistentData");
        persistentData = persistentDataOBJ.GetComponent<PersistentData>();
        PopulateEntities(persistentData.GetCurrentScenarioData());
    }

    //spawns all the audio sources
    private GameObject SpawnPrefabsAudioSource(string audioFile, string id, Movement movement){
        Vector3 localPosition = Vector3.zero; // Local position of the spawned prefabs relative to the parentObject
        Vector3 localRotation = Vector3.zero; // Local rotation of the spawned prefabs relative to the parentObject
        Vector3 localScale = Vector3.one;

        //Initalise Sound Source Object
        GameObject audioSource = Instantiate(PrefabAudioSource, ParentAudioSources.transform);
        audioSource.transform.localPosition = localPosition;
        //Initalise its behaviour
        audioSource.GetComponent<SoundSourceBehaviour>().InitaliseBehaviour(audioFile, id, movement);
        return audioSource;
    }

    //spawns all the bearing trackers
    private void SpawnPrefabsBearingTracker(GameObject audioSource){
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
        bearingTracker.GetComponent<BearingTrackerBehaviour>().InitaliseBehaviour(realClass, behaviourAI, audioSource.GetComponent<SoundSourceBehaviour>());
    }

/*
    //spawns all the map trackers
    private void SpawnPrefabsMapTrackers(){
        Vector3 localPosition = Vector3.zero; // Local position of the spawned prefabs relative to the parentObject
        Vector3 localRotation = Vector3.zero; // Local rotation of the spawned prefabs relative to the parentObject
        Vector3 localScale = Vector3.one;

        localPosition = Vector3.zero;
        localScale = new Vector3(4.5f, 4.5f, 1f);

        //Initalise map tracker
        GameObject mapTracker = Instantiate(PrefabMapTracker, ParentMapTrackers.transform);
        mapTracker.transform.localPosition = localPosition;
        mapTracker.transform.localScale = localScale;
        //Initalise its behaviour
        mapTracker.GetComponent<EntityBehaviour>().InitaliseBehaviour(audioFile, classType, id, movementArray, AIBehaviourArray);
    }
*/

    /*creates the various trackers and sound sources game objects
    and their behaviours from a given scenario
    */
    public void PopulateEntities(Scenario scenario){
        //get the number of entities in first scenario
        int numberOfEntities = scenario.numberEntities;

        //for each entity it spawns its audio source and trackers
        for(int i = 0; i < numberOfEntities; i++){
            //take audio file name from scenarioData
            audioFile = scenario.entities[i].audio;
            //same for classType
            realClass = scenario.entities[i].realClass;
            id = scenario.entities[i].id;
            movement = scenario.entities[i].movement;
            behaviourAI = scenario.entities[i].AI;
            //spawns all the prefabs;
            GameObject audioSource = SpawnPrefabsAudioSource(audioFile, id, movement);
            //SpawnPrefabsMapTrackers();
            SpawnPrefabsBearingTracker(audioSource);
        }
        //after creating the objects, add them to their respective lists
        foundObjects = GameObject.FindGameObjectsWithTag("AudioSourceObj");
        audioSourcesObjArray = new GameObject[foundObjects.Length];
        audioSourcesObjArray = foundObjects;
        /*
        Utilities.Instance.CopyArray(audioSourcesObjArray, foundObjects);

        foundObjects = GameObject.FindGameObjectsWithTag("BTrackers");
        bearingTrackersArray = new BearingTrackerBehaviour[foundObjects.Length];
        //create the bearing trackers list
        for (int i = 0; i < foundObjects.Length; i++){
            bearingTrackersArray[i] = foundObjects[i].GetComponent<BearingTrackerBehaviour>();
        }
        */
    }

    public GameObject getSoundSourceObj(int i){
        return audioSourcesObjArray[i];
    }

    public GameObject[] getSoundSourcesArray(){
        return audioSourcesObjArray;
    }

}
