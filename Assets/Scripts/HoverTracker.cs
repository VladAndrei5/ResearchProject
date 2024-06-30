using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverTracker : MonoBehaviour
{
    public Color outlineColor = Color.white;
    public float outlineWidth = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Material outlineMaterial;
    private Material defaultMaterial;

    private float scaleIncrease = 0.5f;
    private float yShift = 3.3f;

    private Vector3 originalScale;
    private Vector3 originalPosition;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;

        // Create a new material with the outline shader
        outlineMaterial = new Material(Shader.Find("Custom/SpriteOutline"));
        outlineMaterial.SetColor("_OutlineColor", outlineColor);
        outlineMaterial.SetFloat("_OutlineWidth", outlineWidth);

        originalScale = transform.localScale;
        originalPosition = transform.position;
    }

    void OnMouseEnter()
    {
        if(spriteRenderer.enabled){
            spriteRenderer.material = outlineMaterial;
            transform.localScale = originalScale * (1 + scaleIncrease);
            transform.position = originalPosition + new Vector3(0, yShift, 0);
        }
    }

    void OnMouseExit()
    {
        if(spriteRenderer.enabled){
            spriteRenderer.material = defaultMaterial;
            transform.localScale = originalScale;
            transform.position = originalPosition;
        }
    }
}
