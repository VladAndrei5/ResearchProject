using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuUI : MonoBehaviour
{
    public LevelManager levelManager;
    public PersistentData persistentData;

    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;

    private void Awake() {
        startButton.onClick.AddListener(startClick);
        exitButton.onClick.AddListener(exitClick);
    }

    private void startClick(){
        //initialises the level manager
        levelManager.InitialiseLevelManager();
    }

    private void exitClick(){
        Application.Quit();
    }
}
