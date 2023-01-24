using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastlingMove : Move
{
    public Move rookMove;
    public CastlingMove(Piece kingPiece, Piece rookPiece, Coordinate kingTo, Coordinate rookTo): base(kingPiece, kingTo) { 
        rookMove = new Move(rookPiece, rookTo);
     }

    public override bool isValidMove(boardState state)
    {
        //TODO: check if castling move is valid
        return false;
    }
}
