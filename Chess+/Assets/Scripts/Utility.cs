using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
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

    //Returns whether a col and row are in bounds
    private static bool inBounds(int col, int row)
    {
        return (col >= 1 && col <= 8 && row >= 1 && row <= 8);
    }
}
