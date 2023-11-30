using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BearingLogic : MonoBehaviour
{

    public GameObject sliderRightObject;
    public GameObject sliderLeftObject;

    public GameObject sliderRotationObject;

    public GameObject mapWindowObject;

    public GameObject audSourceObject1;
    public GameObject audSourceObject2;

    public GameObject shipRotation1;
    public GameObject shipRotation2;


    private Slider sliderLeft;
    private Slider sliderRight;
    private AudioSource audSource1;
    private AudioSource audSource2;

    public float leftRotation;
    public float rightRotation;
    public float audRot1;
    public float audRot2;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(shipRotation1.transform.localRotation.eulerAngles.z);
        sliderLeft = sliderLeftObject.GetComponent<Slider>();
        sliderRight = sliderRightObject.GetComponent<Slider>();
        audSource1 = audSourceObject1.GetComponent<AudioSource>();
        audSource2 = audSourceObject2.GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        sliderRotationObject.transform.position = mapWindowObject.transform.position;
        sliderRight.value = sliderLeft.value;

        rightRotation = sliderRotationObject.transform.localRotation.eulerAngles.z - sliderRight.value;
        leftRotation = sliderRotationObject.transform.localRotation.eulerAngles.z + sliderLeft.value;
        audRot1 = shipRotation1.transform.localRotation.eulerAngles.z;
        audRot2 = shipRotation2.transform.localRotation.eulerAngles.z;

        if((leftRotation > audRot1) && (audRot1 > rightRotation)){
            audSource1.mute = false;
        }
        else{
            audSource1.mute = true;
        }

        if((leftRotation > audRot2) && (audRot2 > rightRotation)){
            audSource2.mute = false;
        }
        else{
            audSource2.mute = true;
        }
    }
}
