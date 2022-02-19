using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public GameObject movedPiece; //The gameObject chess piece that is being moved
    public Vector2 to; //The colrow of the spot its being moved to

    //Constructor function
    public Move(GameObject movedPiece, Vector2 to)
    {
        this.movedPiece = movedPiece;
        this.to = to;
    }

}
