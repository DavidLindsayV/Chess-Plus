using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**Turn a Piece into a Rook */
public class Rookify: Card
{
    public Rookify():base(){
        this.CardText = "ROOKIFY\nTurn one of your non-rook pieces into a Rook";
        makeCard();
    }

    public override List<CardMove> getPieceSpecificMoves(BoardState bState, Team team, Piece piece){
        if(piece.getTeam() == team && !(piece is Rook)){
            return new List<CardMove>{new RookifyMove(this, piece) };
        }
        return new List<CardMove>();
    }
    public override List<CardMove> getGeneralMoves(BoardState bState, Team team){
        return new List<CardMove>();
    }
}

/**The move for Rookify */
public class RookifyMove: CardMove
{
    Piece piece; //piece to turn into a rook
    Rook r; //the rook you make
    public RookifyMove(Card card, Piece piece): base(card){
        this.piece = piece;
    }

/**Updates the boardState as if the move had happened. Does not update the visuals/gameObjects 
    */
    public override Piece doMoveState(BoardState bState){ 
        this.r = new Rook(piece.getTeam(), piece.getPos(), null);
        bState.setPiece(r.getPos(), r);
        return piece;
    }

    /**Does the parts of a move that the user can see.*/
    public override void showMove(BoardState bState, Piece killedPiece){
        killedPiece.destroy(); //destroy the Piece you just turned into a Rook
        r.makePiece(); //make the gameObj for the new Rook piece
    }


/**Returns whether this position is under threat from this move or not */
    public override bool inDanger(Coordinate pos){return false;}

    /**Returns the Coordinate where the moveTile for this move should be created */
    public override Coordinate moveTilePos(){return piece.getPos();}
}
