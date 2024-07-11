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

    public string difficulty = "easy";


    public float shipSpawnRate;
    public float seaLifeSpawnRate;
    public float pirateSpawnRate;

    public string AIType = "AI";

    //unknown should always be last, and present even if unused
    public string[] classes = new string[] {"ship" , "seaLife" , "pirate" , "unknown"};
    //public float[] classesSpawnWeights = new float[] {1f, 1f, 1f, 1f};

    public Dictionary<string, float[]> speedDistribution = new Dictionary<string, float[]>();
    //
    public Dictionary<string, float[]> timeChangeDirectionIntervalDistribution = new Dictionary<string, float[]>();

    //spawn rate in seconds
    public Dictionary<string, float[]> classesSpawnRate = new Dictionary<string, float[]>();

    public Dictionary<string, float[]> classesDespawnRate = new Dictionary<string, float[]>();

    public Dictionary<string, float[]> AITimeDistribution = new Dictionary<string, float[]>();
    

    public Dictionary<string, float[]> AIConfidenceDistribution = new Dictionary<string, float[]>();

    //the real class represent the key, the float[] represents the weight for each class at that position.
    //e.g. the weight at index 0 represent the classes[0] class's weight.
    public Dictionary<string, float[]> AIClassWeights = new Dictionary<string, float[]>();

    public int countdown = 900;
    public float noiseScalingFactor = 0.05f;

    public HardDifficulty hardDifficulty;
    public EasyDifficulty easyDifficulty;
    public NormalDifficulty normalDifficulty;

    //public TextAsset SaveScoreFile;

    void Awake()
    {
        Random.InitState(seedRandom);
        ResetScore();
        switch (difficulty)
        {
            case "hard":
                speedDistribution = hardDifficulty.speedDistribution;
                timeChangeDirectionIntervalDistribution = hardDifficulty.timeChangeDirectionIntervalDistribution;
                classesSpawnRate = hardDifficulty.classesSpawnRate;
                classesDespawnRate = hardDifficulty.classesDespawnRate;
                AITimeDistribution = hardDifficulty.AITimeDistribution;
                AIConfidenceDistribution = hardDifficulty.AIConfidenceDistribution;
                AIClassWeights = hardDifficulty.AIClassWeights;
                countdown = hardDifficulty.countdown;
                noiseScalingFactor = hardDifficulty.noiseScalingFactor;
                break;
            case "normal":
                speedDistribution = normalDifficulty.speedDistribution;
                timeChangeDirectionIntervalDistribution = normalDifficulty.timeChangeDirectionIntervalDistribution;
                classesSpawnRate = normalDifficulty.classesSpawnRate;
                classesDespawnRate = normalDifficulty.classesDespawnRate;
                AITimeDistribution = normalDifficulty.AITimeDistribution;
                AIConfidenceDistribution = normalDifficulty.AIConfidenceDistribution;
                AIClassWeights = normalDifficulty.AIClassWeights;
                countdown = normalDifficulty.countdown;
                noiseScalingFactor = normalDifficulty.noiseScalingFactor;
                break;
            case "easy":
                speedDistribution = easyDifficulty.speedDistribution;
                timeChangeDirectionIntervalDistribution = easyDifficulty.timeChangeDirectionIntervalDistribution;
                classesSpawnRate = easyDifficulty.classesSpawnRate;
                classesDespawnRate = easyDifficulty.classesDespawnRate;
                AITimeDistribution = easyDifficulty.AITimeDistribution;
                AIConfidenceDistribution = easyDifficulty.AIConfidenceDistribution;
                AIClassWeights = easyDifficulty.AIClassWeights;
                countdown = easyDifficulty.countdown;
                noiseScalingFactor = easyDifficulty.noiseScalingFactor;
                break;        
            default:
                speedDistribution = normalDifficulty.speedDistribution;
                timeChangeDirectionIntervalDistribution = normalDifficulty.timeChangeDirectionIntervalDistribution;
                classesSpawnRate = normalDifficulty.classesSpawnRate;
                classesDespawnRate = normalDifficulty.classesDespawnRate;
                AITimeDistribution = normalDifficulty.AITimeDistribution;
                AIConfidenceDistribution = normalDifficulty.AIConfidenceDistribution;
                AIClassWeights = normalDifficulty.AIClassWeights;
                countdown = normalDifficulty.countdown;
                noiseScalingFactor = normalDifficulty.noiseScalingFactor;
                break;
        }

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
