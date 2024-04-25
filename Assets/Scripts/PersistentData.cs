using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class PersistentData : MonoBehaviour
{

    public TextAsset jsonScenariosFile;
    public ScenarioData scenarioData;
    public int currentScenarioNumb;
    public int currentScore;


    private List<int> TimeScoreList = new List<int>();
    private string scoreFileName = "scoreFile.txt";


    //public TextAsset SaveScoreFile;

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

    private void SaveToTextFile()
    {
        string filePath = Path.Combine(Application.dataPath, scoreFileName);

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (int value in TimeScoreList)
            {
                writer.WriteLine(value);
            }
        }

        Debug.Log("Int list saved to file: " + filePath);
    }

    public int GetScore(){
        return currentScore;
    }

    public void UpdateScore(int reward){
        TimeScoreList.Add(reward);
        SaveToTextFile();
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
