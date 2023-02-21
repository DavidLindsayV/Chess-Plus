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
        moves.AddRange(bState.getHand(team).generalMoves(bState));
        removeCheckingMoves(bState, moves, team);
        return moves;
    }

    /**This gets the valid + allowed moves (don't put you in check) for a single specified piece */
    public static List<Move> validMoves(BoardState bState, Piece piece)
    {
        List<Move> moves = piece.getMoves(bState);
        removeCheckingMoves(bState, moves, piece.getTeam());
        return moves;
    }

    //Used to see if a move puts the king in check. If it does, it is not a valid move.
    //This is more efficient than using updateSafeSquares for checking for a particular spot
    public static bool inCheck(BoardState bState, Team team)
    {
        return inDanger(bState, team, bState.getKing(team).getPos());
    }

    /**A version of inCheck that sees if any enemy moves end at the destination pos specified*/
    public static bool inDanger(BoardState bState, Team team, Coordinate pos)
    {
        List<Move> allEnemyMoves = allAttackingMoves(bState, team.nextTeam());
        foreach (Move move in allEnemyMoves)
        {
            if(move.inDanger(pos)){ return true; }
        }
        return false;
    }

    //Takes a list of moves and removes all the Moves that put the king in check
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
    public static Piece simulateMove(BoardState bState, Move move){
        Piece killedPiece = move.doMoveState(bState);
        move.resetMove();
        return killedPiece;
    }

    /**Returns a list of all of the moves of a certain team*/
    private static List<Move> allMoves(BoardState bState, Team team)
    {
        List<Move> allmoves = new List<Move>(); 
        for (int col = 1; col <= bState.boardSize; col++)
            for (int row = 1; row <= bState.boardSize; row++)
            { //Goes through each Piece in boardArray, and if its in the right team it adds all of its moves to the list its going to return
                Coordinate c = new Coordinate(col, row);
                if (bState.getPiece(c) != null && bState.getPiece(c).getTeam() == team)
                {
                    Piece p = bState.getPiece(c);
                    allmoves.AddRange(p.getMoves(bState));
                    allmoves.AddRange(bState.getHand(team).pieceSpecificMoves(bState, team, p));
                }
            }
        return allmoves;
    }

    /**Returns a list of all of the Attacking moves of a certain team*/
    private static List<Move> allAttackingMoves(BoardState bState, Team team)
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
            if(inCheck(bState, team)){ //If you have no moves and you're in check, its checkmate
                if (team == bState.playersTeam())
                {
                    bState.setGameResult(BoardState.GameResult.GameLost);
                }
                else
                {
                    bState.setGameResult(BoardState.GameResult.GameWon);
                }
            }else{
                bState.setGameResult(BoardState.GameResult.Stalemate);
            }
        }
    }
}
