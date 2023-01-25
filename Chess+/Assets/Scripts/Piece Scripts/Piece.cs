using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * A class to represent a chess piece
 */
public abstract class Piece : MonoBehaviour
{
    //Fields for a generic piece
    private Team team;
    private Coordinate position;

    protected GameObject gameObj;

    /** Constructs a Piece object */
    public Piece(Team team, Coordinate pos)
    {
        this.team = team;
        this.position = pos;
        makePiece();
    }

    /**Constructs a piece from it's FEN string character and position */
    public Piece(char FENchar, Coordinate pos)
    {
        if (char.IsUpper(FENchar))
        {
            this.team = Team.White;
        }
        else
        {
            this.team = Team.Black;
        }
        this.position = pos;
        makePiece();
    }

    /**For creating pieces referencing already existing gameObjects
    Currently used for modelling theoretical board states without creating extra gameObjects */
    public Piece(Team team, Coordinate pos, GameObject gameObj)
    {
        this.team = team;
        this.position = pos;
        this.gameObj = gameObj;
    }

    /** Get the team */
    public Team getTeam()
    {
        return this.team;
    }

    /** To FEN string */
    public string toString()
    {
        char c = typeToChar(this);
        if (this.team == Team.White)
        {
            c = char.ToUpper(c);
        }
        return char.ToString(c);
    }

    /**Converts a Piece class to FEN char (does not take into account Team) */
    public static char typeToChar(Piece type)
    {
        if (type is Pawn)
        {
            return 'p';
        }
        if (type is Rook)
        {
            return 'r';
        }
        if (type is Knight)
        {
            return 'n';
        }
        if (type is Bishop)
        {
            return 'b';
        }
        if (type is Queen)
        {
            return 'q';
        }
        if (type is King)
        {
            return 'k';
        }
        throw new System.Exception("Invalid fen char");
    }

    public virtual void makePiece()
    {
        float y = 0.5F;
        Quaternion rotation = Quaternion.Euler(-90, 0, 0);
        this.gameObj = Instantiate(
            Prefabs.getPrefab(this),
            new Vector3(this.position.getX(), y, this.position.getZ()),
            rotation
        );
        this.gameObj.transform.localScale = new Vector3(35, 35, 35);
        if (this.team == Team.White)
        {
            this.gameObj.GetComponent<Renderer>().material = Prefabs.white;
        }
        else
        {
            this.gameObj.GetComponent<Renderer>().material = Prefabs.black;
        }
        this.gameObj.name = this.team.ToString() + char.ToUpper(typeToChar(this));
    }

    public void destroy()
    {
        Destroy(this.gameObj);
    }

    public GameObject getObject()
    {
        return this.gameObj;
    }

    public Coordinate getPos()
    {
        return this.position;
    }

    public abstract bool isValidMove(boardState bState, Move move);
    public abstract List<Move> getValidMoves(boardState bState);
}
