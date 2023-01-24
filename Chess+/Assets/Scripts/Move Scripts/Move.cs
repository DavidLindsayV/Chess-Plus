using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**Move stores within it one move a chess piece can do. */
public class Move
{
    protected Piece movedPiece; //The Piece chess piece that is being moved
    protected Coordinate to; //The Coordinate of the spot its being moved to

    /**Constructor function */
    public Move(Piece movedPiece, Coordinate to)
    {
        this.movedPiece = movedPiece;
        this.to = to;
    }

    /**gets end coordinate */
    public Coordinate getTo() { return this.to;  }

    /** Gets moved piece */
    public Piece getPiece() { return this.movedPiece; }

    /**Checks if a move is valid */
    public virtual bool isValidMove(boardState state)
    {
        //TODO: do checks that don't depend on the piece checking if the move is valid
        //check state isn't null, row and col are valid numbers, etc
        return movedPiece.isValidMove(state, this);
    }

    public void doMove(boardState state)
    {
        if (this.isValidMove(state))
        {
            //TODO
        }
    }
}
