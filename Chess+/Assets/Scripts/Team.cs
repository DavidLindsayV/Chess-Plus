using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team
{
    private enum myTeam{White, Black};
    public static Team White = new Team(myTeam.White);
    public static Team Black = new Team(myTeam.Black);
    private myTeam mt;

    private Team(myTeam mt){
        this.mt = mt;
    }

    /**
     * A toString method
     */
    public string toString()
    {
        return mt.ToString();
    }

    public Team nextTeam(){
        if (this == Team.White){
            return Team.Black;
        }else if(this == Team.Black){
            return Team.White;
        }
        Messages.Log(MessageType.Error, "You introduced more teams but didn't update nextPlayer()");
        throw new System.Exception("There was an error in nextPlayer()");
    }
}
