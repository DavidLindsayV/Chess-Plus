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
    public void playCardState(BoardState bState)
    {
        bState.getHand(this.card.getTeam()).playCardState(this.card);
    }

    public void playCardShow(){} //TODO
}
