using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{

    public Pawn(Team team, Coordinate pos, pieceType type): base(team, pos, type)
    {

    }

    public Pawn(char FENchar, Coordinate pos): base(FENchar, pos)
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
