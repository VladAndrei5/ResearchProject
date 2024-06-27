using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class PersistentData : MonoBehaviour
{
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

    public Dictionary<string, float[]> speedDistribution = new Dictionary<string, float[]>()
    {
        {"ship", new float[] {5f,0.5f,0f,30f} },
        {"seaLife", new float[] {2f,0.5f,0f,30f} },
        {"pirate", new float[] {5f,1f,0f,30f} }

    };
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
        {"ship", new float[] {24f,3f,1f,100f} },
        {"seaLife", new float[] {13f,3f,1f,100f} },
        {"pirate", new float[] {60f,5f,1f,100f} }
    };

    public Dictionary<string, float[]> classesDespawnRate = new Dictionary<string, float[]>()
    {
        {"ship", new float[] {45,3f,15f,200f} },
        {"seaLife", new float[] {30f,6f,15f,200f} },
        {"pirate", new float[] {55f,9f,15f,200f} }
    };

    public Dictionary<string, float[]> AITimeDistribution = new Dictionary<string, float[]>()
    {
        {"ship", new float[] {7f,1.2f,0f,20f} },
        {"seaLife", new float[] {5f,1f,0f,20f} },
        {"pirate", new float[] {10f,2f,0f,20f} }
    };
    

    public Dictionary<string, float[]> AIConfidenceDistribution = new Dictionary<string, float[]>()
    {
        {"ship-ship", new float[] {50f,5f,0f,100f} },
        {"ship-pirate", new float[] {50f,5f,0f,100f} },
        {"ship-seaLife", new float[] {50f,5f,0f,100f} },
        {"ship-unknown", new float[] {50f,5f,0f,100f} },

        {"pirate-ship", new float[] {50f,10f,0f,100f} },
        {"pirate-pirate", new float[] {50f,10f,0f,100f} },
        {"pirate-seaLife", new float[] {50f,10f,0f,100f} },
        {"pirate-unknown", new float[] {50f,10f,0f,100f} },

        {"seaLife-ship", new float[] {50f,10f,0f,100f} },
        {"seaLife-pirate", new float[] {50f,10f,0f,100f} },
        {"seaLife-seaLife", new float[] {50f,10f,0f,100f} },
        {"seaLife-unknown", new float[] {50f,10f,0f,100f} }
    };

    //the real class represent the key, the float[] represents the weight for each class at that position.
    //e.g. the weight at index 0 represent the classes[0] class's weight.
    public Dictionary<string, float[]> AIClassWeights = new Dictionary<string, float[]>()
    {
        {"ship", new float[] {5f,1f,1f,1f} },
        {"seaLife", new float[] {1f,1f,1f,1f} },
        {"pirate", new float[] {1f,1f,1f,1f} }
    };

    public int countdown = 900;
    public float noiseScalingFactor = 0.05f;

    //public TextAsset SaveScoreFile;

    void Awake()
    {
        Random.InitState(seedRandom);
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

    public float GetNoiseScalingFactor(){
        return noiseScalingFactor;
    }

    public void SaveGame(){
        
    }

}
