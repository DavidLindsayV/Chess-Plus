using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EditTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void EditTestsSimplePasses()
    {
        Assert.AreEqual(Team.White, Team.Black.nextTeam());
    }
}
