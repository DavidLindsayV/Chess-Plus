using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardMove: Move
{
    Card card; 
    public CardMove(Card card)
    {
        this.card = card;
    }
}
