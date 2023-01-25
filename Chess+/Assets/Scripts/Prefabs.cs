using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Prefabs
{
    //The public piece prefabs
    public static readonly GameObject pawnPrefab;
    public static readonly GameObject rookPrefab;
    public static readonly GameObject bishopPrefab;
    public static readonly GameObject knightPrefab;
    public static readonly GameObject queenPrefab;
    public static readonly GameObject kingPrefab;
    public static readonly GameObject tilePrefab;

    //The public materials for each team
    public static readonly Material white;
    public static readonly Material black;
    public static readonly Material highLight;

    public static GameObject getPrefab(Piece type)
    {
        if (type is Pawn)
        {
            return pawnPrefab;
        }
        if (type is Rook)
        {
            return rookPrefab;
        }
        if (type is Knight)
        {
            return knightPrefab;
        }
        if (type is Bishop)
        {
            return bishopPrefab;
        }
        if (type is Queen)
        {
            return queenPrefab;
        }
        if (type is King)
        {
            return kingPrefab;
        }
        throw new System.Exception("Invalid impossible pieceType???");
    }
}
