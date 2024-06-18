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

    public int seedRandom = 10;


    public float shipSpawnRate;
    public float seaLifeSpawnRate;
    public float pirateSpawnRate;

    public string[] classes = new string[] {"ship" , "seaLife" , "pirate" , "unknown"};
    //public float[] classesSpawnWeights = new float[] {1f, 1f, 1f, 1f};


    
    Dictionary<string, float[]> timeChangeDirectionIntervalDistribution = new Dictionary<string, float[]>()
    {
        {"ship", new float[] {10f,1f,0f,20f} },
        {"seaLife", new float[] {10f,1f,0f,20f} },
        {"pirate", new float[] {10f,1f,0f,20f} }

    };


    //spawn rate in seconds
    Dictionary<string, float[]> classesSpawnRate = new Dictionary<string, float[]>()
    {
        {"ship", new float[] {10f,1f,0f,20f} },
        {"seaLife", new float[] {10f,1f,0f,20f} },
        {"pirate", new float[] {10f,1f,0f,20f} }
    };
    
    


    /*
    public string[] audioFileShip = new string[] {"extended_passengership_1_sim" , "extended_passengership_2_sim"};
    public float[] audioFileShipWeight = new float[] {1f, 1f};
    public string[] audioFilePirate = new string[] {"sonar_ping_extended_sim"};
    public float[] audioFilePirateWeight = new float[] {1f};
    public string[] audioFileSeaLife = new string[] {"North_Atlantic_Right_Whale_scream_extended_sim"};
    public float[] audioFileSeaLifeWeight = new float[] {1f};
    */

    //normal distribution for initial number of entities
    //public float[] initialNumberOfEntitiesDistribution =  new float[] { 5f, 1f, 1f, 10f };

    //the time distriubiton until a new entity of class ship is spawned
    //public float[] shipSpawnProbabilityDistributuion = new float[] { 5f, 1f, 1f, 10f };



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
