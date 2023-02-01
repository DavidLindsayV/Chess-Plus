using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    public King(Team team, Coordinate pos)
        : base(team, pos) { }

    public King(Team team, Coordinate pos, GameObject gameObj)
        : base(team, pos, gameObj) { }

    public override char typeToChar()
    {
        return 'k';
    }

        public override Piece clonePiece(){
        return new King(this.getTeam(), this.getPos(), this.getObject());
    }

    public override List<Move> getMoves(boardState bState)
    {
        int col = this.getPos().getCol();
        int row = this.getPos().getRow();
        List<Move> moves = getAttackingMoves(bState);
        //Check for castling
        //Castling does not scale with board size
        bool check = Processing.inCheck(bState, this.getTeam());
        Team team = this.getTeam();
        bool leftCastle = bState.canCastle(team, true);
        bool rightCastle = bState.canCastle(team, false);
        if (
            leftCastle
            && !check
            && bState.getPiece(2, row) == null
            && bState.getPiece(3, row) == null
            && bState.getPiece(4, row) == null
            && !Processing.inDanger(bState, this.getTeam(), new Coordinate(2, row))
            && !Processing.inDanger(bState, this.getTeam(), new Coordinate(3, row))
        )
        {
            Move move = new CastlingMove(
                this.getPos(),
                new Coordinate(1, row),
                this.getPos().move(-2, 0),
                new Coordinate(col - 1, row)
            );
            moves.Add(move);
        }
        if (
            rightCastle
            && !check
            && bState.getPiece(7, row) == null
            && bState.getPiece(6, row) == null
            && !Processing.inDanger(bState, this.getTeam(), new Coordinate(5, row))
            && !Processing.inDanger(bState, this.getTeam(), new Coordinate(6, row))
        ) 
        { 
            Move move = new CastlingMove(
                this.getPos(),
                new Coordinate(8, row),
                this.getPos().move(2, 0),
                this.getPos().move(-2, 0)
            );
            moves.Add(move);
        }
        return moves;
    }

    public override List<Move> getAttackingMoves(boardState bState){
        int col = this.getPos().getCol();
        int row = this.getPos().getRow();
        List<Move> moves = new List<Move>();
        for (int a = -1; a <= 1; a++)
            for (int b = -1; b <= 1; b++)
            {
                Coordinate newCoord = this.getPos().move(a, b);
                if ((a != 0 || b != 0) && newCoord.inBounds(bState) && bState.spotNotAlly(this, newCoord))
                {
                    moves.Add(new Move(this.getPos(), newCoord));
                }
            }
        return moves;
    }

    public override void makePiece()
    {
        base.makePiece();
        Vector3 vec = this.gameObj.transform.position;
        vec.y = 0.2F;
        this.gameObj.transform.position = vec;
    }
}
