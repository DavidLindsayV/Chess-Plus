using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**The bare essentials of a Move, that is flexible enough for both CardMove and PieceMove */
public abstract class Move
{

    /**Updates the castling variables in the bState, based on the moved and killed pieces
The variables are a boardState, the piece that has moved, where it has moved from, and the piece that has been killed
Either movedPiece or killedPiece can be null */
    public static void updateCastling(BoardState bState, Piece movedPiece, Coordinate from, Piece killedPiece)
    {
        if (killedPiece != null && killedPiece is Rook)
        {
            if (killedPiece.getPos().getCol() == 1)
            {
                bState.setCastle(killedPiece.getTeam(), true, false);
            }
            else if (killedPiece.getPos().getCol() == bState.boardSize)
            {
                bState.setCastle(killedPiece.getTeam(), false, false);
            }
        }
        if (movedPiece != null)
        {
            //Update castling variables if a king/rook moved
            if (movedPiece is King)
            {
                bState.setCastle(movedPiece.getTeam(), false, false);
                bState.setCastle(movedPiece.getTeam(), true, false);
            }
            if (movedPiece is Rook)
            {
                if (from.getCol() == 1)
                {
                    bState.setCastle(movedPiece.getTeam(), true, false);
                }
                else if (from.getCol() == bState.boardSize)
                {
                    bState.setCastle(movedPiece.getTeam(), false, false);
                }
            }
        }
    }

/**A function to update boardState, to move a Piece "movePiece" to a coordinate "to", 
returning any killed pieces
This implements the base functionality of doMoveState for PieceMove, and may be used in some CardMoves */
    protected static Piece movePieceState(BoardState bState, Team team, Piece movePiece, Coordinate to){
        bState.setPiece(movePiece.getPos(), null);
        Piece killedPiece = null;
        if (bState.getPiece(to) != null)
        {
            killedPiece = bState.getPiece(to); 
        }
        updateCastling(bState, movePiece, movePiece.getPos(), killedPiece);
        bState.setPiece(to, movePiece);
        movePiece.setPos(to);
        bState.setEnPassant(null);
        return killedPiece;
    }

    /**If a move is to be used again (after doMoveState has been called), this resets
     the move so it may be used again
     Intended for use in things like removeCheckingMoves that simply checks the results of moves
     but does not keep their permanent consequences */
    public virtual void resetMove() { }

    /**Updates the boardState as if the move had happened. Does not update the visuals/gameObjects 
        */
    public abstract Piece doMoveState(BoardState bState);

    /**Does the parts of a move that the user can see.*/
    public abstract void showMove(BoardState bState, Piece killedPiece);

    /**If the User is going to do this move, this does any actions needed by the move
before the Board is modified and doMoveState/showMove happen */
    public virtual void prepareMove(BoardState bState) { }

    /**Returns whether this position is under threat from this move or not */
    public abstract bool inDanger(Coordinate pos);

    /**Returns the Coordinate where the moveTile for this move should be created */
    public abstract Coordinate moveTilePos();
}
