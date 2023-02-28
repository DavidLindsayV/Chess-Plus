using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**Stores the Team of pieces (currently black and white) and limited functionality associated with that
Should be an IMMUTABLE class
*/
public class Team
{
    private enum myTeam
    {
        White,
        Black
    };

    public static readonly Team White = new Team(myTeam.White);
    public static readonly Team Black = new Team(myTeam.Black);
    private myTeam mt;

    private Team(myTeam mt)
    {
        this.mt = mt;
    }

    /**
     * A toString method
     */
    public override string ToString()
    {
        return mt.ToString();
    }

    public Team nextTeam()
    {
        if (this == Team.White)
        {
            return Team.Black;
        }
        else if (this == Team.Black)
        {
            return Team.White;
        }
        Messages.Log(MessageType.Error, "You introduced more teams but didn't update nextPlayer()");
        throw new System.Exception("There was an error in nextPlayer()");
    }
}
