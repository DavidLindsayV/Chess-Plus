using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Hand
{
    List<Card> cards;
    public Hand()
    {
        cards = new List<Card>();
    }

    public Hand(Deck deck, int cardNum)
    {
        cards = new List<Card>();
        for (int i = 0; i < cardNum; i++)
        {
            Pickup(deck.draw());
        }
        positionCards();
    }

    //Positions all the cards correctly if none of them are being looked at individually
    public void positionCards()
    {
        int cardNum = cards.Count;
        int angleSpread = 20;
        float angle = 0;
        float step = 0;
        float zStep = 0.1f;
        float z = zStep*cardNum;
        if (cardNum > 1)
        {
            angle = -angleSpread / 2;
            step = angleSpread / (cardNum - 1);
        }
        foreach (Card card in cards)
        {
            GameObject cardObj = card.getObj();
            if (cardObj == null) { continue; }
            cardObj.transform.SetParent(Camera.main.transform);
            //x = rsin angle
            //y = rcos angle
            float radius = 10;
            cardObj.transform.localPosition = new Vector3((float)(radius * Math.Sin(angle * Math.PI / 180f)), (float)(radius * Math.Cos(angle * Math.PI / 180f) - 12), 5+z);
            cardObj.transform.localRotation = Quaternion.Euler(90 + angle, 90, -90);
            angle += step;
            z -= zStep;
        }
    }

    /**reutrns the moves that need a piece to do them */
    public List<CardMove> pieceSpecificMoves(BoardState bState, Team team, Piece piece)
    {
        List<CardMove> moves = new List<CardMove>();
        foreach (Card c in cards)
        {
            moves.AddRange(c.getPieceSpecificMoves(bState, team, piece));
        }
        return moves;
    }

    /**Returns the moves that do not need a specific piece to do them */
    public List<CardMove> generalMoves(BoardState bState, Team team)
    {
        List<CardMove> moves = new List<CardMove>();
        foreach (Card c in cards)
        {
            moves.AddRange(c.getGeneralMoves(bState, team));
        }
        return moves;
    }

    /**Picks up a card specified for you */
    public void Pickup(Card card)
    {
        cards.Add(card);
        positionCards();
    }

    /**Highlight all the cards that might be options to play with the piece selected */
    public void showCardOptions(BoardState bState, Piece p)
    {
        foreach (Card c in cards)
        {
            if (c.getPieceSpecificMoves(bState, p.getTeam(), p).Count != 0)
            {
                highlight(c);
            }
        }
    }

    public void highlight(Card c)
    {
        if (!c.isHighlighted())
        {
            c.highlight();
            c.getObj().transform.localPosition += new Vector3(0, 1.5f, 0);
        }
    }

    /**Un-highlights all cards */
    public void dehighlight()
    {
        foreach (Card c in cards)
        {
            c.dehighlight();
        }
        positionCards();
    }

    /**Creates a clone of this hand but the cards don't have gameObjects */
    public Hand clone()
    {
        Hand h = new Hand();
        foreach (Card c in cards)
        {
            Card card = c.clone();
            h.Pickup(card);
        }
        return h;
    }

    /**Destroys all gameobjects of the cards in this hand */
    public void destroyCardObjs()
    {
        foreach (Card c in cards)
        {
            c.destroyObj();
        }
    }

    /**Does the non-gameObject actions of removing a card from this hand*/
    public void removeCardState(Card c)
    {
        this.cards.Remove(c);
    }
}
