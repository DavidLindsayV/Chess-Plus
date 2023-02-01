using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    public Rook(Team team, Coordinate pos)
        : base(team, pos) { }

    public Rook(Team team, Coordinate pos, GameObject gameObj)
        : base(team, pos, gameObj) { }

    public override char typeToChar()
    {
        return 'r';
    }

    public override Piece clonePiece()
    {
        return new Rook(this.getTeam(), this.getPos(), this.getObject());
    }

    //Returns the moves for a Rook
    public override List<Move> getMoves(boardState bState)
    {
        List<Move> moves = new List<Move>();
        int col = this.getPos().getCol();
        int row = this.getPos().getRow();
        for (int i = 1; i < bState.boardSize; i++)
        {
            if (Coordinate.inBounds(col + i, row, bState))
            {
                if (bState.spotNotAlly(this, new Coordinate(col + i, row)))
                {
                    moves.Add(new Move(this, new Coordinate(col + i, row)));
                }
                if (bState.getPiece(col + i, row) != null)
                {
                    break;
                }
            }
        }
        for (int i = 1; i < bState.boardSize; i++)
        {
            if (Coordinate.inBounds(col - i, row, bState))
            {
                if (bState.spotNotAlly(this, new Coordinate(col - i, row)))
                {
                    moves.Add(new Move(this, new Coordinate(col - i, row)));
                }
                if (bState.getPiece(col - i, row) != null)
                {
                    break;
                }
            }
        }
        for (int i = 1; i < bState.boardSize; i++)
        {
            if (Coordinate.inBounds(col, row + i, bState))
            {
                if (bState.spotNotAlly(this, new Coordinate(col, row + i)))
                {
                    moves.Add(new Move(this, new Coordinate(col, row + i)));
                }
                if (bState.getPiece(col, row + i) != null)
                {
                    break;
                }
            }
        }
        for (int i = 1; i < bState.boardSize; i++)
        {
            if (Coordinate.inBounds(col, row - i, bState))
            {
                if (bState.spotNotAlly(this, new Coordinate(col, row - i)))
                {
                    moves.Add(new Move(this, new Coordinate(col, row - i)));
                }
                if (bState.getPiece(col, row - i) != null)
                {
                    break;
                }
            }
        }
        return moves;
    }

    public override List<Move> getAttackingMoves(boardState bState)
    {
        return this.getMoves(bState);
    }
}
