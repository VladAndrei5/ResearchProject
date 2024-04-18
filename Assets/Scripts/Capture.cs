using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capture : MonoBehaviour
{
    public Sprite newSprite; // The sprite to replace with
    private Sprite originalSprite; // The original sprite

    private SpriteRenderer spriteRenderer;
    public LedgerUI ledgerUI;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Save the original sprite
        originalSprite = spriteRenderer.sprite;
    }

    void OnMouseDown()
    {
        // Start the coroutine to change the sprite
        StartCoroutine(ChangeSpriteTemporarily());
    }

    IEnumerator ChangeSpriteTemporarily()
    {
        // Change the sprite to the new one
        spriteRenderer.sprite = newSprite;
        ledgerUI.Capture();

        // Wait for 1 second
        yield return new WaitForSeconds(1f);

        // Change back to the original sprite after 1 second
        spriteRenderer.sprite = originalSprite;
    }
}