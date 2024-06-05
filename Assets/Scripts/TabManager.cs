using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabManager : MonoBehaviour
{

    //class references
    public BearingTrackerBehaviour selectedTracker;
    public PersistentData persistentData;

    public TabAI tabAI;
    public TabUser tabUser;

    void Start()
    {
        
    }

    //it takes a tracker and sets it as selected
    public void SelectTracker(BearingTrackerBehaviour tracker){
        if(tracker.isAvailable == false){
            return;
        }
        //turn off the outline of previous selected tracker
        if(selectedTracker != null){
            //selectedTracker.ToggleTrackerOutline(false);
            selectedTracker.isTrackerSelected = false;
            Unselect();
        }

        ChooseClassificationTab.SetActive(true);

    }
}
