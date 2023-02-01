using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastlingMove : Move
{
    public Move rookMove;
    public CastlingMove(Coordinate kingFrom, Coordinate rookFrom, Coordinate kingTo, Coordinate rookTo): base(kingFrom, kingTo) { 
        rookMove = new Move(rookFrom, rookTo);
     }

    public override Piece doMoveState(boardState bState){
        base.doMoveState(bState);
            bState.setCastle(this.getPiece(bState).getTeam(), true, false);
            bState.setCastle(this.getPiece(bState).getTeam(), false, false);
            //Does the rook-moving part of Castling. No pieces will be killed by this, so the return value
            //can be ignored
            this.rookMove.doMoveState(bState);
            return null; //castling moves can't kill
    }
}
