using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StressDetector : MonoBehaviour
{
    public float stressLevel = 70f;
    public GameObject stressPopUpTab;
    [SerializeField] public Button exitButton;
    public bool exitPopUp = false;

    public float getStressLevel(){
        return stressLevel;
    }

    private void Awake() {
        exitButton.onClick.AddListener(exitClick);
    }

    void Update(){
        if(stressLevel > 50f && !exitPopUp){
            stressPopUpTab.SetActive(true);
        }
    }

    private void exitClick(){
        exitPopUp = true;
        stressPopUpTab.SetActive(false);
    }
}
