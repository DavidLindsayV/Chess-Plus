using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnPassantMove : PieceMove
{
    public EnPassantMove(Coordinate from, Coordinate to): base(from, to)
    {

    }

    public override Piece doMoveState(BoardState bState){
        base.doMoveState(bState);
            Piece killedPiece = bState.getPiece(
                new Coordinate(this.getTo().getCol(), this.getFrom().getRow())
            );
            bState.setPiece(new Coordinate(this.getTo().getCol(), this.getFrom().getRow()), null);
            return killedPiece;
    }
}
