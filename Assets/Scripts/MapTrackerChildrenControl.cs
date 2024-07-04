using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTrackerChildrenControl : MonoBehaviour
{
    public BearingTrackerBehaviour bTrack;
    public SoundSourceBehaviour soundSourcePair;
    public Utilities utilities;

    public SpriteRenderer spriteRenderer;

    public void Init(BearingTrackerBehaviour bTrack, SoundSourceBehaviour soundSourcePair){
        GameObject obj = GameObject.FindWithTag("Utilities");
        utilities = obj.GetComponent<Utilities>();

        this.bTrack = bTrack;
        this.soundSourcePair = soundSourcePair;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(bTrack.spriteRenderer.enabled ){
            spriteRenderer.enabled = true;
            spriteRenderer.sprite = bTrack.spriteRenderer.sprite;
            spriteRenderer.material = bTrack.spriteRenderer.material;
        }
        else{
            spriteRenderer.enabled = false;
        }
    }
}
