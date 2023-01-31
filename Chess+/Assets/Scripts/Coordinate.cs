using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Class for storing position on the board.
 * Row and col go from 1 to 8
 * Row 1 col 1 is white's left rook
 * A coordinate object once constructed cannot change its values
 Should be an IMMUTABLE class
 */
public class Coordinate
{
    private readonly int row;
    private readonly int col;

    /** Constructs a coordinate */
    public Coordinate(int col, int row)
    {
        this.row = row; //TODO replace all 8s with references to boardSize
        this.col = col;
    }

    /** Constructs a Coordinate from a string */
    public Coordinate(string coord)
    {
        this.col = decimal.ToInt32(char.ToLower(coord[0])) - 96;
        this.row = coord[1] - '0';
    }

    /**Returns the coordinate if it moved by col and row*/
    public Coordinate move(int colChange, int rowChange)
    {
        return new Coordinate(this.col + colChange, this.row + rowChange);
    }

    /**
     * Returns the row of a coordinate
     */
    public int getRow()
    {
        return this.row;
    }

    /**
     * Returns the column of a coordinate
     */
    public int getCol()
    {
        return this.col;
    }

    /**
     * A toString method
     */
    public override string ToString()
    {
        return (char)(this.col+96) + this.row.ToString();
    }

    //Returns whether a col and row are in bounds
    public static bool inBounds(int col, int row)
    {
        return (col >= 1 && col <= 8 && row >= 1 && row <= 8);
    }

    //Returns whether this coordinate is in bounds
    public bool inBounds()
    {
        return inBounds(this.col, this.row);
    }

    //Converts a x to Col, or a Z to Row.
    public static int xToCol(float x)
    {
        x = Mathf.Round(x + 0.5F) - 0.5F;
        return (int)(x + 4.5);
    }

    //Converts a col to X, or row to Z
    //Note: col should really be an int. But taking in a float prevents typecasting.
    public static float colToX(float col)
    {
        return (col - 4.5F);
    }

    public float getX()
    {
        return colToX(this.col);
    }

    public float getZ()
    {
        return colToX(this.row);
    }
}
