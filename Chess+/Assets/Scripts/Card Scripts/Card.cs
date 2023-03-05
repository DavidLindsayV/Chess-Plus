using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**The base class for Cards. This abstract class just deals with the base gameObject sutff
and sets up abstract functions for subclasses to fill out.
Note to self: A Card is able to generate a large number of possible moves, a CardMove is only one 
of those possible moves */
public abstract class Card
{
    protected GameObject cardObj;
    protected Sprite cardSprite;

    private Team team;

    public Card(Team team) { this.team = team; }

    /**Makes the gameObject for the card. Does not Space it in the hand - merely creates it */
    public void makeCard()
    {
        this.cardObj = UnityEngine.Object.Instantiate(Prefabs.cardPrefab);
        this.cardObj.transform.SetParent(CardUI.canvas.transform, false);
        RectTransform cardRect = this.cardObj.GetComponent<RectTransform>();
        this.cardObj.GetComponent<CardHolder>().setCard(this);
        this.cardObj.GetComponent<Image>().sprite = this.cardSprite;
        this.cardObj.GetComponent<CardUI>().setCard(this);
    }

    public GameObject getObj()
    {
        return this.cardObj;
    }

    public Team getTeam() { return this.team; }

    /**getCoordSpecificMoves takes in the boardState, the Team that is playing the card, and the
coord it is being played on */
    public abstract List<CardMove> getCoordSpecificMoves(BoardState bState, Coordinate coor);

    /**Returns moves that don't need a specific position to be played on */
    public abstract List<CardMove> getGeneralMoves(BoardState bState);

    //Returns whether or not this Card can be played on a certain coordinate
    public abstract bool canPlayOnPos(BoardState bState, Coordinate coor);

    /**Each card should have a unique string attached. I'm just gonna name them card1, card2, etc */
    public override string ToString() { return "card" + this.cardNum(); }

    /**Each int should have a unique int, the end of their ToString */
    public abstract int cardNum();



    /**Returns a cloned card but with no gameObject */
    public abstract Card clone();


    /**Destroys the GameObject of the card */
    public void destroyObj()
    {
        UnityEngine.Object.Destroy(cardObj);
    }
}
