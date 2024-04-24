using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class PersistentData : MonoBehaviour
{

    public TextAsset jsonScenariosFile;
    public ScenarioData scenarioData;
    public int currentScenarioNumb;
    public int currentScore;

    void Awake()
    {
        //prevents it from being deleted on scene change
        DontDestroyOnLoad(this.gameObject);
        //loading the scenario data
        scenarioData = JsonConvert.DeserializeObject<ScenarioData>(jsonScenariosFile.text);
        //set up the current scenario number
        currentScenarioNumb = 1;
        ResetScore();
    }

    public int GetScore(){
        return currentScore;
    }

    public void UpdateScore(int reward){
        currentScore = currentScore + reward;
    }

    public void ResetScore(){
        currentScore = 0;
    }

    //returns the current scenario being played
    public Scenario GetCurrentScenarioData(){
        return scenarioData.scenarios[currentScenarioNumb-1];
    }

    public int GetNumberOfScenarios(){
        return scenarioData.scenarios.Length;
    }

    public float GetNoiseScalingFactor(){
        return scenarioData.scenarios[currentScenarioNumb-1].noiseScalingFactor;
    }

    public void SaveGame(){
        
    }

}
