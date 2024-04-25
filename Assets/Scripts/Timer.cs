using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Button nextScenarioButton;
    public Button endScreenButton;
    public float totalTime;
    private float currentTime;
    public Text timerText;
    public Light spotlight1;
    public Light spotlight2;
    public PersistentData persistentData;
    public LevelManager levelManager;
    public GameObject nextLevelPanel;

    public GameObject endScreenPanel;

    public void SetPanelInteractibility(GameObject panel, bool visible){
        panel.SetActive(visible);
    }

    void HandleEndScreenButtonClick()
    { 
        Application.Quit();
    }

    /*if the button "next scenario" is pressed then restart the time
    increment the current scenario number and move to next level
    */
    void HandleNextLevelButtonClick()
    { 
        Time.timeScale = 0f;
        persistentData.currentScenarioNumb++;
        levelManager.NextLevel();  
        SetPanelInteractibility(endScreenPanel, false);
        SetPanelInteractibility(nextLevelPanel, false);
    }

    //make the "next scenario" screen appear
    public void NextScenarioScreen(){
        Time.timeScale = 0f;
        SetPanelInteractibility(nextLevelPanel, true);
    }

    public void EndScreen(){
        Time.timeScale = 0f;
        SetPanelInteractibility(endScreenPanel, true);
    }

    // Start is called before the first frame update
    void Awake()
    {
        SetPanelInteractibility(endScreenPanel, false);
        SetPanelInteractibility(nextLevelPanel, false);
        Time.timeScale = 1f;
        nextScenarioButton.onClick.AddListener(HandleNextLevelButtonClick);

        endScreenButton.onClick.AddListener(HandleEndScreenButtonClick);

        GameObject persistentDataOBJ = GameObject.FindWithTag("PersistentData");
        persistentData = persistentDataOBJ.GetComponent<PersistentData>();

        GameObject levelManagerOBJ = GameObject.FindWithTag("LevelManager");
        levelManager = levelManagerOBJ.GetComponent<LevelManager>();

        totalTime = persistentData.GetCurrentScenarioData().countdown;
        currentTime = totalTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTime > 0)
        {
            if (currentTime < 60)
            {
                timerText.color = Color.red;
                float rotationAmount = 1f * Time.deltaTime;

                // Create a rotation increment based on the calculated rotation amount
                Quaternion rotationIncrement = Quaternion.Euler(0f, rotationAmount, 0f);

                // Apply the rotation increment to current rotation
                spotlight1.transform.rotation *= rotationIncrement;
                spotlight2.transform.rotation *= rotationIncrement;
            }

            currentTime -= Time.deltaTime;

            // Calculate minutes, seconds, and milliseconds
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            int milliseconds = Mathf.FloorToInt((currentTime * 1000) % 1000);

            // Display the timer in mm:ss:msms format
            timerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        }
        else
        {
             //if timer reached 0 move on to next scenario
            timerText.text = "00:00:000";
            if(persistentData.currentScenarioNumb < persistentData.GetNumberOfScenarios()){
                persistentData.ResetScore();
                NextScenarioScreen();
            }
            else{
                EndScreen();
            }           
        }
    }
}
