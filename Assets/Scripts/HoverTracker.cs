using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverTracker : MonoBehaviour
{
    public float outlineWidth = 0.1f;
    public Material previousMaterial;
    public Material hoverMaterial;

    private float scaleIncrease = 0.5f;
    private float yShift = 3.3f;
    public float activationDistance = 2f;

    private Vector3 originalScale;
    private Vector3 originalPosition;

    public BearingTrackerBehaviour bearingTrackerBehaviour;
    private bool isHovering = false;

    private SpriteRenderer spriteRenderer;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        previousMaterial = spriteRenderer.material;

        originalScale = transform.localScale;
        originalPosition = transform.position;
        isHovering = false;
        hoverMaterial = new Material(Shader.Find("Custom/HoverMaterial"));
    }

    /*
    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z; // Ensure we're comparing in the same 2D plane

        float distanceToMouse = Vector3.Distance(transform.position, mousePosition);

        if (isHovering && distanceToMouse <= activationDistance)
        {
            ApplyHoverEffect();
        }
        else
        {
            RemoveHoverEffect();
        }
    }
    */

    void OnMouseEnter()
    {
        isHovering = true;
        if(spriteRenderer.enabled){
            if(spriteRenderer.material != hoverMaterial){
                previousMaterial = spriteRenderer.material;
            }

            //spriteRenderer.material = hoverMaterial;
            transform.localScale = originalScale * (1 + scaleIncrease);
            transform.position = originalPosition + new Vector3(0, yShift, 0);
        }
    }

    void OnMouseExit()
    {
        isHovering = false;
        if(spriteRenderer.enabled){
            //spriteRenderer.material = previousMaterial;
            transform.localScale = originalScale;
            transform.position = originalPosition;
        }
    }
    /*
    void ApplyHoverEffect()
    {
        if(spriteRenderer.enabled){
            spriteRenderer.material = hoverMaterial;
            transform.localScale = originalScale * (1 + scaleIncrease);
            transform.position = originalPosition + new Vector3(0, yShift, 0);
        }
    }

    void RemoveHoverEffect()
    {
        if(spriteRenderer.enabled){
            spriteRenderer.material = previousMaterial;
            transform.localScale = originalScale;
            transform.position = originalPosition;
        }
    }
    */
}
