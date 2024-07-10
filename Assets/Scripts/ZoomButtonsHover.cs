using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ZoomButtonsHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float hoverScaleFactor = 1.1f;
    private Color normalColor = Color.gray;
    private Color hoverColor = Color.white;

    private Vector3 originalScale;
    private Image buttonImage;

    void Awake()
    {
        originalScale = transform.localScale;
        buttonImage = GetComponent<Image>();
        buttonImage.color = normalColor;
        SetNormalState();
        Debug.Log("Here");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetHoverState();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetNormalState();
    }

    private void SetHoverState()
    {
        transform.localScale = originalScale * hoverScaleFactor;
        buttonImage.color = hoverColor;
        Debug.Log("enter hover");
    }

    private void SetNormalState()
    {
        Debug.Log("enter normal");
        transform.localScale = originalScale;
        buttonImage.color = normalColor;
    }
}
