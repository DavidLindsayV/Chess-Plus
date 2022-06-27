using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromoteMove : Move
{
    Piece promotedTo;
    public PromoteMove(Piece movedPiece, Coordinate to, Piece promotedTo) : base(movedPiece, to)
    {
        this.promotedTo = promotedTo;
    }

    public override bool isValidMove(boardState state)
    {
        if (!base.isValidMove(state)) { return false; }
        //TODO: do custom checks to make sure the promotion move is valid
        //(promoted to right types and anything else you can think of)
        return false;
    }
}
