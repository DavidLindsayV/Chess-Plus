using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Prefabs 
{
    //The public piece prefabs
    public static GameObject pawnPrefab;
    public static GameObject rookPrefab;
    public static GameObject bishopPrefab;
    public static GameObject knightPrefab;
    public static GameObject queenPrefab;
    public static GameObject kingPrefab;
    public static GameObject tilePrefab;

    //The public materials for each team
    public static Material white;
    public static Material black;
    public static Material highLight;

    public static GameObject getPrefab(Piece.pieceType type)
    {
        switch (type)
        {
            case Piece.pieceType.Pawn:
                return pawnPrefab;
            case Piece.pieceType.Rook:
                return rookPrefab;
            case Piece.pieceType.Bishop:
                return bishopPrefab;
            case Piece.pieceType.Knight:
                return knightPrefab;
            case Piece.pieceType.Queen:
                return queenPrefab;
            case Piece.pieceType.King:
                return kingPrefab;
            default:
                throw new System.Exception("Invalid impossible pieceType???");
        }
    }
}
