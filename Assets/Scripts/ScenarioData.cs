using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=UnityEngine.Random;
using System.IO;

//Classes use to store the information in the json file
[System.Serializable]
public class ScenarioData
{
    public Scenario[] scenarios;
}

[System.Serializable]
public class Scenario
{
    public string id;
    public int countdown;
    public string AIType;
    public float noiseScalingFactor;
    public int numberEntities;
    public List<Entity> entities;
}

[System.Serializable]
public class Entity
{
    public string id;
    public string audio;
    public string realClass;
    public Movement movement;
    public AI AI;
}

[System.Serializable]
public class Movement
{
    public float[] time;
    public float[] x;
    public float[] y;
}

[System.Serializable]
public class AI
{
    public float[] time;
    public string[] classType;
    public float[] confidence;
}