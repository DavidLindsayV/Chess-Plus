using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * A class to represent a chess piece
 */
public abstract class Piece : MonoBehaviour
{
    //An enum for the names of pieces
    public enum pieceType { Pawn, Rook, Knight, Bishop, Queen, King};

    //Fields for a generic piece
    private Team team;
    private Coordinate position;
    private pieceType type;
    private GameObject gameObj;

    /** Constructs a Piece object */
    public Piece(Team team, Coordinate pos, pieceType type)
    {
        this.team = team;
        this.position = pos;
        this.type = type;
    }

    /**Constructs a piece from it's FEN string character and position */
    public Piece(char FENchar, Coordinate pos)
    {
        this.type = charToType(FENchar);
        if (char.IsUpper(FENchar)) { this.team = Team.White; } else { this.team = Team.Black; }
        this.position = pos;
    }

    /** Get the team */
    public Team getTeam() { return this.team;  }

    /** To FEN string */
    public string toString() {
        char c = typeToChar(this.type);
        if (this.team == Team.White) { c =  char.ToUpper(c); }
        return char.ToString(c);
     }

    /**Converts a pieceType to FEN char (does not take into account Team) */
    public static char typeToChar(pieceType type)
    {
        char c = '\0';
        switch (type)
        {
            case pieceType.Pawn:
                c = 'p';
                break;
            case pieceType.Rook:
                c = 'r';
                break;
            case pieceType.Knight:
                c = 'n';
                break;
            case pieceType.Bishop:
                c = 'b';
                break;
            case pieceType.Queen:
                c = 'q';
                break;
            case pieceType.King:
                c = 'k';
                break;
            default:
                throw new System.Exception("Invalid fen char");
        }
        return c;
    }

    /**Converts a FEN char into a pieceType. Ignores Team */
    public static pieceType charToType(char FENchar)
    {
        switch (char.ToLower(FENchar))
        {
            case 'p':
                return pieceType.Pawn;
            case 'r':
                return pieceType.Rook;
            case 'n':
                return pieceType.Knight;
            case 'b':
                return pieceType.Bishop;
            case 'q':
                return pieceType.Queen;
            case 'k':
                return pieceType.King;
            default: throw new System.Exception("invalid FEN char");
        }
    }

    public abstract bool isValidMove(boardState state, Move move);
    public abstract Move[] getValidMoves(boardState state);
}
