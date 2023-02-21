using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand 
{
    List<Card> cards;
    GameObject camera;
    public Hand(Card[] c){
        cards = new List<Card>();
        camera = GameObject.Find("Main Camera");
        foreach(Card card in c){
            cards.Add(card);
        }
    }

//Positions all the cards correctly if none of them are being looked at individually
    public void positionCards(){
        int cardNum = cards.Count;
        float angle = -40;
        float step = 80f/(cardNum-1);
        foreach(Card card in cards){
            GameObject cardObj = card.getObj();
            cardObj.transform.SetParent(camera.transform);
            cardObj.transform.position = new Vector3(0,1,0);
            cardObj.transform.rotation = Quaternion.Euler(0, angle, 0);
            angle += step;
        }
    }

    /**reutrns the moves that need a piece to do them */
    public List<CardMove> pieceSpecificMoves(BoardState bState, Team team, Piece piece){
        List<CardMove> moves = new List<CardMove>();
        foreach(Card c in cards){
            moves.AddRange(c.getPieceSpecificMoves(bState, team, piece));
        }
        return moves;
    }

/**Returns the moves that do not need a specific piece to do them */
    public List<CardMove> generalMoves(BoardState bState, Team team){
        List<CardMove> moves = new List<CardMove>();
        foreach(Card c in cards){
            moves.AddRange(c.getGeneralMoves(bState, team));
        }
        return moves;
    }
}
