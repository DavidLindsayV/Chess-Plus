using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card
{
    private GameObject cardObj;

    public Card()
    {
        makeCard();
    }

    public void makeCard()
    {
        Quaternion rotation = Quaternion.Euler(-90, 0, 0);
        this.cardObj = UnityEngine.Object.Instantiate(
            Prefabs.cardPrefab,
            new Vector3(0, 0, 0),
            rotation
        );
    }

    public GameObject getObj()
    {
        return this.cardObj;
    }

    public abstract List<CardMove> getPieceSpecificMoves(BoardState bState, Piece piece);
    public abstract List<CardMove> getGeneralMoves(BoardState bState);

    public abstract Piece doMoveState(BoardState bState);

    public abstract void showMove(BoardState bState);

    public abstract Coordinate moveTilePos();
}
