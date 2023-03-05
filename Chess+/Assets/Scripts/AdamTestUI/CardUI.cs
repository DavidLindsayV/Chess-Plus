using UnityEngine;
using UnityEngine.EventSystems;

/**A script for handling the movement and dragging of Cards */
public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Vector2 offset;
    public static Canvas canvas = null;

    private static Game game;

    private Card card;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (canvas == null) { canvas = FindObjectOfType<Canvas>(); }
        if (game == null) { game = FindObjectOfType<Game>(); }
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
        game.selectCard(this.getCard());
        Vector2 mouseScreenOffset = new Vector2((canvas.GetComponent<RectTransform>().rect.height - GetComponent<RectTransform>().rect.height), (canvas.GetComponent<RectTransform>().rect.width) / 2.0f - (GetComponent<RectTransform>().rect.height * 2.0f));
        rectTransform.anchoredPosition = eventData.position - mouseScreenOffset + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        game.deselectCard();
        rectTransform.anchoredPosition = originalPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        offset = eventData.position - rectTransform.anchoredPosition;
    }

    public void setOriginalPosition(Vector2 position) { this.originalPosition = position; }

    public void setCard(Card c) { this.card = c; }

    public Card getCard() { return this.card; }
}
