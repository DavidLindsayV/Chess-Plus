using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Class for storing position on the board.
 * Row and col go from 1 to 8
 * Row 1 col 1 is white's left rook
 */
public class Coordinate : MonoBehaviour
{
    private int row;
    private int col;

    /** Constructs a coordinate */
    public Coordinate(int col, int row)
    {
        if(row >= 1 && row <= 8 && col >= 1 && col <= 8)
        {
            throw new System.Exception("Invalid coordinate constructed, row: " + row + " col: " + col);
        }
        this.row = row;
        this.col = col;
    }

    /** Constructs a Coordinate from a string */
    public Coordinate(string coord)
    {
        switch (coord[0])
        {
            case 'a':
                this.col = 1;
                break;
            case 'b':
                this.col = 2;
                break;
            case 'c':
                this.col = 3;
                break;
            case 'd':
                this.col = 4;
                break;
            case 'e':
                this.col = 5;
                break;
            case 'f':
                this.col = 6;
                break;
            case 'g':
                this.col = 7;
                break;
            case 'h':
                this.col = 8;
                break;
        }
        this.row = coord[1] - '0';
    }
    /**
     * Returns the row of a coordinate
     */
    public int getRow() { return this.row;  }

    /**
     * Returns the column of a coordinate
     */
    public int getCol() { return this.col;  }

    /**
     * A toString method
     */
    public string toString()
    {
        string toReturn = "";
        switch (this.col)
        {
            case 1:
                toReturn += 'a';
                break;
            case 2:
                toReturn += 'b';
                break;
            case 3:
                toReturn += 'c';
                break;
            case 4:
                toReturn += 'd';
                break;
            case 5:
                toReturn += 'e';
                break;
            case 6:
                toReturn += 'f';
                break;
            case 7:
                toReturn += 'g';
                break;
            case 8:
                toReturn += 'h';
                break;
        }
        return toReturn + this.row;
    }
}
