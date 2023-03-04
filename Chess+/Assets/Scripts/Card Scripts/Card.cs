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

    private bool isHighlight = false;

    public Card(Team team) { this.team = team; }

    /**Makes the gameObject for the card. Does not Space it in the hand - merely creates it */
    public void makeCard()
    {
        this.cardObj = UnityEngine.Object.Instantiate(Prefabs.cardPrefab);
        this.cardObj.transform.SetParent(CardUI.canvas.transform, false);
        RectTransform cardRect = this.cardObj.GetComponent<RectTransform>();
        this.cardObj.GetComponent<CardHolder>().setCard(this);
        this.cardObj.GetComponent<Image>().sprite = this.cardSprite;
    }

    public GameObject getObj()
    {
        return this.cardObj;
    }

    public Team getTeam() { return this.team; }

    /**getPieceSpecificMoves takes in the boardState, the Team that is playing the card, and the
Piece it is being played on */
    public abstract List<CardMove> getPieceSpecificMoves(BoardState bState, Piece piece);
    public abstract List<CardMove> getGeneralMoves(BoardState bState);

    /**Each card should have a unique string attached. I'm just gonna name them card1, card2, etc */
    public override string ToString() { return "card" + this.cardNum(); }

    /**Each int should have a unique int, the end of their ToString */
    public abstract int cardNum();


    public void highlight()
    {
        isHighlight = true;
        //this.cardObj.GetComponent<Renderer>().material = Prefabs.highLight; TODO find some way to show highlighting
    }
    /**Returns card to looking normal non highlghted */
    public void dehighlight()
    {
        isHighlight = false;
        //this.cardObj.GetComponent<Renderer>().material = Prefabs.white; TODO find some way to show highlighting
    }

    /**Returns a cloned card but with no gameObject */
    public abstract Card clone();


    /**Destroys the GameObject of the card */
    public void destroyObj()
    {
        UnityEngine.Object.Destroy(cardObj);
    }

    /**Returns whether the card is highlighted or not */
    public bool isHighlighted()
    {
        return isHighlight;
    }
}
