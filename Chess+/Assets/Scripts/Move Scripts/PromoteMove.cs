using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromoteMove : Move
{
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

    public override void showMove(boardState bState, Piece killedPiece)
    {
        base.showMove(bState, killedPiece);
        if (bState.currentTeam() == bState.playersTeam())
        {
            //promotionMenuReference.Run(this); 
            //TODO figure out what to do here
            //Make the promotion menu stuff happen when a tile is selected under Game?
            //Or have it trigger when the move is enacted in showMove?
            //probably under showMove is safe - it only triggers when its ready to be shown to player
            //but need to fix the promotion menu and stuff
        }
        else //Promotion for enemy AI
        {
            getPiece(bState).destroy();
            this.makePromotedPiece(); //Allow the new piece replacing the pawn to appear
        }
    }
}
