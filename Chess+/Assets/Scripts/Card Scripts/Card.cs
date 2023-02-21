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
    private GameObject cardObj;
    protected string CardText = "";

    public void makeCard()
    {
        Quaternion rotation = Quaternion.Euler(-90, 0, 0);
        this.cardObj = UnityEngine.Object.Instantiate(
            Prefabs.cardPrefab,
            new Vector3(0, 0, 0),
            rotation
        );
        this.cardObj.GetComponent<CardObjScript>().setCard(this);
        Text text = this.cardObj.transform.Find("Canvas").transform.Find("Text").gameObject.GetComponent<Text>();
        text.text = this.CardText;
    }

    public GameObject getObj()
    {
        return this.cardObj;
    }

    /**getPieceSpecificMoves takes in the boardState, the Team that is playing the card, and the
Piece it is being played on */
    public abstract List<CardMove> getPieceSpecificMoves(BoardState bState, Team team, Piece piece);
    public abstract List<CardMove> getGeneralMoves(BoardState bState);
}
