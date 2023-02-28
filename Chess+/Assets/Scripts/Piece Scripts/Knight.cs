using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    public Knight(Team team, Coordinate pos)
        : base(team, pos) { }

    public Knight(Team team, Coordinate pos, GameObject gameObj)
        : base(team, pos, gameObj) { }

    public override char typeToChar(){
        return 'n';
    }

        public override Piece clonePiece(){
        return new Knight(this.getTeam(), this.getPos(), null);
    }


        //Returns the moves for a Knight
    public override List<Move> getMoves(BoardState bState)
    {
        List<Move> moves = new List<Move>();
        int col = this.getPos().getCol();
        int row = this.getPos().getRow();
        for (int a = -2; a <= 2; a = a + 4)
            for (int b = -1; b <= 1; b = b + 2)
            {
                if (Coordinate.inBounds(col + a, row + b, bState) && bState.spotNotAlly(this, new Coordinate(col + a, row + b)))
                {
                    moves.Add(
                        new PieceMove(this.getPos(), new Coordinate(col + a, row + b))
                    );
                }
                if (Coordinate.inBounds(col + b, row + a, bState) && bState.spotNotAlly(this, new Coordinate(col + b, row + a)))
                {
                    moves.Add(
                        new PieceMove(this.getPos(), new Coordinate(col + b, row + a))
                    );
                }
            }
        return moves;
    }

    public override List<Move> getAttackingMoves(BoardState bState){
        return this.getMoves(bState);
    }

    public override void makePiece()
    {
        base.makePiece();
        if (this.getTeam() == Team.Black)
        {
            this.gameObj.transform.rotation = Quaternion.Euler(-90, 180, 0);
        }
    }
}
