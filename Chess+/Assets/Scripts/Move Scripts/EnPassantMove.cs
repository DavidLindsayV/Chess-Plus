using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnPassantMove : Move
{
    public EnPassantMove(Piece movedPiece, Coordinate to): base(movedPiece, to)
    {

    }

    public override bool isValidMove(boardState state)
    {
        //TODO: check en passant move is valid
        return false;
    }
}
