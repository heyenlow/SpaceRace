using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimBuilder : MonoBehaviour
{
    Coordinate coord;

    public Coordinate Location { get { return coord; } }

    public Builder()
    {
        coord = new Coordinate();
    }

    public Builder(Builder b)
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
