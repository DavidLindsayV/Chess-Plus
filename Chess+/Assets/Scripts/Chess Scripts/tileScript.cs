using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The tileScript is a script attached to each moveTile. It basically stores which Move is associated with which tile.
public class tileScript : MonoBehaviour
{
    //Stores the Move the tile is associated with
    private Move move;

    //Sets the Move
    public void setMove(Move move)
    {
        this.move = move;
    }

    //Returns the Move
    public Move getMove()
    {
        return move;
    }
}
