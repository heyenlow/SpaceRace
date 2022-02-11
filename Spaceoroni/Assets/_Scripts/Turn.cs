using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn
{
    public Coordinate BuilderLocation;
    public Coordinate MoveLocation;
    public Coordinate BuildLocation;
    public bool isWin = false;
    public bool canPerformTurn = true;

    public Turn(Coordinate builderLocation, Coordinate moveLocation, Coordinate buildLocation)
    {
        BuilderLocation = builderLocation;
        MoveLocation = moveLocation;
        BuildLocation = buildLocation;
    }

    public Turn(Coordinate builderLocation, Coordinate moveLocation)
    {
        BuilderLocation = builderLocation;
        MoveLocation = moveLocation;
        BuildLocation = null;
        isWin = true;
    }

    public Turn() { }

    public override string ToString()
    {
        return Coordinate.coordToString(BuilderLocation) + Coordinate.coordToString(MoveLocation) + Coordinate.coordToString(BuildLocation);
    }

    public Turn(string s) 
    {
        BuilderLocation = Coordinate.stringToCoord(s.Substring(0, 2));
        MoveLocation = Coordinate.stringToCoord(s.Substring(2, 2));
        BuildLocation = Coordinate.stringToCoord(s.Substring(4, 2));
    }
}
