using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**Move stores within it one move a chess piece can do. */
public class Move
{
    protected Piece movedPiece; //The Piece chess piece that is being moved 
    //TODO remove movedPiece as using it can cause errors. You should look at the piece from your current board
    protected Coordinate to; //The Coordinate of the spot its being moved to

    /**Constructor function */
    public Move(Piece movedPiece, Coordinate to)
    {
        this.movedPiece = movedPiece;
        this.to = to;
    }

    /**gets end coordinate */
    public Coordinate getTo() { return this.to;  }

    /**gets start coordinate */
    public Coordinate getFrom(){ return movedPiece.getPos(); }

    /** Gets moved piece */
    public Piece getPiece() { return this.movedPiece; }

/**Updates the boardState as if the move had happened. Does not update the visuals/gameObjects 
    Moves the GameObjects in boardArray. Is used as part of doMove, and also used to test/check moves (for check and whatnot)
    Returns any killed piece
    Overridden by subclasses of move with special additional implementations
    */
    public virtual Piece doMoveState(boardState bState)
    {
        Piece piece = bState.getPiece(this.getFrom()); //You need to refer to the piece from the cloned board
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
                else if (this.getTo().getCol() == 8)
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
            }else if(this.getFrom().getCol() == 8){
                bState.setCastle(team, false, false);
            }
        }
        bState.setPiece(this.getTo(), piece);
        piece.setPos(this.getTo());
        bState.setEnPassant(null);
        return killedPiece;
    }
}
