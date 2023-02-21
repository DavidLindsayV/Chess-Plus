using UnityEngine;

/**A script so that Card-GameObjects can store information about which Card they are*/
public class CardObjScript : MonoBehaviour
{
    //Stores the Card the game object is associated with
    private Card card;

    /**Sets the Move */
    public void setCard(Card card)
    {
        this.card = card;
    }

    /** Returns the Card */
    public Card getCard()
    {
        return card;
    }
}
