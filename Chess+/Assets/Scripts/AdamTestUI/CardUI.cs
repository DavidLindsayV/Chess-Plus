using UnityEngine;
using UnityEngine.EventSystems;

/**A script for handling the movement and dragging of Cards */
public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Vector2 offset;
    public static Canvas canvas = null;
    public static Camera cam = null;

    private static Game game;

    private Card card;

    private GameObject moveTile;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (canvas == null) { canvas = FindObjectOfType<Canvas>(); }
        if (game == null) { game = FindObjectOfType<Game>(); }
        if (cam == null) { cam = FindObjectOfType<Camera>(); }
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
        //If the card is being dragged above a move tile
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("Move tiles");
        if (Physics.Raycast(
            ray,
            out hit,
            1000,
            mask
        ))
        {
            if (moveTile != null) { dehighlightTile(); }
            moveTile = hit.collider.gameObject;
            highlightTile();
        }
        else
        {
            if (moveTile != null) { dehighlightTile(); moveTile = null; }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (moveTile != null)
        {
            game.selectTile(moveTile.GetComponent<MoveHolder>());
        }
        else
        {
            game.deselectCard();
        }
        rectTransform.anchoredPosition = originalPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        offset = eventData.position - rectTransform.anchoredPosition;
    }

    public void setOriginalPosition(Vector2 position) { this.originalPosition = position; }

    public void setCard(Card c) { this.card = c; }

    public Card getCard() { return this.card; }

    private void highlightTile()
    {
        this.moveTile.GetComponent<Renderer>().material = Prefabs.highlight2;
    }

    private void dehighlightTile()
    {
        this.moveTile.GetComponent<Renderer>().material = Prefabs.highLight;
    }
}
