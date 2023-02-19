using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**A gamestate where the user is promoting pieces
This is attached to the Canvas */
public class PromoteMenu : GameState
{
    private GameState previousState;
    private BoardState bState;
    private PromoteMove promotionMove;

    private GameObject PromotionPanel;

    void Start()
    {
        PromotionPanel = this.gameObject.transform.Find("PromotionPanel").gameObject;
    }

    /** Pause game for the selection */
    public void GetPromotedTo(PromoteMove move, BoardState bState)
    {
        this.promotionMove = move;
        this.bState = bState;
        previousState = StateManager.OpenState(this);
    }

    /**Promotes to a piece of the given type */
    public void promotePiece(Piece piece)
    {
        this.promotionMove.promotedTo = piece;
        StateManager.OpenState(previousState);
    }

    /**Promotes to a queen */
    public void promoteQueen()
    {
        promotePiece(new Queen(this.promotionMove.getPiece(bState).getTeam(), this.promotionMove.getTo(), null));
    }
    /** Promotes to a bishop */
    public void promoteBishop()
    {
        promotePiece(new Bishop(this.promotionMove.getPiece(bState).getTeam(), this.promotionMove.getTo(), null));
    }
    /** Promotes to a rook */
    public void promoteRook()
    {
        promotePiece(new Rook(this.promotionMove.getPiece(bState).getTeam(), this.promotionMove.getTo(), null));
    }
    /**Promotes to a knight */
    public void promoteKnight()
    {
        promotePiece(new Knight(this.promotionMove.getPiece(bState).getTeam(), this.promotionMove.getTo(), null));
    }

    public override void runState(){
        this.enabled = false;
        Time.timeScale = 0f;
        PromotionPanel.SetActive(true);
    }

    public override void closeState(){
        PromotionPanel.SetActive(false);
    }
}
