using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnDoublejump : Move
{
    public PawnDoublejump(Coordinate from, Coordinate to) : base(from, to)
    {

    }

    public override Piece doMoveState(BoardState bState){
        Piece killedPiece = base.doMoveState(bState);
            int direction = 0;
            if (this.getPiece(bState).getTeam() == Team.White)
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
