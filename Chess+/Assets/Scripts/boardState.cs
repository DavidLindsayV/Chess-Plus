using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Stores the state of a board
 */
public class boardState : MonoBehaviour
{
    private Piece[,] boardArray;
    //Stores the Piece objects
    //Stores the Piece chess pieces. Goes from 0-7 for col and row. 
    //Note, it is stored [col, row].
    //Note, 0 on this is col 1. 7 on this is col 8. This is because arrays start from index 0. So to convert from col, row to this array, use col -1, row - 1
    private Team currentPlayer;
    private bool bLCastle; //Stores whether you can castle in this direction (eg neither king nor rook has moved)
    private bool bRCastle; //It's black left, black right, white left and white right
    private bool wLCastle;
    private bool wRCastle;
    private Coordinate enPassant; //can be null

    //Optional fields, used for efficiency
    private King whiteKing;
    private King blackKing;

    /** Constructor taking in all essential fields */
    public boardState(Piece[,] boardArray, Team currentPlayer, bool bLCastle, bool bRCastle, bool wLCastle, bool wRCastle, Coordinate enPassant)
    {
        this.boardArray = boardArray;
        this.currentPlayer = currentPlayer;
        this.bLCastle = bLCastle;
        this.bRCastle = bRCastle;
        this.wLCastle = wLCastle;
        this.wRCastle = wRCastle;
        this.enPassant = enPassant;
    }

    /**Loads in the board state from a FEN string */
    public boardState(string FENstring)
    {
        string[] FENwords = FENstring.Split(' ');
        //Setting the board
        int col = 1;
        int row = boardScript.boardSize;
        for (int i = 0; i < FENwords[0].Length; i++)
        {
            char c = FENwords[0][i];
            if (c.Equals('/'))
            {
                row--;
                col = 1;
            }
            else if (char.IsDigit(c))
            {
                col = col + c;
            }
            else if (c.Equals(' '))
            {
                break;
            }
            else
            {
                Piece piece;
                switch (char.ToLower(c))
                {
                    case 'p': piece = new Pawn(c, new Coordinate(col, row));
                        break;
                    case 'r': piece = new Rook(c, new Coordinate(col, row));
                        break;
                    case 'n': piece = new Knight(c, new Coordinate(col, row));
                        break;
                    case 'b': piece = new Bishop(c, new Coordinate(col, row));
                        break;
                    case 'q': piece = new Queen(c, new Coordinate(col, row));
                        break;
                    case 'k': piece = new King(c, new Coordinate(col, row));
                        //store white and black kings
                        if(piece.getTeam() == Team.White) { whiteKing = (King)piece; } else if(piece.getTeam() == Team.Black){ blackKing = (King)piece; }
                        break;
                    default: throw new System.Exception("boardState had an oopsie in a switch statement");
                }
                boardArray[col - 1, row - 1] = piece;
                col++;
            }
        }

        //The player turn
        if (FENwords[1].Equals("w")) { this.currentPlayer = Team.White; } else { this.currentPlayer = Team.Black; }

        //Castling
        if (!FENwords[2].Contains("K")) { wRCastle = true; }
        if (!FENwords[2].Contains("Q")) { wLCastle = true; }
        if (!FENwords[2].Contains("k")) { bLCastle = true; }
        if (!FENwords[2].Contains("q")) { bRCastle = true; }

        //En Passant square
        if (!FENwords[3].Contains("-"))
        {
            string positionString = char.ToString(FENwords[3][0]) + char.ToString(FENwords[3][1]);
            enPassant = new Coordinate(positionString);
        }

        //Implement halfmoves??? (50 moves without pawn progression or killing = draw)

        //Implement fullmoves??? Stores how many turns have elapsed
    }

    /** Returns the board state as a FEN string */
    public string toFen()
    {
        string fenString = "";
        //Storing board setup
        for (int row = boardScript.boardSize; row >= 1; row--)
        {
            int emptySpaces = 0;
            for (int col = 1; col <= boardScript.boardSize; col++)
            {
                if (boardArray[col - 1, row - 1] != null)
                {
                    Piece currentPiece = boardArray[col - 1, row - 1];
                    if (emptySpaces != 0) { fenString += emptySpaces; }
                    fenString += currentPiece.toString();
                    emptySpaces = 0;
                }
                else
                {
                    emptySpaces++;
                }
            }
            if (emptySpaces != 0) { fenString += emptySpaces; }
            if (row != 1) { fenString += char.ToString('/'); }
        }

        //The player turn
        if (this.currentPlayer == Team.White) { fenString += " w "; } else { fenString += " b "; }

        //Castling
        if (wRCastle) { fenString += "K"; }
        if (wLCastle) { fenString += "Q"; }
        if (bLCastle) { fenString += "k"; }
        if (bRCastle) { fenString += "q"; }
        fenString += ' ';

        //En Passant square
        if (this.enPassant == null) { fenString += "- "; }
        else
        {
            fenString += enPassant.toString() + " ";
        }

        //Implement halfmoves??? (50 moves without pawn progression or killing = draw)

        //Implement fullmoves??? Stores how many turns have elapsed

        return fenString;
    }

    //Returns the chess piece for a certain col and row. 
    private Piece getPiece(int col, int row)
    {
        return boardArray[col - 1, row - 1];
    }

    public void setPiece(Coordinate pos, Piece newPiece)
    {
        Piece oldPiece = boardArray[pos.getCol() - 1, pos.getRow() - 1];
        if(oldPiece != null) { oldPiece.destroy(); }
        boardArray[pos.getCol() - 1, pos.getRow() - 1] = newPiece;
    }
}
