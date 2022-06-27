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
    public void Run(Move move)
    {
        this.promotionMove = move;
        Time.timeScale = 0f;
        PromotionPanel.SetActive(true);
        boardScriptReference.enabled = false;
    }

    /**Promotes to a piece of the given type */
    public void promotePiece(Piece.pieceType type)
    {
        //TODO: update promotePiece and Run and Resume once you've updated boardScript/active screen managers
        boardScriptReference.makePiece(type, promotionMove.getTo().getCol(), promotionMove.getTeam());
        Destroy(promotionMove.movedPiece);
        Resume();
    }

    /**Promotes to a queen */
    public void promoteQueen()
    {
        promotePiece(Piece.pieceType.Queen);
    }
    /** Promotes to a bishop */
    public void promoteBishop()
    {
        promotePiece(Piece.pieceType.Bishop);
    }
    /** Promotes to a rook */
    public void promoteRook()
    {
        promotePiece(Piece.pieceType.Rook);
    }
    /**Promotes to a knight */
    public void promoteKnight()
    {
        promotePiece(Piece.pieceType.Knight);
    }
}
