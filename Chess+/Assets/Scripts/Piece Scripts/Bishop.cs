using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
    public Bishop(Team team, Coordinate pos)
        : base(team, pos) { }

    public Bishop(char FENchar, Coordinate pos)
        : base(FENchar, pos) { }

    public Bishop(Team team, Coordinate pos, GameObject gameObj)
        : base(team, pos, gameObj) { }

    public override bool isValidMove(boardState state, Move move)
    {
        //TODO
        return false;
    }

    //Returns the moves for a Bishop
    public override List<Move> getValidMoves(boardState bState)
    {
        List<Move> moves = new List<Move>();
        int col = this.getPos().getCol();
        int row = this.getPos().getRow();
        for (int i = 1; i < 8; i++)
        {
            if (Coordinate.inBounds(col + i, row + i))
            {
                if (bState.spotNotAlly(this, new Coordinate(col + i, row + i)))
                {
                    moves.Add(
                        new Move(this, new Coordinate(col + i, row + i))
                    );
                }
                if (bState.getPiece(col + i, row + i) != null)
                {
                    break;
                }
            }
        }
        for (int i = 1; i < 8; i++)
        {
            if (Coordinate.inBounds(col - i, row - i))
            {
                if (bState.spotNotAlly(this, new Coordinate(col - i, row - i)))
                {
                    moves.Add(
                        new Move(this, new Coordinate(col - i, row - i))
                    );
                }
                if (bState.getPiece(col - i, row - i) != null)
                {
                    break;
                }
            }
        }
        for (int i = 1; i < 8; i++)
        {
            if (Coordinate.inBounds(col - i, row + i))
            {
                if (bState.spotNotAlly(this, new Coordinate(col - i, row + i)))
                {
                    moves.Add(
                        new Move(this, new Coordinate(col - i, row + i))
                    );
                }
                if (bState.getPiece(col - i, row + i) != null)
                {
                    break;
                }
            }
        }
        for (int i = 1; i < 8; i++)
        {
            if (Coordinate.inBounds(col + i, row - i))
            {
                if (bState.spotNotAlly(this, new Coordinate(col + i, row - i)))
                {
                    moves.Add(
                        new Move(this, new Coordinate(col + i, row - i))
                    );
                }
                if (bState.getPiece(col + i, row - i) != null)
                {
                    break;
                }
            }
        }
        return moves;
    }

    public override void makePiece()
    {
        base.makePiece();
        Vector3 vec = this.gameObj.transform.position;
        vec.y = 0.5F;
        this.gameObj.transform.position = vec;
    }
}
