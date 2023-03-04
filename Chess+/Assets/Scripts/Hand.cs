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
    public Hand(Card[] cards, Deck deck)
    {
        this.deck = deck;
        if (cards.Length != numCards) { throw new Exception("wrong number of cards in hand"); }
        this.cards = cards;
        positionCards();
    }

    public Hand(Deck deck)
    {
        this.deck = deck;
        cards = new Card[numCards];
        for (int i = 0; i < numCards; i++)
        {
            cards[i] = deck.draw();
        }
        positionCards();
    }

    /**Positions all the cards correctly if none of them are being looked at individually*/
    public void positionCards()
    {
        float totalWidth = 0f; //TODO combine these 2 for loops into 1 for loop
        foreach (Card card in cards)
        {
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
    public List<CardMove> pieceSpecificMoves(BoardState bState, Piece piece)
    {
        List<CardMove> moves = new List<CardMove>();
        foreach (Card c in cards)
        {
            moves.AddRange(c.getPieceSpecificMoves(bState, piece));
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
        foreach (Card c in cards)
        {
            if (c.getPieceSpecificMoves(bState, p).Count != 0)
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
        }
    }

    /**Un-highlights all cards */
    public void dehighlight()
    {
        foreach (Card c in cards)
        {
            c.dehighlight();
        }
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

    /**Destroys all gameobjects of the cards in this hand */
    public void destroyCardObjs()
    {
        foreach (Card c in cards)
        {
            c.destroyObj();
        }
    }

    /**Plays a card. Affects the code and the gameObjects*/
    public void playCard(Card c)
    {
        for (int i = 0; i < numCards; i++)
        {
            if (c == cards[i])
            {
                UnityEngine.Object.Destroy(c.getObj());
                cards[i] = deck.draw();
                return;
            }
        }
        throw new Exception("a card was played that's not in the players hand");
    }

    public Deck getDeck() { return this.deck; }

    public void playCardState(Card card) { } //TODO

    public void playCardShow() { } //TODO
}
