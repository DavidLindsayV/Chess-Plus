using UnityEngine;

/**The MoveHolder is a script attached to each moveTile. It basically stores which Move is associated with which tile. */
public class MoveHolder : MonoBehaviour
{
    //Stores the Move the tile is associated with
    private Move move;

    /**Sets the Move */
    public void setMove(Move move)
    {
        this.move = move;
    }

    /** Returns the Move */
    public Move getMove()
    {
        return move;
    }

/**Returns how many moves this move tile is storing  TODO upgrade movetiles to store multiple moves*/
    public int getMoveCount(){
        return 1;
    }
}
