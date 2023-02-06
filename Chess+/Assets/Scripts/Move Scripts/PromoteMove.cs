using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromoteMove : Move
{
    //promotedTo should be a Piece (rook/queen/knight/bishop) that does NOT have a gameObject
    public Piece promotedTo;

    public PromoteMove(Coordinate from, Coordinate to, Piece promotedTo)
        : base(from, to)
    {
        this.promotedTo = promotedTo; 
    }

    //Makes the promoted piece visible
    public void makePromotedPiece()
    {
        this.promotedTo.makePiece();
    }

    public override Piece doMoveState(boardState bState){
        Piece killedPiece = base.doMoveState(bState);
        bState.setPiece(this.getTo(), this.promotedTo);
        return killedPiece;
    }
}
