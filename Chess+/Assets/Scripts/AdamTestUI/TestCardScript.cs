using UnityEngine;
using UnityEngine.EventSystems;

public class TestCardScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Vector2 offset;
    public Canvas canvas;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        canvas = FindObjectOfType<Canvas>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.anchoredPosition = originalPosition + new Vector2(0f, 200f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.anchoredPosition = originalPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 mouseScreenOffset = new Vector2((canvas.GetComponent<RectTransform>().rect.height - GetComponent<RectTransform>().rect.height),(canvas.GetComponent<RectTransform>().rect.width)/2.0f - (GetComponent<RectTransform>().rect.height*2.0f));
        rectTransform.anchoredPosition = eventData.position - mouseScreenOffset + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition = originalPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        offset = eventData.position - rectTransform.anchoredPosition;
    }
}
