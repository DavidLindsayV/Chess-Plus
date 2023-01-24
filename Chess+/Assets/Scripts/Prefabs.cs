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
