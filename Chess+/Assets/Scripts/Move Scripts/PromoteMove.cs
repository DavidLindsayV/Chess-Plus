using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromoteMove : Move
{
    public Piece promotedTo;

    public PromoteMove(Coordinate from, Coordinate to, Piece promotedTo)
        : base(from, to)
    {
        this.promotedTo = promotedTo;
        this.promotedTo.destroy();
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
