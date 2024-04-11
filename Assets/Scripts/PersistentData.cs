using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class PersistentData : MonoBehaviour
{

    public TextAsset jsonScenariosFile;
    public ScenarioData scenarioData;
    public int currentScenarioNumb;

    void Awake()
    {
        //prevents it from being deleted on scene change
        DontDestroyOnLoad(this.gameObject);
        //loading the scenario data
        scenarioData = JsonConvert.DeserializeObject<ScenarioData>(jsonScenariosFile.text);
        //set up the current scenario number
        currentScenarioNumb = 1;
    }

    //returns the current scenario being played
    public Scenario GetCurrentScenarioData(){
        return scenarioData.scenarios[currentScenarioNumb-1];
    }

    public int GetNumberOfScenarios(){
        return scenarioData.scenarios.Length;
    }

}