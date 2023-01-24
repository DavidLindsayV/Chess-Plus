using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
    public Bishop(Team team, Coordinate pos) : base(team, pos, Piece.pieceType.Bishop)
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

    public override List<Move> getValidMoves(boardState state)
    {
        //TODO
        return null;
    }

    protected override void makePiece()
    {
        base.makePiece();
        Vector3 vec = this.gameObj.transform.position;
        vec.y = 0.5F;
        this.gameObj.transform.position = vec;
    }
}
