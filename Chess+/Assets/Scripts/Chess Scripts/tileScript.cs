using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tileScript : MonoBehaviour
{
    private Move move;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setMove(Move move)
    {
        this.move = move;
    }

    public Move getMove()
    {
        return move;
    }
}
