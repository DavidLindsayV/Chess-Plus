using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMove: Move
{
    Card card; 
    public CardMove(Card card)
    {
        this.card = card;
    }

    public override Piece doMoveState(BoardState bState){
        return this.card.doMoveState(bState);
    }

    public override void showMove(BoardState bState, Piece killedPiece){
        this.card.showMove(bState);
    }

    public override bool inDanger(Coordinate pos){
        return false; //TODO fix this to actually check if a position is in danger
    }

    public override Coordinate moveTilePos()
    {
        return card.moveTilePos(); //TODO fix all of Card and make it actually do stuff
    }
}
