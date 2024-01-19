using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BearingOverlay : MonoBehaviour
{

    public GameLogic gameLogic;
    public GameObject beamDir;
    public GameObject beamRotOverlay;
    public Slider sliderLeft;
    public Slider sliderRight;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        beamDir.transform.rotation = Quaternion.Euler(0f, 0f, gameLogic.beamRotation);
        beamRotOverlay.transform.rotation = Quaternion.Euler(0f, 0f, gameLogic.beamRotation);
        sliderLeft.value = gameLogic.beamW / 2;
        sliderRight.value = gameLogic.beamW / 2;


    }
}
