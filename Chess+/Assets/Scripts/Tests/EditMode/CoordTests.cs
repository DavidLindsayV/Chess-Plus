using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CoordTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void MakeCoord()
    {
        Coordinate c = new Coordinate(7, 5);
        Assert.AreEqual(c.getCol(), 7);
        Assert.AreEqual(c.getRow(), 5);
        Coordinate c2 = new Coordinate("g3");
        Assert.AreEqual(c2.getCol(), 7);
        Assert.AreEqual(c2.getRow(), 3);
    }
[Test]
    public void moveTest(){
        Coordinate c = new Coordinate(3,4);
        Coordinate c2 = c.move(-2, 3);
        Assert.AreEqual(c.getCol(), 3);
        Assert.AreEqual(c.getRow(), 4);
        Assert.AreEqual(c2.getCol(), 1);
        Assert.AreEqual(c2.getRow(), 7);
    }
[Test]
    public void toString(){
        Coordinate c = new Coordinate(3, 4);
        Assert.AreEqual("c4", c.ToString());
    }
}
