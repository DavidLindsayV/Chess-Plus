using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Hand
{
    //Fields for the spacing of cards
    private int numCards = 5;
    private float cardSpacing = 10f;
    private float bottomPosition = -400;

    private Deck deck;

    //All the cards in a hand
    Card[] cards;

    /**A constructor that does NOT create gameObjects */
    public Hand(Card[] cards, Deck deck)
    {
        this.deck = deck;
        if (cards.Length != numCards) { throw new Exception("wrong number of cards in hand"); }
        this.cards = cards;
    }

    /**A constructor that DOES create gameObjects */
    public Hand(Deck deck)
    {
        this.deck = deck;
        cards = new Card[numCards];
        for (int i = 0; i < numCards; i++)
        {
            cards[i] = deck.draw();
            cards[i].handIndex = i;
        }
    }

    /**Positions all the cards correctly if none of them are being looked at individually*/
    public void positionCards()
    {
        float totalWidth = 0f; //TODO combine these 2 for loops into 1 for loop
        foreach (Card card in cards)
        {
            if (card.getObj() == null) { return; } //If this hand has no gameObjects for its cards (is the enemy hand), return
            GameObject cardObj = card.getObj();
            RectTransform buttonRect = cardObj.GetComponent<RectTransform>();
            totalWidth += buttonRect.rect.width * 2 + cardSpacing;
        }
        // Calculate the x position of the first button based on the total width and the canvas size
        float startX = UnityEngine.Object.FindObjectOfType<Canvas>().pixelRect.width / 2f - totalWidth / 2f;
        //TODO change the way you get canvas above, so you don't have to find the canvas lots of separate times throughout different code files
        // Instantiate the button prefabs and position them at the bottom of the canvas
        for (int i = 0; i < numCards; i++)
        {
            Card card = cards[i];
            GameObject cardObj = card.getObj();
            RectTransform cardRect = cardObj.GetComponent<RectTransform>();
            cardRect.anchoredPosition = new Vector2((i * (cardRect.rect.width * 2f)) + (cardSpacing * i) - startX, bottomPosition);
            CardUI cardscript = card.getObj().GetComponent<CardUI>();
            cardscript.setOriginalPosition(cardRect.anchoredPosition);
        }
    }

    /**reutrns the moves that need a piece to do them */
    public List<CardMove> coordSpecificMoves(BoardState bState, Coordinate coor)
    {
        List<CardMove> moves = new List<CardMove>();
        foreach (Card c in cards)
        {
            moves.AddRange(c.getCoordSpecificMoves(bState, coor));
        }
        return moves;
    }

    /**Returns the moves that do not need a specific piece to do them */
    public List<CardMove> generalMoves(BoardState bState)
    {
        List<CardMove> moves = new List<CardMove>();
        foreach (Card c in cards)
        {
            moves.AddRange(c.getGeneralMoves(bState));
        }
        return moves;
    }

    /**Highlight all the cards that might be options to play with the piece selected */
    public void showCardOptions(BoardState bState, Piece p)
    {
        //TODO
    }

    /**Creates a clone of this hand but the cards don't have gameObjects */
    public Hand clone()
    {
        Deck cloneDeck = this.deck.clone();
        Card[] cardsClone = new Card[numCards];
        for (int i = 0; i < numCards; i++)
        {
            Card card = cards[i].clone();
            cardsClone[i] = card;
        }
        return new Hand(cardsClone, cloneDeck);
    }

    /**Makes all gameobjects for the cards in this hand */
    public void makeCardObjs()
    {
        foreach (Card c in cards)
        {
            c.makeCard();
        }
        positionCards();
    }

    public Deck getDeck() { return this.deck; }

    /**Updates the Hand's state. Does not affect gameObjects */
    public void playCardState(Card card)
    {
        int handIndex = card.handIndex;
        cards[handIndex] = deck.draw();
        cards[handIndex].handIndex = handIndex;
    }

    /**Updates the visuals of the Hand after a card has been played*/
    public void playCardShow()
    {
        foreach (Card c in cards)
        {
            if (c.getObj() == null) { c.makeCard(); }
        }
        positionCards();
    }
}
