using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Stores the state of a board
 */
public class BoardState
{
    private Piece[,] boardArray;

    //The size of the board. For setUpBoard, this must be 8x8 board.
    public readonly int boardSize = 8;

    //Stores the Piece objects
    //Stores the Piece chess pieces. Goes from 0-7 for col and row.
    //It is stored [col, row].
    //0 on this is col 1. 7 on this is col 8. This is because arrays start from index 0. So to convert from col, row to this array, use col -1, row - 1
    private Team currentPlayer;

    //Queenside is left from white's perspective, and kingside is right
    private bool bQCastle; //Stores whether you can castle in this direction (eg neither king nor rook has moved)
    private bool bKCastle; //It's black queenside, black kngside, white queenside and white kingside
    private bool wQCastle;
    private bool wKCastle;
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

    //Card fields
    private Hand whiteHand;
    private Hand blackHand;

    //Optional fields, used for efficiency
    private King whiteKing;
    private King blackKing;

    /** Constructor taking in all essential fields */
    public BoardState(
        Piece[,] boardArray,
        Team currentPlayer,
        bool bQCastle,
        bool bKCastle,
        bool wQCastle,
        bool wKCastle,
        Coordinate enPassant,
        King whiteKing,
        King blackKing,
        Hand whiteHand,
        Hand blackHand
    )
    {
        this.boardArray = boardArray;
        this.currentPlayer = currentPlayer;
        this.bQCastle = bQCastle;
        this.bKCastle = bKCastle;
        this.wQCastle = wQCastle;
        this.wKCastle = wKCastle;
        this.enPassant = enPassant;
        this.whiteKing = whiteKing;
        this.blackKing = blackKing;
        this.whiteHand = whiteHand;
        this.blackHand = blackHand;
    }

    /**Loads in the board state from a FEN string */
    public BoardState(string FENstring)
    //TODO let your FEN string specify if player is white or black
    // make a way of having the hand and decks generated by FEN string
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
                Coordinate coor = new Coordinate(col, row);
                Team team = Team.White;
                if (char.IsLower(c))
                {
                    team = Team.Black;
                }
                switch (char.ToLower(c))
                {
                    case 'p':
                        piece = new Pawn(team, coor);
                        break;
                    case 'r':
                        piece = new Rook(team, coor);
                        break;
                    case 'n':
                        piece = new Knight(team, coor);
                        break;
                    case 'b':
                        piece = new Bishop(team, coor);
                        break;
                    case 'q':
                        piece = new Queen(team, coor);
                        break;
                    case 'k':
                        piece = new King(team, coor);
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
        if (FENwords[2].Contains("K"))
        {
            wKCastle = true;
        }
        if (FENwords[2].Contains("Q"))
        {
            wQCastle = true;
        }
        if (FENwords[2].Contains("k"))
        {
            bKCastle = true;
        }
        if (FENwords[2].Contains("q"))
        {
            bQCastle = true;
        }

        //En Passant square
        if (!FENwords[3].Contains("-"))
        {
            string positionString = char.ToString(FENwords[3][0]) + char.ToString(FENwords[3][1]);
            enPassant = new Coordinate(positionString);
        }

        switch (FENwords[4])
        {
            case "B":
                if (playersTeam() == Team.Black) { gameResult = GameResult.GameWon; } else { gameResult = GameResult.GameLost; }
                break;
            case "W":
                if (playersTeam() == Team.White) { gameResult = GameResult.GameWon; } else { gameResult = GameResult.GameLost; }
                break;
            case "O":
                this.gameResult = GameResult.Ongoing;
                break;
        }

        Deck whiteDeck = new Deck(Team.White, Deck.DefaultCards);
        Deck blackDeck = new Deck(Team.Black, Deck.DefaultCards);
        this.whiteHand = new Hand(whiteDeck);
        this.blackHand = new Hand(blackDeck);
        if (playersTeam() == Team.White)
        {
            whiteHand.makeCardObjs();
        }
        else if (playersTeam() == Team.Black)
        {
            blackHand.makeCardObjs();
        }
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
    private string endOfFen()
    {
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

        if (wKCastle)
        {
            fenString += "K";
        }
        if (wQCastle)
        {
            fenString += "Q";
        }
        if (bKCastle)
        {
            fenString += "k";
        }
        if (bQCastle)
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

        //Include the current result of the game - ongoing O, black won B, white won W, stalemate S
        switch (gameResult)
        {
            case GameResult.Ongoing:
                fenString += "O";
                break;
            case GameResult.GameWon:
                if (playersTeam() == Team.White) { fenString += "W"; } else { fenString += "B"; }
                break;
            case GameResult.GameLost:
                if (playersTeam() == Team.White) { fenString += "B"; } else { fenString += "W"; }
                break;
            case GameResult.Stalemate:
                fenString += "S";
                break;
        }

        return fenString;
    }

    public override string ToString()
    {
        string boardString = "";
        for (int row = boardSize; row >= 1; row--)
        {
            for (int col = 1; col <= boardSize; col++)
            {
                if (getPiece(col, row) != null)
                {
                    boardString += getPiece(col, row).ToString();
                }
                else
                {
                    boardString += "_";
                }
            }
            boardString += "\r\n";
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

    /**Gets the Deck of the team specified */
    public Deck getDeck(Team team)
    {
        if (team == Team.White)
        {
            return whiteHand.getDeck();
        }
        else if (team == Team.Black)
        {
            return blackHand.getDeck();
        }
        return null;
    }

    /**Gets the Deck of the team specified */
    public Hand getHand(Team team)
    {
        if (team == Team.White)
        {
            return whiteHand;
        }
        else if (team == Team.Black)
        {
            return blackHand;
        }
        return null;
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
    public bool canCastle(Team team, bool queenside)
    {
        if (team == Team.White)
        {
            if (queenside)
            {
                return wQCastle;
            }
            else
            {
                return wKCastle;
            }
        }
        else if (team == Team.Black)
        {
            if (queenside)
            {
                return bQCastle;
            }
            else
            {
                return bKCastle;
            }
        }
        Messages.Log(MessageType.Error, "canCastle had a booboo");
        throw new System.Exception(":-(");
    }

    /**Sets the values for castling, whether you can or can't castle on whichever side*/
    public void setCastle(Team team, bool queenside, bool value)
    {
        if (team == Team.White)
        {
            if (queenside)
            {
                wQCastle = value;
            }
            else
            {
                wKCastle = value;
            }
        }
        else if (team == Team.Black)
        {
            if (queenside)
            {
                bQCastle = value;
            }
            else
            {
                bKCastle = value;
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
    Makes clones of everything EXCEPT the gameObjects the Pieces and cards etc refer to */
    public BoardState clone()
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
        BoardState clone = new BoardState(
            newBoardArray,
            this.currentPlayer,
            this.bQCastle,
            this.bKCastle,
            this.wQCastle,
            this.wKCastle,
            this.enPassant,
            (King)newBoardArray[whiteKing.getPos().getCol() - 1, whiteKing.getPos().getRow() - 1],
            (King)newBoardArray[blackKing.getPos().getCol() - 1, blackKing.getPos().getRow() - 1],
            whiteHand.clone(),
            blackHand.clone()

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
