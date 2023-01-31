using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnPassantMove : Move
{
    public EnPassantMove(Piece movedPiece, Coordinate to): base(movedPiece, to)
    {

    }

    public override Piece doMoveState(boardState bState){
        base.doMoveState(bState);
            Piece killedPiece = bState.getPiece(
                new Coordinate(this.getTo().getCol(), this.getFrom().getRow())
            );
            bState.setPiece(new Coordinate(this.getTo().getCol(), this.getFrom().getRow()), null);
            return killedPiece;
    }
}
