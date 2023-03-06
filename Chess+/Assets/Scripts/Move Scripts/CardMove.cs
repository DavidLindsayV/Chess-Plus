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

    /**Updates the state of the Hand after a card has been played */
    public void playCardState(BoardState bState)
    {
        bState.getHand(this.card.getTeam()).playCardState(this.card);
    }

    /**Updates the gameObjects of the card after it has been played (destroy the card obj)*/
    public void playCardShow(BoardState bState)
    {
        if (bState.playersTeam() == this.card.getTeam())
        {
            this.card.destroyObj();
            bState.getHand(this.card.getTeam()).playCardShow();
        }
    }
}
