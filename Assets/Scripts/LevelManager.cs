using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    public PersistentData persistentData;
    public Scenario currentScenarioData;
    public EntityManager entityManager;

    void Awake()
    {
        //prevents it from being deleted on scene change
        DontDestroyOnLoad(this.gameObject);
        //GameObject entityManagerObj = GameObject.FindWithTag("EntityManager");
        //entityManager = entityManagerObj.GetComponent<EntityManager>();
    }

    public void InitialiseLevelManager(){
        currentScenarioData = persistentData.GetCurrentScenarioData();
        SceneManager.LoadScene(0);
        //initialises the level from the current scenario data
        //LoadLevelFromScenario(currentScenarioData);
    }

    public void NextLevel(){
        currentScenarioData = persistentData.GetCurrentScenarioData();
        SceneManager.LoadScene(0);
        //initialises the level from the current scenario data
        //LoadLevelFromScenario(currentScenarioData);
    }

    /*
    public void LoadLevelFromScenario(Scenario scenario){
        //loads a new scene
        SceneManager.LoadScene(0);
        //populates the scene with entities from scenario
        //entityManager.PopulateEntities(scenario);
    }
    */

}
