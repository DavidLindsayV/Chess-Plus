using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Text.RegularExpressions;

public class bStateTests
{

    // A Test behaves as an ordinary method
    [Test]
    public void BoardInitialises()
    {
        string board =
@"rnbqkbnr
pppppppp
________
________
________
________
PPPPPPPP
RNBQKBNR
 w KQkq - O";
        runMoves(new List<Coordinate>(), new List<Coordinate>(), board);
    }

    [Test]
    public void WhiteMoves()
    {
        string board =
@"rnbqkbnr
pppppppp
________
________
________
______P_
PPPPPP_P
RNBQKBNR
 b KQkq - O";
        List<Coordinate> froms = new List<Coordinate>();
        List<Coordinate> tos = new List<Coordinate>();
        froms.Add(new Coordinate(7, 2)); tos.Add(new Coordinate(7, 3));
        runMoves(froms, tos, board);
    }

    [Test]
    public void Stalemate()
    {
        string board =
@"_____bnr
____p_pq
____Qpkr
_______p
__P____P
________
PP_PPPP_
RNB_KBNR
 b KQ - S";
        List<Coordinate> froms = new List<Coordinate>{
    new Coordinate("c2"), new Coordinate("h7"), new Coordinate("h2"), new Coordinate("a7"), new Coordinate("d1"), new Coordinate("a8"), new Coordinate("a4"), new Coordinate("a6"), new Coordinate("a5"), new Coordinate("f7"), new Coordinate("c7"), new Coordinate("e8"), new Coordinate("d7"), new Coordinate("d8"), new Coordinate("b7"), new Coordinate("d3"), new Coordinate("b8"), new Coordinate("f7"), new Coordinate("c8")};
        List<Coordinate> tos = new List<Coordinate>{
    new Coordinate("c4"), new Coordinate("h5"), new Coordinate("h4"), new Coordinate("a5"), new Coordinate("a4"), new Coordinate("a6"), new Coordinate("a5"), new Coordinate("h6"), new Coordinate("c7"), new Coordinate("f6"), new Coordinate("d7"), new Coordinate("f7"), new Coordinate("b7"), new Coordinate("d3"), new Coordinate("b8"), new Coordinate("h7"), new Coordinate("c8"), new Coordinate("g6"), new Coordinate("e6")};
        runMoves(froms, tos, board);
    }

    [Test]
    public void Pawna2Toa3()
    {
        string board =
@"rnbqkbnr
pppppppp
________
________
________
P_______
_PPPPPPP
RNBQKBNR
 b KQkq - O";
        List<Coordinate> froms = new List<Coordinate>{
    new Coordinate("a2")};
        List<Coordinate> tos = new List<Coordinate>{
    new Coordinate("a3") };
        runMoves(froms, tos, board);
    }

    //TODO take runMoves (both versions) and some other useful testing functions and put them in a separate file so they're 
    //better organised instead of higgledy piggledy across your tests

    /**Basic test checking that a series of moves, stored as from and to coordinates, result in a certain board state
Cannot check for Checks at the correct times
Cannot do Promotion, as several moves have the same from/to coordinates 
It is kept because it is used in preexisting tests, but it is difficult to use as it requires
lists of coordinates 
Cannot do things such as check when the board is in Check, or do moves where there are multiple options
with the same start & end coordinates (such as promotion)*/
    public static void runMoves(List<Coordinate> froms, List<Coordinate> tos, string board)
    {
        BoardState bState = makeBoard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - O");
        for (int i = 0; i < froms.Count; i++)
        {
            Coordinate from = froms[i];
            Coordinate to = tos[i];
            //Look through all the valid moves, find one which the From and To match.
            Move move = null;
            List<Move> allMoves = Processing.allValidMoves(bState, bState.currentTeam());
            foreach (Move m in allMoves)
            {
                if (m is PieceMove)
                {
                    PieceMove piecemove = (PieceMove)m;
                    if (piecemove.getFrom().Equals(from) && piecemove.getTo().Equals(to))
                    {
                        move = m;
                        break;
                    }
                }
            }
            if (move == null)
            {
                Assert.Fail("Move " + froms[i].ToString() + "-" + tos[i].ToString() + " not valid");
            }
            Assert.AreEqual(bState.getPiece(from).getTeam(), bState.currentTeam());
            move.doMoveState(bState);
            Processing.updateGameResult(bState, bState.currentTeam().nextTeam());
            bState.setTeam(bState.currentTeam().nextTeam());
        }
        Assert.AreEqual(board, bState.ToString());
    }

    /** The constructor for a board from a FEN string that doesn't create gameobjects
*/
    public static BoardState makeBoard(string FENstring)
    {
        string[] FENwords = FENstring.Split(' ');
        Piece[,] boardArray = new Piece[8, 8];
        King whiteKing = null;
        King blackKing = null;
        //Setting the board
        int col = 1;
        int row = 8;
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
                Team team = Team.White;
                if (char.IsLower(c)) { team = Team.Black; }
                switch (char.ToLower(c))
                {
                    case 'p':
                        piece = new Pawn(team, new Coordinate(col, row), null);
                        break;
                    case 'r':
                        piece = new Rook(team, new Coordinate(col, row), null);
                        break;
                    case 'n':
                        piece = new Knight(team, new Coordinate(col, row), null);
                        break;
                    case 'b':
                        piece = new Bishop(team, new Coordinate(col, row), null);
                        break;
                    case 'q':
                        piece = new Queen(team, new Coordinate(col, row), null);
                        break;
                    case 'k':
                        piece = new King(team, new Coordinate(col, row), null);
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
        Team currentPlayer;
        if (FENwords[1].Equals("w"))
        {
            currentPlayer = Team.White;
        }
        else
        {
            currentPlayer = Team.Black;
        }
        bool wKCastle = false;
        bool wQCastle = false;
        bool bKCastle = false;
        bool bQCastle = false;
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
        Coordinate enPassant = null;
        if (!FENwords[3].Contains("-"))
        {
            string positionString = char.ToString(FENwords[3][0]) + char.ToString(FENwords[3][1]);
            enPassant = new Coordinate(positionString);
        }
        Deck whiteDeck =  new Deck(Team.White, Deck.DefaultCards);
        Deck blackDeck = new Deck(Team.Black, Deck.DefaultCards);
        BoardState b = new BoardState(boardArray, currentPlayer, bQCastle, bKCastle, wQCastle, wKCastle, enPassant, whiteKing, blackKing, new Hand(whiteDeck), new Hand(blackDeck)); //TODO fix this 4 nulls at the end. Update this board construction to be the same as what you have under boardState

        BoardState.GameResult g = (BoardState.GameResult)(-1);
        switch (FENwords[4])
        {
            case "B":
                if (b.playersTeam() == Team.Black) { g = BoardState.GameResult.GameWon; } else { g = BoardState.GameResult.GameLost; }
                break;
            case "W":
                if (b.playersTeam() == Team.White) { g = BoardState.GameResult.GameWon; } else { g = BoardState.GameResult.GameLost; }
                break;
            case "O":
                g = BoardState.GameResult.Ongoing;
                break;
        }

        b.setGameResult(g);
        return b;
    }
}
