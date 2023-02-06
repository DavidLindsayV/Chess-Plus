using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**Move stores within it one move a chess piece can do. */
public class Move
{
    protected Coordinate from; //The coordinate of the chess piece being moved
    protected Coordinate to; //The Coordinate of the spot its being moved to

    private Coordinate piecePos; //the coordinate the piece actually is

    /**Constructor function */
    public Move(Coordinate from, Coordinate to)
    {
        this.from = from;
        this.to = to;
        this.piecePos = from;
    }

    /**gets end coordinate */
    public Coordinate getTo() { return this.to;  }

    /**gets start coordinate */
    public Coordinate getFrom(){ return from; }

    /** Gets moved piece */
    public Piece getPiece(boardState bState) { return bState.getPiece(piecePos); }

/**If a move is to be used again (after doMoveState has been called), this resets
 the move so it may be used again
 Intended for use in things like removeCheckingMoves that simply checks the results of moves
 but does not keep their permanent consequences */
    public void resetMove(){
        this.piecePos = from;
    }

/**Updates the boardState as if the move had happened. Does not update the visuals/gameObjects 
    Moves the Pieces in boardArray. Is used as part of doMove, and also used to test/check moves (for check and whatnot)
    Returns any killed piece
    Overridden by subclasses of move with special additional implementations
    */
    public virtual Piece doMoveState(boardState bState)
    {
        piecePos = from;
        Piece piece = getPiece(bState); //You need to refer to the piece from the cloned board
        //not from the move, because the one in the move may be from a different board state
        //(piece may have the same values as move.getPiece(), but only piece can be changed without consequence)
        Team team = piece.getTeam();
        bState.setPiece(this.getFrom(), null);
        Piece killedPiece = null;
        if (bState.getPiece(this.getTo()) != null)
        {
            killedPiece = bState.getPiece(this.getTo()); 
            //Stores the piece that could be killed when the theoretical move is done
            //Update castling variables
            if (killedPiece is Rook)
            {
                if (this.getTo().getCol() == 1)
                {
                    bState.setCastle(killedPiece.getTeam(), true, false);
                }
                else if (this.getTo().getCol() == bState.boardSize)
                {
                    bState.setCastle(killedPiece.getTeam(), false, false);
                }
            }
        }
        //Update castling variables if a king/rook moved
        if (piece is King){
            bState.setCastle(team, false, false);
            bState.setCastle(team, true, false);
        }
        if (piece is Rook){
            if (this.getFrom().getCol() == 1){
                bState.setCastle(team, true, false);
            }else if(this.getFrom().getCol() == bState.boardSize){
                bState.setCastle(team, false, false);
            }
        }
        bState.setPiece(this.getTo(), piece);
        piece.setPos(this.getTo());
        bState.setEnPassant(null);
        piecePos = to;
        return killedPiece;
    }

    /**Does the parts of a move that the user can see.
    Updates the gameobjects (creates, destroys, moves) so the user can see the changes to the chess game*/
    public virtual void showMove(boardState bState, Piece killedPiece)
    {
        if (killedPiece != null)
        {
            killedPiece.destroy();
        }
        Piece movedPiece = this.getPiece(bState);
        movedPiece.getObject().transform.position = new Vector3(
            this.getTo().getX(),
            movedPiece.getObject().transform.position.y,
            this.getTo().getZ()
        );
    }
}
