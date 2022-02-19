using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public GameObject movedPiece;
    public Vector2 to; //The colrow of the spot its being moved to
    //public GameObject moveTile;

    //Constructor function
    public Move(GameObject movedPiece, Vector2 to)
    {
        this.movedPiece = movedPiece;
        this.to = to;
    }

    //public void setMoveTile(GameObject moveTile)
    //{
    //    this.moveTile = moveTile;
    //}

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
