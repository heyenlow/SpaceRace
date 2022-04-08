using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimBuilder : MonoBehaviour
{
    Coordinate coord;

    public Coordinate Location { get { return coord; } }

    public SimBuilder()
    {
        coord = new Coordinate();
    }

    public SimBuilder(Coordinate b)
    {
        coord = new Coordinate(b);
    }

    public SimBuilder(SimBuilder b)
    {
        coord = new Coordinate(b.Location);
    }

    public void move(Coordinate c)
    {
        coord.x = c.x;
        coord.y = c.y;
    }

    public string getLocation()
    {
        return Coordinate.coordToString(coord);
    }
}
