using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnDoublejump : Move
{
    public PawnDoublejump(Piece movedPiece, Coordinate to) : base(movedPiece, to)
    {

    }

    public override Piece doMoveState(boardState bState){
        Piece killedPiece = base.doMoveState(bState);
            int direction = 0;
            if (this.movedPiece.getTeam() == Team.White)
            {
                direction = -1;
            }
            else
            {
                direction = +1;
            }
            bState.setEnPassant(
                new Coordinate(this.getTo().getCol(), this.getTo().getRow() + direction)
            );
            return killedPiece;
    }
}
