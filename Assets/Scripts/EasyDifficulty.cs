using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyDifficulty : MonoBehaviour
{
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

    public int countdown = 400;
    public float noiseScalingFactor = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
