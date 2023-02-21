using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**The bare essentials of a Move, that is flexible enough for both CardMove and PieceMove */
public abstract class Move
{

/**If a move is to be used again (after doMoveState has been called), this resets
 the move so it may be used again
 Intended for use in things like removeCheckingMoves that simply checks the results of moves
 but does not keep their permanent consequences */
    public virtual void resetMove(){}

/**Updates the boardState as if the move had happened. Does not update the visuals/gameObjects 
    */
    public abstract Piece doMoveState(BoardState bState);

    /**Does the parts of a move that the user can see.*/
    public abstract void showMove(BoardState bState, Piece killedPiece);

/**If the User is going to do this move, this does any actions needed by the move
before the Board is modified and doMoveState/showMove happen */
    public virtual void prepareMove(BoardState bState){}

/**Returns whether this position is under threat from this move or not */
    public abstract bool inDanger(Coordinate pos);

    /**Returns the Coordinate where the moveTile for this move should be created */
    public abstract Coordinate moveTilePos();
}
