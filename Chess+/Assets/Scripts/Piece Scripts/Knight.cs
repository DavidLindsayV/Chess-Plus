using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    public Knight(Team team, Coordinate pos) : base(team, pos, Piece.pieceType.Knight)
    {

    }

    public Knight(char FENchar, Coordinate pos) : base(FENchar, pos)
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
        if(this.getTeam() == Team.Black)
        {
            this.gameObj.transform.rotation = Quaternion.Euler(-90, 180, 0);
        }
    }
}
