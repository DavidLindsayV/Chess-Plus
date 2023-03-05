using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**"ROOKIFY
Turn one of your non-rook non-king pieces into a Rook" */
public class Rookify : Card
{
    public Rookify(Team team) : base(team)
    {
        this.cardSprite = Prefabs.rookifyPrefab;
        makeCard();
    }

    public Rookify(Team team, GameObject g) : base(team)
    {
        this.cardSprite = Prefabs.rookifyPrefab;
        this.cardObj = g;
    }

    public override List<CardMove> getCoordSpecificMoves(BoardState bState, Coordinate coor)
    {
        if (canPlayOnPos(bState,coor))
        {
            return new List<CardMove> { new RookifyMove(this, bState.getPiece(coor)) };
        }
        return new List<CardMove>();
    }
    public override List<CardMove> getGeneralMoves(BoardState bState)
    {
        return new List<CardMove>();
    }

    public override bool canPlayOnPos(BoardState bstate, Coordinate coor)
    {
        Piece p = bstate.getPiece(coor);
        return p != null && p.getTeam() == this.getTeam() && !(p is King) && !(p is Rook);
    }

    public override int cardNum()
    {
        return 1;
    }

    public override Card clone()
    {
        Rookify r = new Rookify(this.getTeam(), null);
        return r;
    }
}

/**The move for Rookify */
public class RookifyMove : CardMove
{
    Piece piece; //piece to turn into a rook
    Rook r; //the rook you make
    public RookifyMove(Card card, Piece piece) : base(card)
    {
        this.piece = piece;
    }

    /**Updates the boardState as if the move had happened. Does not update the visuals/gameObjects 
        */
    public override Piece doMoveState(BoardState bState)
    {
        playCardState(bState);
        bState.setEnPassant(null);
        this.r = new Rook(piece.getTeam(), piece.getPos(), null);
        bState.setPiece(r.getPos(), r);
        return piece;
    }

    /**Does the parts of a move that the user can see.*/
    public override void showMove(BoardState bState, Piece killedPiece)
    {
        playCardShow();
        killedPiece.destroy(); //destroy the Piece you just turned into a Rook
        r.makePiece(); //make the gameObj for the new Rook piece
    }


    /**Returns whether this position is under threat from this move or not */
    public override bool inDanger(Coordinate pos) { return false; }

    /**Returns the Coordinate where the moveTile for this move should be created */
    public override Coordinate moveTilePos() { return piece.getPos(); }

}
