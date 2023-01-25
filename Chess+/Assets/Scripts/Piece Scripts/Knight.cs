using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    public Knight(Team team, Coordinate pos)
        : base(team, pos) { }

    public Knight(char FENchar, Coordinate pos)
        : base(FENchar, pos) { }

    public Knight(Team team, Coordinate pos, GameObject gameObj)
        : base(team, pos, gameObj) { }

    public override bool isValidMove(boardState bState, Move move)
    {
        //TODO
        return false;
    }

        //Returns the moves for a Knight
    public override List<Move> getValidMoves(boardState bState)
    {
        List<Move> moves = new List<Move>();
        int col = this.getPos().getCol();
        int row = this.getPos().getRow();
        for (int a = -2; a <= 2; a = a + 4)
            for (int b = -1; b <= 1; b = b + 2)
            {
                if (Coordinate.inBounds(col + a, row + b) && bState.spotNotAlly(this, new Coordinate(col + a, row + b)))
                {
                    moves.Add(
                        new Move(this, new Coordinate(col + a, row + b))
                    );
                }
                if (Coordinate.inBounds(col + b, row + a) && bState.spotNotAlly(this, new Coordinate(col + b, row + a)))
                {
                    moves.Add(
                        new Move(this, new Coordinate(col + b, row + a))
                    );
                }
            }
        return moves;
    }

    public override void makePiece()
    {
        base.makePiece();
        if (this.getTeam() == Team.Black)
        {
            this.gameObj.transform.rotation = Quaternion.Euler(-90, 180, 0);
        }
    }
}
