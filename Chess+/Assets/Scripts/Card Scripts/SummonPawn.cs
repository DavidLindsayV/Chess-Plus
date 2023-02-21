using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**Summon a pawn */
public class SummonPawn: Card
{
    public SummonPawn():base(){
        this.CardText = "SUMMON PAWN\nCreate a pawn of your team on any empty space";
        makeCard();
    }

    public override List<CardMove> getPieceSpecificMoves(BoardState bState, Team team, Piece piece){
        return new List<CardMove>();
    }
    public override List<CardMove> getGeneralMoves(BoardState bState, Team team){
        List<CardMove> moves = new List<CardMove>();
        for(int row = 1; row <= bState.boardSize; row++){
            for(int col = 1; col <= bState.boardSize; col++){
                if(bState.getPiece(col,row) == null){
                    moves.Add(new SummonPawnMove(this, new Coordinate(col,row), team));
                }
            }
        }
        return moves;
    }
}

/**The move for SummonPawn */
public class SummonPawnMove: CardMove
{
    Coordinate pos;
    Team team;
    public SummonPawnMove(Card card, Coordinate pos, Team team): base(card){
        this.pos = pos;
        this.team = team;
    }

/**Updates the boardState as if the move had happened. Does not update the visuals/gameObjects 
    */
    public override Piece doMoveState(BoardState bState){ 
        Pawn pawn = new Pawn(team, pos, null);
        bState.setPiece(pos, pawn);
        return null;
    }

    /**Does the parts of a move that the user can see.*/
    public override void showMove(BoardState bState, Piece killedPiece){
        bState.getPiece(pos).makePiece(); //make the gameObj for the pawn you just created
    }


/**Returns whether this position is under threat from this move or not */
    public override bool inDanger(Coordinate pos){return false;}

    /**Returns the Coordinate where the moveTile for this move should be created */
    public override Coordinate moveTilePos(){return pos;}
}
