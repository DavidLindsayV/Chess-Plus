using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Stores the state of a board
 */
public class boardState
{
    private Piece[,] boardArray;

    //The size of the board. For setUpBoard, this must be 8x8 board.
    public readonly int boardSize = 8;

    //Stores the Piece objects
    //Stores the Piece chess pieces. Goes from 0-7 for col and row.
    //Note, it is stored [col, row].
    //Note, 0 on this is col 1. 7 on this is col 8. This is because arrays start from index 0. So to convert from col, row to this array, use col -1, row - 1
    private Team currentPlayer;
    private bool bLCastle; //Stores whether you can castle in this direction (eg neither king nor rook has moved)
    private bool bRCastle; //It's black left, black right, white left and white right
    private bool wLCastle; //TODO rename castling variables to kingside and queenside
    private bool wRCastle;
    private Coordinate enPassant; //Stores a location that can be en-passanted. can be null

    private Team playerTeam = Team.White;
    private Team enemyTeam = Team.Black;

    public enum GameResult
    {
        Ongoing,
        GameWon,
        GameLost,
        Stalemate
    }

    private GameResult gameResult = GameResult.Ongoing;

    //Optional fields, used for efficiency
    private King whiteKing;
    private King blackKing;

    /** Constructor taking in all essential fields */
    public boardState(
        Piece[,] boardArray,
        Team currentPlayer,
        bool bLCastle,
        bool bRCastle,
        bool wLCastle,
        bool wRCastle,
        Coordinate enPassant,
        King whiteKing,
        King blackKing
    )
    {
        this.boardArray = boardArray;
        this.currentPlayer = currentPlayer;
        this.bLCastle = bLCastle;
        this.bRCastle = bRCastle;
        this.wLCastle = wLCastle;
        this.wRCastle = wRCastle;
        this.enPassant = enPassant;
        this.whiteKing = whiteKing;
        this.blackKing = blackKing;
    }

    /**Loads in the board state from a FEN string */
    public boardState(string FENstring)
    {
        string[] FENwords = FENstring.Split(' ');
        boardArray = new Piece[boardSize, boardSize];
        //Setting the board
        int col = 1;
        int row = boardSize;
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
                    case 'p': //TODO dynamic dispatch this?
                        piece = new Pawn(c, new Coordinate(col, row));
                        break;
                    case 'r':
                        piece = new Rook(c, new Coordinate(col, row));
                        break;
                    case 'n':
                        piece = new Knight(c, new Coordinate(col, row));
                        break;
                    case 'b':
                        piece = new Bishop(c, new Coordinate(col, row));
                        break;
                    case 'q':
                        piece = new Queen(c, new Coordinate(col, row));
                        break;
                    case 'k':
                        piece = new King(c, new Coordinate(col, row));
                        //store white and black kings
                        if (piece.getTeam() == Team.White)
                        {
                            whiteKing = (King)piece;
                        }
                        else if (piece.getTeam() == Team.Black)
                        {
                            blackKing = (King)piece;
                        }
                        break;
                    default:
                        throw new System.Exception(
                            "boardState had an oopsie in a switch statement"
                        );
                }
                boardArray[col - 1, row - 1] = piece;
                col++;
            }
        }

        //The player turn
        if (FENwords[1].Equals("w"))
        {
            this.currentPlayer = Team.White;
        }
        else
        {
            this.currentPlayer = Team.Black;
        }

        //Castling
        if (!FENwords[2].Contains("K"))
        {
            wRCastle = true;
        }
        if (!FENwords[2].Contains("Q"))
        {
            wLCastle = true;
        }
        if (!FENwords[2].Contains("k"))
        {
            bLCastle = true;
        }
        if (!FENwords[2].Contains("q"))
        {
            bRCastle = true;
        }

        //En Passant square
        if (!FENwords[3].Contains("-"))
        {
            string positionString = char.ToString(FENwords[3][0]) + char.ToString(FENwords[3][1]);
            enPassant = new Coordinate(positionString);
        }

        //TODO Implement halfmoves??? (50 moves without pawn progression or killing = draw)

        //TODO Implement fullmoves??? Stores how many turns have elapsed
    }

    /** Returns the board state as a FEN string */
    public string toFEN()
    {
        string fenString = "";
        //Storing board setup
        for (int row = boardSize; row >= 1; row--)
        {
            int emptySpaces = 0;
            for (int col = 1; col <= boardSize; col++)
            {
                if (boardArray[col - 1, row - 1] != null)
                {
                    Piece currentPiece = boardArray[col - 1, row - 1];
                    if (emptySpaces != 0)
                    {
                        fenString += emptySpaces;
                    }
                    fenString += currentPiece.ToString();
                    emptySpaces = 0;
                }
                else
                {
                    emptySpaces++;
                }
            }
            if (emptySpaces != 0)
            {
                fenString += emptySpaces;
            }
            if (row != 1)
            {
                fenString += char.ToString('/');
            }
        }
        return fenString + endOfFen();
    }

/**Returns the end of the FEN string (all but the board state)*/
    private string endOfFen(){
        string fenString = "";
        //The player turn
        if (this.currentPlayer == Team.White)
        {
            fenString += " w ";
        }
        else
        {
            fenString += " b ";
        }

        //Castling
        if (wRCastle)
        {
            fenString += "K";
        }
        if (wLCastle)
        {
            fenString += "Q";
        }
        if (bLCastle)
        {
            fenString += "k";
        }
        if (bRCastle)
        {
            fenString += "q";
        }
        fenString += ' ';

        //En Passant square
        if (this.enPassant == null)
        {
            fenString += "- ";
        }
        else
        {
            fenString += enPassant.ToString() + " ";
        }

        //Implement halfmoves??? (50 moves without pawn progression or killing = draw)

        //Implement fullmoves??? Stores how many turns have elapsed

        return fenString;
    }

/**Sets the fields whiteKing and blackKing*/
    public void setKings(){
                for (int row = boardSize; row >= 1; row--)
        {
            for (int col = 1; col <= boardSize; col++)
            {
                if(boardArray[col-1,row-1] is King){
                    if(boardArray[col-1,row-1].getTeam() == Team.White){
                        whiteKing = (King)boardArray[col-1,row-1];
                    }else{
                        blackKing = (King)boardArray[col-1,row-1];
                    }
                }
            }
        }
    }

    public override string ToString(){
        string boardString = "";
        for (int row = boardSize; row >= 1; row--)
        {
            for (int col = 1; col <= boardSize; col++)
            {
                if(getPiece(col,row) != null){
                boardString += getPiece(col,row).typeToChar();
                }else{
                    boardString += "_";
                }
            }
            boardString += "\n";
        }
        return boardString + endOfFen();
    }

    //Returns the chess piece for a certain col and row.
    public Piece getPiece(int col, int row)
    {
        return boardArray[col - 1, row - 1];
    }

    //Returns the chess piece for a certain Coordinate
    public Piece getPiece(Coordinate coor)
    {
        return boardArray[coor.getCol() - 1, coor.getRow() - 1];
    }

    public void setPiece(Coordinate pos, Piece newPiece)
    {
        Piece oldPiece = boardArray[pos.getCol() - 1, pos.getRow() - 1];
        boardArray[pos.getCol() - 1, pos.getRow() - 1] = newPiece;
    }

    //Returns a King for a certain team
    public King getKing(Team team)
    {
        if (team == Team.White)
        {
            return whiteKing;
        }
        else if (team == Team.Black)
        {
            return blackKing;
        }
        Messages.Log(MessageType.Error, "wasn't black or white team");
        throw new System.Exception("getKing had error");
    }

    /**Returns the en passant position */
    public Coordinate enPassantPos()
    {
        return this.enPassant;
    }

    /**Set en passant position*/
    public void setEnPassant(Coordinate coor)
    {
        this.enPassant = coor;
    }

    /**Returns the team currently playing */
    public Team currentTeam()
    {
        return this.currentPlayer;
    }

    public void setTeam(Team team)
    {
        this.currentPlayer = team;
    }

    /**Returns which Team is the player */
    public Team playersTeam()
    {
        return this.playerTeam;
    }

    /**Returns which Team is the enemy */
    public Team enemysTeam()
    {
        return this.enemyTeam;
    }

    /**Gets the value for whether this team can castle on this side or not
    The left is true if you want to know if you can castle on the left, false if you're checking the right */
    public bool canCastle(Team team, bool left)
    {
        if (team == Team.White)
        {
            if (left)
            {
                return wLCastle;
            }
            else
            {
                return wRCastle;
            }
        }
        else if (team == Team.Black)
        {
            if (left)
            {
                return bLCastle;
            }
            else
            {
                return bRCastle;
            }
        }
        Messages.Log(MessageType.Error, "canCastle had a booboo");
        throw new System.Exception(":-(");
    }

    /**Sets the values for castling, whether you can or can't castle on whichever side*/
    public void setCastle(Team team, bool left, bool value)
    {
        if (team == Team.White)
        {
            if (left)
            {
                wLCastle = value;
            }
            else
            {
                wRCastle = value;
            }
        }
        else if (team == Team.Black)
        {
            if (left)
            {
                bLCastle = value;
            }
            else
            {
                bRCastle = value;
            }
        }
    }

    /**Returns the game result*/
    public GameResult getGameResult()
    {
        return this.gameResult;
    }

    /**Sets the game result*/
    public void setGameResult(GameResult gr)
    {
        this.gameResult = gr;
    }

    /**Clones a boardState.
    Makes clones of everything EXCEPT the gameObject the Pieces refer to */
    public boardState clone()
    {
        Piece[,] newBoardArray = new Piece[
            this.boardArray.GetLength(0),
            this.boardArray.GetLength(1)
        ];
        for (int row = 0; row < this.boardArray.GetLength(0); row++)
        {
            for (int col = 0; col < this.boardArray.GetLength(1); col++)
            {
                if (boardArray[col, row] != null)
                {
                    newBoardArray[col, row] = boardArray[col, row].clonePiece();
                }
            }
        }
        boardState clone = new boardState(
            newBoardArray,
            this.currentPlayer,
            this.bLCastle,
            this.bRCastle,
            this.wLCastle,
            this.wRCastle,
            this.enPassant,
            (King)newBoardArray[whiteKing.getPos().getCol()-1,whiteKing.getPos().getRow()-1],
            (King)newBoardArray[blackKing.getPos().getCol()-1,blackKing.getPos().getRow()-1]
        );
        return clone;
    }

    //Checks if a spot in boardArray is NOT an ally with a certain chess piece
    public bool spotNotAlly(Piece piece, Coordinate coor)
    {
        return getPiece(coor) == null || piece.getTeam() != getPiece(coor).getTeam();
    }

    //Checks if a spot in boardArray is of the opposite team of Piece
    public bool spotIsEnemy(Piece piece, Coordinate coor)
    {
        return this.getPiece(coor) != null && piece.getTeam() != this.getPiece(coor).getTeam();
    }
}
