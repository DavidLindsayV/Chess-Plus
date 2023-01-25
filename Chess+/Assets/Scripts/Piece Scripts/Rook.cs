using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    //TODO see if you can delete these 2 constructors as they add nothing
    public Rook(Team team, Coordinate pos)
        : base(team, pos) { }

    public Rook(char FENchar, Coordinate pos)
        : base(FENchar, pos) { }

    public Rook(Team team, Coordinate pos, GameObject gameObj)
        : base(team, pos, gameObj) { }

    public override bool isValidMove(boardState bState, Move move)
    {
        //TODO
        return false;
    }

    //Returns the moves for a Rook
    public override List<Move> getValidMoves(boardState bState)
    {
        List<Move> moves = new List<Move>();
        int col = this.getPos().getCol();
        int row = this.getPos().getRow();
        for (int i = 1; i < 8; i++)
        {
            if (Coordinate.inBounds(col + i, row))
            {
                if (bState.spotNotAlly(this, new Coordinate(col + i, row)))
                {
                    moves.Add(
                        new Move(this, new Coordinate(col + i, row))
                    );
                }
                if (bState.getPiece(col + i, row) != null)
                {
                    break;
                }
            }
        }
        for (int i = 1; i < 8; i++)
        {
            if (Coordinate.inBounds(col - i, row))
            {
                if (bState.spotNotAlly(this, new Coordinate(col - i, row)))
                {
                    moves.Add(
                        new Move(this, new Coordinate(col - i, row))
                    );
                }
                if (bState.getPiece(col - i, row) != null)
                {
                    break;
                }
            }
        }
        for (int i = 1; i < 8; i++)
        {
            if (Coordinate.inBounds(col, row + i))
            {
                if (bState.spotNotAlly(this, new Coordinate(col, row + i)))
                {
                    moves.Add(
                        new Move(this, new Coordinate(col, row + i))
                    );
                }
                if (bState.getPiece(col, row + i) != null)
                {
                    break;
                }
            }
        }
        for (int i = 1; i < 8; i++)
        {
            if (Coordinate.inBounds(col, row - i))
            {
                if (bState.spotNotAlly(this, new Coordinate(col, row - i)))
                {
                    moves.Add(
                        new Move(this, new Coordinate(col, row - i))
                    );
                }
                if (bState.getPiece(col, row - i) != null)
                {
                    break;
                }
            }
        }
        return moves;
    }
}
