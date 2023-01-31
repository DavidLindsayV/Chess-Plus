using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TeamTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void WhiteNotBlack()
    {
        Assert.AreNotEqual(Team.White, Team.Black);
    }
    [Test]
    public void WhiteNextTeam(){
        Assert.AreEqual(Team.Black, Team.White.nextTeam());
    }
    [Test]
    public void BlackNextTeam(){
        Assert.AreEqual(Team.White, Team.Black.nextTeam());
    }
    [Test]
    public void TeamToString(){
        Assert.AreEqual(Team.White.ToString(), "White");
        Assert.AreEqual(Team.Black.ToString(), "Black");
    }
}
