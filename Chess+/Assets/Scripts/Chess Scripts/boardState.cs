using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boardState : MonoBehaviour
{
    string[,] boardArray;
    bool playerTurn;
   bool bLCastle; //Stores whether you can castle in this direction (eg neither king nor rook has moved)
     bool bRCastle; //It's black left, black right, white left and white right
     bool wLCastle;
     bool wRCastle;
    Vector2 enPassant;

    boardState(string[,] boardArray, bool playerTurn, bool bLCastle, bool bRCastle, bool wLCastle, bool wRCastle, Vector2 enPassant)
    {
        this.boardArray = boardArray;
        this.playerTurn = playerTurn;
        this.bLCastle = bLCastle;
        this.bRCastle = bRCastle;
        this.wLCastle = wLCastle;
        this.wRCastle = wRCastle;
        this.enPassant = enPassant;
    }

}
