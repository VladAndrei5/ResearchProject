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

    public string AIType = "AI";

    //unknown should always be last, and present even if unused
    public string[] classes = new string[] {"ship" , "seaLife" , "pirate" , "unknown"};
    //public float[] classesSpawnWeights = new float[] {1f, 1f, 1f, 1f};


    //
    public Dictionary<string, float[]> timeChangeDirectionIntervalDistribution = new Dictionary<string, float[]>()
    {
        {"ship", new float[] {25,4f,5f,30f} },
        {"seaLife", new float[] {25,4f,5f,30f} },
        {"pirate", new float[] {25,4f,5f,30f} }

    };

    //spawn rate in seconds
    public Dictionary<string, float[]> classesSpawnRate = new Dictionary<string, float[]>()
    {
        {"ship", new float[] {15,3f,1f,25f} },
        {"seaLife", new float[] {10f,3f,1f,25f} },
        {"pirate", new float[] {30f,1f,1f,40f} }
    };

    public Dictionary<string, float[]> classesDespawnRate = new Dictionary<string, float[]>()
    {
        {"ship", new float[] {20f,3f,15f,30f} },
        {"seaLife", new float[] {20f,3f,15f,30f} },
        {"pirate", new float[] {20f,3f,15f,30f} }
    };

    public Dictionary<string, float[]> AITimeDistribution = new Dictionary<string, float[]>()
    {
        {"ship", new float[] {5f,1f,0f,20f} },
        {"seaLife", new float[] {5f,1f,0f,20f} },
        {"pirate", new float[] {5f,1f,0f,20f} }
    };
    

    public Dictionary<string, float[]> AIConfidenceDistribution = new Dictionary<string, float[]>()
    {
        {"ship-ship", new float[] {80f,1f,0f,100f} },
        {"ship-pirate", new float[] {80f,1f,0f,100f} },
        {"ship-seaLife", new float[] {50f,1f,0f,100f} },
        {"ship-unknown", new float[] {50f,1f,0f,100f} },

        {"pirate-ship", new float[] {50f,1f,0f,100f} },
        {"pirate-pirate", new float[] {50f,1f,0f,100f} },
        {"pirate-seaLife", new float[] {50f,1f,0f,100f} },
        {"pirate-unknown", new float[] {50f,1f,0f,100f} },

        {"seaLife-ship", new float[] {50f,1f,0f,100f} },
        {"seaLife-pirate", new float[] {50f,1f,0f,100f} },
        {"seaLife-seaLife", new float[] {50f,1f,0f,100f} },
        {"seaLife-unknown", new float[] {50f,1f,0f,100f} }
    };

    //the real class represent the key, the float[] represents the weight for each class at that position.
    //e.g. the weight at index 0 represent the classes[0] class's weight.
    public Dictionary<string, float[]> AIClassWeights = new Dictionary<string, float[]>()
    {
        {"ship", new float[] {5f,1f,1f,1f} },
        {"seaLife", new float[] {1f,1f,1f,1f} },
        {"pirate", new float[] {1f,1f,1f,1f} }
    };

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
