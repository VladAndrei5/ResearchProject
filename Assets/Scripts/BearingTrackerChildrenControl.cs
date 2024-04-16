using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearingTrackerChildrenControl : MonoBehaviour
{
    public SpriteRenderer parentRenderer;
    public SpriteRenderer childRenderer;

    private Color lastColor;

    void Start()
    {
        if (parentRenderer == null)
        {
            Debug.LogError("Parent SpriteRenderer is not assigned!");
            return;
        }

        if (childRenderer == null)
        {
            Debug.LogError("Child SpriteRenderer is not assigned!");
            return;
        }

        // Get the initial color of the parent sprite renderer
        lastColor = parentRenderer.color;
    }

    void Update()
    {
        // Check if the color of the parent sprite renderer has changed
        if (parentRenderer.color != lastColor)
        {
            // Update the color of the child sprite renderer to match the parent
            childRenderer.color = parentRenderer.color;
            
            // Update the last color to the new color
            lastColor = parentRenderer.color;
        }

        if(parentRenderer.enabled == false){
            childRenderer.enabled = false;
        }
        else{
            childRenderer.enabled = true;
        }
    }
}
