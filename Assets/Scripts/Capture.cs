using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capture : MonoBehaviour
{
    public Sprite newSprite;
    private Sprite originalSprite;

    private SpriteRenderer spriteRenderer;
    public TabManager tabManager;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSprite = spriteRenderer.sprite;
    }

    void OnMouseDown()
    {
        tabManager.Capture();
        StartCoroutine(ChangeSpriteTemporarily());
    }

    IEnumerator ChangeSpriteTemporarily()
    {
        //change the sprite to the new one
        spriteRenderer.sprite = newSprite;
        yield return new WaitForSeconds(1f);
        //change back to the original sprite
        spriteRenderer.sprite = originalSprite;
    }
}