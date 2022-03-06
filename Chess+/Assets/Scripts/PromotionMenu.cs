using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromotionMenu : MonoBehaviour
{
    public GameObject PromotionPanel;
    public GameObject board;
    private boardScript boardScriptReference; //Used to stop boardScript from running whilst paused
    private Move promotionMove;
    // Start is called before the first frame update
    void Start()
    {
        boardScriptReference = board.GetComponent<boardScript>();
    }

    //Resumes the game
    private void Resume()
    {
        Time.timeScale = 1f;
        PromotionPanel.SetActive(false);
        boardScriptReference.enabled = true;
    }

    //Pauses the game
    public void Run(Move move)
    {
        this.promotionMove = move;
        Time.timeScale = 0f;
        PromotionPanel.SetActive(true);
        boardScriptReference.enabled = false;
    }

    public void promoteQueen()
    {
        boardScriptReference.makePiece(boardScriptReference.queen, (int)promotionMove.to.x, (int)promotionMove.to.y, promotionMove.movedPiece.name.Contains("white"), true);
        boardScriptReference.pieceHasMoved[(int)promotionMove.to.x - 1, (int)promotionMove.to.y - 1] = true;
        Destroy(promotionMove.movedPiece);
        Resume();
    }
    public void promoteBishop()
    {
        boardScriptReference.makePiece(boardScriptReference.bishop, (int)promotionMove.to.x, (int)promotionMove.to.y, promotionMove.movedPiece.name.Contains("white"), true);
        boardScriptReference.pieceHasMoved[(int)promotionMove.to.x - 1, (int)promotionMove.to.y - 1] = true;
        Destroy(promotionMove.movedPiece);
        Resume();
    }
    public void promoteRook()
    {
        boardScriptReference.makePiece(boardScriptReference.rook, (int)promotionMove.to.x, (int)promotionMove.to.y, promotionMove.movedPiece.name.Contains("white"), true);
        boardScriptReference.pieceHasMoved[(int)promotionMove.to.x - 1, (int)promotionMove.to.y - 1] = true;
        Destroy(promotionMove.movedPiece);
        Resume();
    }
    public void promoteKnight()
    {
        boardScriptReference.makePiece(boardScriptReference.knight, (int)promotionMove.to.x, (int)promotionMove.to.y, promotionMove.movedPiece.name.Contains("white"), true);
        boardScriptReference.pieceHasMoved[(int)promotionMove.to.x - 1, (int)promotionMove.to.y - 1] = true;
        Destroy(promotionMove.movedPiece);
        Resume();
    }
}
