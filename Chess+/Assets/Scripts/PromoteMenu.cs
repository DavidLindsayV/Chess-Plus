using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromoteMenu : MonoBehaviour
{
    public GameObject PromotionPanel;
    public GameObject board;
    private boardScript boardScriptReference; //Used to stop boardScript from running whilst paused
    private PromoteMove promotionMove;

    void Start()
    {
        boardScriptReference = board.GetComponent<boardScript>();
    }

    /** Resumes the game for the menu selection */
    private void Resume()
    {
        Time.timeScale = 1f;
        PromotionPanel.SetActive(false);
        boardScriptReference.enabled = true;
    }

    /** Pause game for the selection */
    public void Run(PromoteMove move)
    {
        this.promotionMove = move;
        Time.timeScale = 0f;
        PromotionPanel.SetActive(true);
        boardScriptReference.enabled = false;
    }

    /**Promotes to a piece of the given type */
    public void promotePiece(Piece piece)
    {
        //TODO: update promotePiece and Run and Resume once you've updated boardScript/active screen managers
        //Currently user promotion is not working
        //Still need to update: PromoteMenu, gameStateManager, PauseMenu, and stuff in boardScript
        boardScriptReference.state.setPiece(promotionMove.getTo(), piece);
        Resume();
    }

    /**Promotes to a queen */
    public void promoteQueen()
    {
        //promotePiece(new Queen(promotionMove.getTeam(), promotionMove.getTo()));
    }
    /** Promotes to a bishop */
    public void promoteBishop()
    {
        //promotePiece(new Bishop(promotionMove.getPiece().getTeam(), promotionMove.getTo()));
    }
    /** Promotes to a rook */
    public void promoteRook()
    {
        //promotePiece(new Rook(promotionMove.getPiece().getTeam(), promotionMove.getTo()));
    }
    /**Promotes to a knight */
    public void promoteKnight()
    {
        //promotePiece(new Knight(promotionMove.getPiece().getTeam(), promotionMove.getTo()));
    }
}
