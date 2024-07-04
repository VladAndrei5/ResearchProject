using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capture : MonoBehaviour
{
    public Sprite newSprite;
    private Sprite originalSprite;

    private SpriteRenderer spriteRenderer;
    public TabManager tabManager;

    public GameObject captureText;
    private Vector3 textOriginalPosition;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSprite = spriteRenderer.sprite;
        textOriginalPosition = captureText.transform.position;
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
        captureText.transform.position = textOriginalPosition + new Vector3(0, -5f, 0);
        yield return new WaitForSeconds(1f);
        //change back to the original sprite
        captureText.transform.position = textOriginalPosition;
        spriteRenderer.sprite = originalSprite;
    }
}