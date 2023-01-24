using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{
    public Queen(Team team, Coordinate pos) : base(team, pos, Piece.pieceType.Queen)
    {

    }

    public Queen(char FENchar, Coordinate pos) : base(FENchar, pos)
    {
    }
    public override bool isValidMove(boardState state, Move move)
    {
        //TODO
        return false;
    }

    public override List<Move> getValidMoves(boardState state)
    {
        //TODO
        return null;
    }
}
