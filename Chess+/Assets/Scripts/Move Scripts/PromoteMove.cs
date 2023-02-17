using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromoteMove : Move
{
    private static GameState promoteMenu;

    //promotedTo should be a Piece (rook/queen/knight/bishop) that does NOT have a gameObject
    public Piece promotedTo;

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

    public override Piece doMoveState(boardState bState)
    {
        Piece killedPiece = base.doMoveState(bState);
        bState.setPiece(this.getTo(), this.promotedTo);
        return killedPiece;
    }

    /**promoted a piece, and it uses a menu to see what the piece is promoted to if it's the players turn
*/
    public override void showMove(boardState bState, Piece killedPiece)
    {
        if (bState.currentTeam() == bState.playersTeam())
        {
            ((PromoteMenu)(StateManager.promoteMenu)).GetPromotedTo(this, bState);
        }
        getPiece(bState).destroy();
        this.makePromotedPiece(); //Allow the new piece replacing the pawn to appear
        base.showMove(bState, killedPiece);
    }
}
