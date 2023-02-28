using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public Pawn(Team team, Coordinate pos)
        : base(team, pos) { }

    public Pawn(Team team, Coordinate pos, GameObject gameObj)
        : base(team, pos, gameObj) { }

    public override char typeToChar()
    {
        return 'p';
    }

    public override Piece clonePiece()
    {
        return new Pawn(this.getTeam(), this.getPos(), null);
    }

    //Returns the moves for a Pawn
    public override List<Move> getMoves(BoardState bState)
    {
        List<Move> moves = getAttackingMoves(bState);
        int col = this.getPos().getCol();
        int row = this.getPos().getRow();
        int direction = -1;
        if (this.getTeam() == Team.White)
        {
            direction = 1;
        }
        bool promotion = false;
        if ((direction == 1 && row + direction == bState.boardSize) || (direction == -1 && row + direction == 1))
        {
            promotion = true;
        }

        if (
            Coordinate.inBounds(col, row + direction, bState)
            && bState.getPiece(col, row + direction) == null
        )
        {
            if (!promotion)
            {
                moves.Add(new PieceMove(this.getPos(), new Coordinate(col, row + direction)));
            }
            else
            {
                moves.AddRange(promotionMoves(new Coordinate(col, row + direction)));
            }
            //The "row == 4.5 + 2.5*direction" ensures that the pawn is in its original starting row, hence hasn't moved
            if (
                row == 4.5 + 2.5 * -direction
                && Coordinate.inBounds(col, row + 2 * direction, bState)
                && bState.getPiece(col, row + 2 * direction) == null
            )
            { //This is for moving 2 spaces forwards. Don't check for promotion here.
                moves.Add((new PawnDoublejump(this.getPos(), new Coordinate(col, row + 2 * direction))));
            }
        }
        return moves;
    }

/**Creates the promotion moves */
    private List<Move> promotionMoves(Coordinate to)
    {
        //The piece constructors take in a specific game object so makePiece isn't called
        //(null is given so the pieces don't yet have gameObjects)
        Move m1 = new PromoteMove(this.getPos(), to, new Queen(this.getTeam(), to, null));
        Move m2 = new PromoteMove(this.getPos(), to, new Rook(this.getTeam(), to, null));
        Move m3 = new PromoteMove(this.getPos(), to, new Bishop(this.getTeam(), to, null));
        Move m4 = new PromoteMove(this.getPos(), to, new Knight(this.getTeam(), to, null));
        List<Move> moves = new List<Move>();
        moves.Add(m1);
        moves.Add(m2);
        moves.Add(m3);
        moves.Add(m4);
        return moves;
    }

    public override List<Move> getAttackingMoves(BoardState bState)
    {
        List<Move> moves = new List<Move>();
        int col = this.getPos().getCol();
        int row = this.getPos().getRow();
        int direction = -1;
        if (this.getTeam() == Team.White)
        {
            direction = 1;
        }
        bool promotion = false;
        if ((direction == 1 && row + direction == bState.boardSize) || (direction == -1 && row + direction == 1))
        {
            promotion = true;
        }
        if (
            Coordinate.inBounds(col - 1, row + direction, bState)
            && bState.spotIsEnemy(this, new Coordinate(col - 1, row + direction))
        )
        {
            if (!promotion)
            {
                moves.Add(new PieceMove(this.getPos(), new Coordinate(col - 1, row + direction)));
            }
            else
            {
                moves.AddRange(promotionMoves(new Coordinate(col - 1, row + direction)));
            }
        }
        if (
            Coordinate.inBounds(col + 1, row + direction, bState)
            && bState.spotIsEnemy(this, new Coordinate(col + 1, row + direction))
        )
        {
            if (!promotion)
            {
                moves.Add(new PieceMove(this.getPos(), new Coordinate(col + 1, row + direction)));
            }
            else
            {
                moves.AddRange(promotionMoves(new Coordinate(col + 1, row + direction)));
            }
        }
        //En Passant
        if (
            bState.enPassantPos() != null
            && bState.enPassantPos().getRow() == row + direction
            && Mathf.Abs(bState.enPassantPos().getCol() - col) == 1
        )
        {
            moves.Add(
                (
                    new EnPassantMove(
                        this.getPos(),
                        new Coordinate(bState.enPassantPos().getCol(), row + direction)
                    )
                )
            );
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
