using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromoteMove : Move
{
    private static GameState promoteMenu;

    //promotedTo should be a Piece (rook/queen/knight/bishop) that does NOT have a gameObject
    public Piece promotedTo;
    private Piece promotedFrom; //this is used to store the Pawn that was promoted, so it can be destroyed

    public PromoteMove(Coordinate from, Coordinate to, Piece promotedTo)
        : base(from, to)
    {
        this.promotedTo = promotedTo;
    }

    //Makes the promoted piece visible
    public void makePromotedPiece()
    {
        this.promotedTo.makePiece();
    }

    public override Piece doMoveState(BoardState bState)
    {
        Piece killedPiece = base.doMoveState(bState);
        promotedFrom = bState.getPiece(this.getTo());
        bState.setPiece(this.getTo(), this.promotedTo);
        return killedPiece;
    }

    /**promoted a piece, and it uses a menu to see what the piece is promoted to if it's the players turn
*/
    public override void showMove(BoardState bState, Piece killedPiece)
    {
        promotedFrom.destroy();
        this.makePromotedPiece(); //Allow the new piece replacing the pawn to make a gameobject
        base.showMove(bState, killedPiece);
    }

    public override void prepareMove(BoardState bState)
    {
        base.prepareMove(bState);
        //prepareMove only happens on the players turn, so the promotion menu only appears for the player
        ((PromoteMenu)(StateManager.promoteMenu)).GetPromotedTo(this, bState);
    }
}
