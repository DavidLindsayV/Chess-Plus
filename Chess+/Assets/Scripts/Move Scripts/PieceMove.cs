using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**PieceMove stores within it one move a chess piece can do. */
public class PieceMove : Move
{
    protected Coordinate from; //The coordinate of the chess piece being moved
    protected Coordinate to; //The Coordinate of the spot its being moved to

    private Coordinate piecePos; //the coordinate the piece actually is

    /**Constructor function */
    public PieceMove(Coordinate from, Coordinate to)
    {
        this.from = from;
        this.to = to;
        this.piecePos = from;
    }

    /**gets end coordinate */
    public Coordinate getTo() { return this.to; }

    /**gets start coordinate */
    public Coordinate getFrom() { return from; }

    /** Gets moved piece */
    public Piece getPiece(BoardState bState) { return bState.getPiece(piecePos); }

    /**If a move is to be used again (after doMoveState has been called), this resets
     the move so it may be used again
     Intended for use in things like removeCheckingMoves that simply checks the results of moves
     but does not keep their permanent consequences */
    public override void resetMove()
    {
        this.piecePos = from;
    }

    /**Updates the boardState as if the move had happened. Does not update the visuals/gameObjects 
        Moves the Pieces in boardArray. Is used as part of doMove, and also used to test/check moves (for check and whatnot)
        Returns any killed piece
        Overridden by subclasses of move with special additional implementations
        */
    public override Piece doMoveState(BoardState bState)
    {
        piecePos = from;
        Piece killedPiece = Move.movePieceState(bState, getPiece(bState).getTeam(), getPiece(bState), this.getTo());
        piecePos = to;
        return killedPiece;
    }

    /**Does the parts of a move that the user can see.
    Updates the gameobjects (creates, destroys, moves) so the user can see the changes to the chess game*/
    public override void showMove(BoardState bState, Piece killedPiece)
    {
        if (killedPiece != null)
        {
            killedPiece.destroy();
        }
        Piece movedPiece = this.getPiece(bState);
        movedPiece.moveGameObjTo(this.getTo());
    }

    public override bool inDanger(Coordinate pos)
    {
        return this.getTo() == pos;
    }

    public override Coordinate moveTilePos()
    {
        return this.getTo();
    }
}
