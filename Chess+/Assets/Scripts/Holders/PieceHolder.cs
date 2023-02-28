using UnityEngine;

/**The PieceHolder allows a gameobject to reference the Piece object that created it */
public class PieceHolder : MonoBehaviour
{
    //Stores the Piece the gameobject is associated with
    private Piece piece;

    /**Sets the Piece */
    public void setPiece(Piece piece)
    {
        this.piece = piece;
    }

    /** Returns the Piece */
    public Piece getPiece()
    {
        return this.piece;
    }
}
