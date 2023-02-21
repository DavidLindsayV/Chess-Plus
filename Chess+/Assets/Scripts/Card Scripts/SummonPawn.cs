using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**Summon a Pawn onto any empty space*/
public class SummonPawn: Card
{

    public override List<CardMove> getPieceSpecificMoves(BoardState bState, Team team, Piece piece){
        return new List<CardMove>();
    }
    public override List<CardMove> getGeneralMoves(BoardState bState){
        return null;
    }
}
