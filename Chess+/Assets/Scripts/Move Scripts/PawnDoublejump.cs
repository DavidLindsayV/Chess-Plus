using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnDoublejump : Move
{
    public PawnDoublejump(Piece movedPiece, Coordinate to) : base(movedPiece, to)
    {

    }

    public override bool isValidMove(boardState state)
    {
        //TODO: check en passant move is valid
        return false;
    }
}
