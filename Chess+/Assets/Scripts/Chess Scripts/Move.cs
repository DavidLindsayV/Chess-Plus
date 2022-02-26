using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Move stores within it one move a chess piece can do. It combines the col + row of where the gameObject is moved and the gameObject that is being moved.
public class Move
{
    //SHOULD I ADD IN A BOOL TO STORE THE TEAM?
    public Vector2 from; //Stores where the piece came from
    public GameObject movedPiece; //The gameObject chess piece that is being moved
    public Vector2 to; //The colrow of the spot its being moved to

    public bool castling = false; //Variables for Castling
    public Move castlingMove;

    public bool promotion = false;

    //Constructor function
    public Move(GameObject movedPiece, Vector2 from, Vector2 to)
    {
        this.movedPiece = movedPiece;
        this.from = from;
        this.to = to;
    }

    //If a move is Castling, you need to store how the king moves AND how the rook moves. This stores how the rook moves
    public void setCastling(Move castlingMove)
    {
        this.castling = true;
        this.castlingMove = castlingMove;
    }

    public void setPromotion()
    {
        promotion = true;
    }

}
