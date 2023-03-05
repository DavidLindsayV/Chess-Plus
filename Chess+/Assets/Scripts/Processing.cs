using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Class for static functions which process the boardState
 and give information such as inCheck or validMoves
 */
public static class Processing
{
    /**Gets all of the valid legal moves for a team
    Accounts for not going into check/needing to escape from check*/
    public static List<Move> allValidMoves(BoardState bState, Team team)
    {
        List<Move> moves = allMoves(bState, team);
        removeCheckingMoves(bState, moves, team);
        return moves;
    }

    /**This gets the valid + allowed moves (don't put you in check) for a single specified piece
    NOT including cards moves */
    public static List<Move> validMoves(BoardState bState, Piece piece)
    {
        List<Move> moves = piece.getMoves(bState);
        removeCheckingMoves(bState, moves, piece.getTeam());
        return moves;
    }

    /**This gets the valid + allowed moves (don't put you in check) for a single specified Card */
    public static List<Move> validMoves(BoardState bState, Card card, Team team)
    {
        List<Move> moves = new List<Move>();
        moves.AddRange(card.getGeneralMoves(bState));
        for (int col = 1; col <= bState.boardSize; col++)
            for (int row = 1; row <= bState.boardSize; row++)
            {
                moves.AddRange(card.getCoordSpecificMoves(bState, new Coordinate(col, row)));
            }
        removeCheckingMoves(bState, moves, team);
        return moves;
    }

    /**Used to see if a move puts the king in check. If it does, it is not a valid move.
    This is more efficient than using updateSafeSquares for checking for a particular spot*/
    public static bool inCheck(BoardState bState, Team team)
    {
        return inDanger(bState, team, bState.getKing(team).getPos());
    }

    /**A version of inCheck that sees if any enemy moves end at the destination pos specified*/
    public static bool inDanger(BoardState bState, Team team, Coordinate pos)
    {
        Team enemyTeam = team.nextTeam();
        List<Move> allEnemyMoves = allAttackingPieceMoves(bState, enemyTeam);
        //Sees if the attacking Piece moves of the enemy team put this position in danger
        foreach (Move move in allEnemyMoves)
        {
            if (move.inDanger(pos)) { return true; }
        }
        //Sees if the general card moves of the enemy team put this pos in danger
        foreach (Move move in bState.getHand(enemyTeam).generalMoves(bState))
        {
            if (move.inDanger(pos)) { return true; }
        }
        //Sees if the coordinate-specific card moves put this pos in danger
        for (int col = 1; col <= bState.boardSize; col++)
            for (int row = 1; row <= bState.boardSize; row++)
            {
                foreach (Move move in bState.getHand(enemyTeam).coordSpecificMoves(bState, new Coordinate(col,row)))
                {
                    if (move.inDanger(pos)) { return true; }
                }
            }
        return false;
    }

    /**Takes a list of moves and removes all the Moves that put the king in check*/
    private static void removeCheckingMoves(BoardState bState, List<Move> moves, Team team)
    {
        //Removes all the moves that cause the player to be in check
        for (int i = 0; i < moves.Count; i++)
        {
            Move move = moves[i];
            //Simulate doing the move
            BoardState cloneState = bState.clone();
            simulateMove(cloneState, move);
            if (inCheck(cloneState, team))
            {
                moves.RemoveAt(i); //If the king is in check, remove that move
                i--;
            }
        }
    }

    /**This method returns the killed piece
This method is used if a move is to be done but the move might be used again later
A cloned boardState should be passed in */
    public static Piece simulateMove(BoardState bState, Move move)
    {
        Piece killedPiece = move.doMoveState(bState);
        move.resetMove();
        return killedPiece;
    }

    /**Returns a list of all of the moves of a certain team*/
    private static List<Move> allMoves(BoardState bState, Team team)
    {
        List<Move> allmoves = new List<Move>();
        Hand hand = bState.getHand(team);
        //Gets the moves from Cards that don't require positions
        allmoves.AddRange(hand.generalMoves(bState));
        //Goes through each position in the board
        for (int col = 1; col <= bState.boardSize; col++)
            for (int row = 1; row <= bState.boardSize; row++)
            {
                Coordinate c = new Coordinate(col, row);
                //Adds the moves for the movement of a specific piece
                if (bState.getPiece(c) != null && bState.getPiece(c).getTeam() == team)
                {
                    Piece p = bState.getPiece(c);
                    allmoves.AddRange(p.getMoves(bState));
                }
                //Adds the moves that can be played on this Coordinate on the board from Cards
                allmoves.AddRange(bState.getHand(team).coordSpecificMoves(bState, c));
            }
        return allmoves;
    }

    /**Returns a list of all of the Attacking moves of a certain team*/
    private static List<Move> allAttackingPieceMoves(BoardState bState, Team team)
    {
        List<Move> allmoves = new List<Move>();
        for (int col = 1; col <= bState.boardSize; col++)
            for (int row = 1; row <= bState.boardSize; row++)
            { //Goes through each Piece in boardArray, and if its in the right team it adds all of its moves to the list its going to return
                Coordinate c = new Coordinate(col, row);
                if (bState.getPiece(c) != null && bState.getPiece(c).getTeam() == team)
                {
                    allmoves.AddRange(bState.getPiece(c).getAttackingMoves(bState));
                }
            }
        return allmoves;
    }

    //Checks for Stalemate and Checkmate and updates GameResult
    public static void updateGameResult(BoardState bState, Team team)
    {
        List<Move> moves = allMoves(bState, team);
        removeCheckingMoves(bState, moves, team);
        if (moves.Count == 0)
        {
            //If there are no moves that don't leave you in check, you're either in stalemate
            //or checkmate
            if (inCheck(bState, team))
            { //If you have no moves and you're in check, its checkmate
                if (team == bState.playersTeam())
                {
                    bState.setGameResult(BoardState.GameResult.GameLost);
                }
                else
                {
                    bState.setGameResult(BoardState.GameResult.GameWon);
                }
            }
            else
            {
                bState.setGameResult(BoardState.GameResult.Stalemate);
            }
        }

        //TODO update updateGameResult so that it can cope with:
        //- king being killed
        //- ending a turn in check
        //normal piece movements shouldn't allow this, but if cards get weird and funky enough these become possible
        //so make sure these don't cause errors
    }
}
