using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardMove : Move
{
    Card card;
    public CardMove(Card card)
    {
        this.card = card;
    }

    /**Removes card from the hand */
    public void removeCardState(BoardState bState)
    {
        bState.getHand(this.card.getTeam()).removeCardState(this.card);
    }

    /**Deletes the gameobject of the card */
    public void removeCardShow()
    {
        this.card.destroyObj();
    }
}
