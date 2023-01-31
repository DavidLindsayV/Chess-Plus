using System; //TODO lessen this import into a more specified library for allowing use of Enum.GetName
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        return Enum.GetName(typeof(myTeam), mt);
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
