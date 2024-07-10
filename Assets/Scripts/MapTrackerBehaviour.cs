using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTrackerBehaviour : MonoBehaviour
{

    public BearingTrackerBehaviour bTrack;
    public SoundSourceBehaviour soundSourcePair;
    public MapTrackerChildrenControl mapTrackerChildrenControl;
    public Utilities utilities;

    public SpriteRenderer spriteRenderer;

    public void InitaliseBehaviour(BearingTrackerBehaviour bTrack, SoundSourceBehaviour soundSourcePair){
        GameObject obj = GameObject.FindWithTag("Utilities");
        utilities = obj.GetComponent<Utilities>();

        this.bTrack = bTrack;
        this.soundSourcePair = soundSourcePair;

        mapTrackerChildrenControl.Init( this.bTrack ,  this.soundSourcePair);
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition(soundSourcePair.getGameObject());
    }

    public void UpdatePosition(GameObject soundSource){
        float soundRot = utilities.getSoundSourceAngle(soundSource);
        
        Vector3 localRotation = new Vector3(0f, 0f, soundRot);
        //Vector3 currentPosition = transform.position;
        //transform.localPosition = new Vector3(soundRot * -1f, currentPosition.y, currentPosition.z);
        transform.localRotation = Quaternion.Euler(localRotation);
    }

    public void Despawn(){
        Destroy(gameObject);
    }
}
