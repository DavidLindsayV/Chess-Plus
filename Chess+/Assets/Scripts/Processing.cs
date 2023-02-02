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
    public static List<Move> allValidMoves(boardState bState, Team team)
    {
        List<Move> moves = allMoves(bState, team);
        removeCheckingMoves(bState, moves, team);
        return moves;
    }

    /**This gets the valid + allowed moves (don't put you in check) for a single specified piece */
    public static List<Move> validMoves(boardState bState, Piece piece)
    {
        List<Move> moves = piece.getMoves(bState);
        removeCheckingMoves(bState, moves, piece.getTeam());
        return moves;
    }

    //Used to see if a move puts the king in check. If it does, it is not a valid move.
    //This is more efficient than using updateSafeSquares for checking for a particular spot
    public static bool inCheck(boardState bState, Team team)
    {
        return inDanger(bState, team, bState.getKing(team).getPos());
    }

    /**A version of inCheck that sees if any enemy moves end at the destination pos specified*/
    public static bool inDanger(boardState bState, Team team, Coordinate pos)
    {
        List<Move> allEnemyMoves = allAttackingMoves(bState, team.nextTeam());
        foreach (Move move in allEnemyMoves)
        {
            if (move.getTo() == pos)
            {
                return true;
            }
        }
        return false;
    }

    //Takes a list of moves and removes all the Moves that put the king in check
    private static void removeCheckingMoves(boardState bState, List<Move> moves, Team team)
    {
        //Removes all the moves that cause the player to be in check
        for (int i = 0; i < moves.Count; i++)
        {
            Move move = moves[i];
            //Simulate doing the move
            boardState cloneState = bState.clone();
            move.doMoveState(cloneState);
            move.resetMove();
            if (inCheck(cloneState, team))
            {
                moves.RemoveAt(i); //If the king is in check, remove that move
                i--;
            }
        }
    }

    /**Returns a list of all of the moves of a certain team*/
    private static List<Move> allMoves(boardState bState, Team team)
    {
        List<Move> allmoves = new List<Move>(); 
        //TODO make move generation/checking more efficient
        //Currently pieces make the same move multiple times. Fix this?
        for (int col = 1; col <= bState.boardSize; col++)
            for (int row = 1; row <= bState.boardSize; row++)
            { //Goes through each Piece in boardArray, and if its in the right team it adds all of its moves to the list its going to return
                Coordinate c = new Coordinate(col, row);
                if (bState.getPiece(c) != null && bState.getPiece(c).getTeam() == team)
                {
                    allmoves.AddRange(bState.getPiece(c).getMoves(bState));
                }
            }
        return allmoves;
    }

    /**Returns a list of all of the Attacking moves of a certain team*/
    private static List<Move> allAttackingMoves(boardState bState, Team team)
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
    public static void updateGameResult(boardState bState, Team team)
    { 
        checkForStaleMate(bState, team); //sets the value of staleMate
        if (bState.getGameResult() != boardState.GameResult.Ongoing)
        {
            return;
        }
        checkForMate(bState, team);
    }

    //Checks if the game is in a stale mate (if the game is not in check but there are no valid moves (that don't put you in check)
    private static void checkForStaleMate(boardState bState, Team team)
    {
        List<Move> moves = allMoves(bState, team);
        removeCheckingMoves(bState, moves, team);
        if (moves.Count == 0)
        {
            bState.setGameResult(boardState.GameResult.Stalemate);
        } //If there are no valid moves that don't put you in check, then its a stalemate
    }

    private static void checkForMate(boardState bState, Team team)
    {
        //See if you're in check. If not, stop there.
        if (!inCheck(bState, team))
        {
            return;
        }
        //If you are in check, check all possible moves and see if you can avoid check
        //This is more efficient than checking if allValidMoves is of size 0
        //because if an escape from check is found the program terminates early
        List<Move> allTeamMoves = allMoves(bState, team);
        foreach (Move move in allTeamMoves) //This goes through all the moves of your team and simulates them, then sees if you're still in check.
        { //If no move you can do prevents check, then you're in checkmate
            boardState cloneState = bState.clone();
            move.doMoveState(cloneState);
            move.resetMove();
            if (!inCheck(cloneState, team))
            {
                return;
            }
        }
        if (team == bState.playersTeam())
        {
            bState.setGameResult(boardState.GameResult.GameLost);
        }
        else
        {
            bState.setGameResult(boardState.GameResult.GameWon);
        }
    }
}
