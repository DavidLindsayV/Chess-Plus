using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
    public Bishop(Team team, Coordinate pos, pieceType type) : base(team, pos, type)
    {

    }

    public Bishop(char FENchar, Coordinate pos) : base(FENchar, pos)
    {
    }

    public override bool isValidMove(boardState state, Move move)
    {
        //TODO
        return false;
    }

    public override Move[] getValidMoves(boardState state)
    {
        //TODO
        return null;
    }
}
