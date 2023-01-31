using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**The thinking and planning for the enemy chess AI */
public class AI
{
    public static List<Move> getMaxPriMoves(boardState bState, List<Move> AIMoves, Team team)
    {
        int maxPriority = 0; 
        List<Move> maxPriMoves = new List<Move>();
        foreach (Move move in AIMoves)
        {
            int movePri = movePriority(bState, move, team);
            if (movePri == maxPriority)
            {
                maxPriMoves.Add(move);
            }
            if (movePri > maxPriority)
            {
                maxPriority = movePri;
                maxPriMoves.Clear();
                maxPriMoves.Add(move);
            }
        }
        return maxPriMoves;
    }

    /**Generates a "priority" for the move: how good it is. BASIC right now*/
    private static int movePriority(boardState bState, Move move, Team team)
    {
        boardState cloneState = bState.clone();
        Piece killedPiece = move.doMoveState(cloneState);
        bool checking = Processing.inCheck(cloneState, team.nextTeam());

        if (checking)
        {
            return 10;
        }
        if (killedPiece != null)
        {
            if (killedPiece is King)
            {
                Messages.Log(
                    MessageType.Error,
                    "This shouldn't be possible. The game should end with checkmate before this happens"
                );
            }
            if (killedPiece is Queen)
            {
                return 9;
            }
            if (killedPiece is Rook || killedPiece is Bishop)
            {
                return 8;
            }
            if (killedPiece is Knight)
            {
                return 7;
            }
            if (killedPiece is Pawn)
            {
                return 6;
            }
        }
        return 0; //If killedPiece == null and its not checking
    }
}
