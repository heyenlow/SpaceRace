using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public abstract class SimIPlayer// : MonoBehaviour
{
    public int ID;
    public enum States
    {
        Winner,
        Loser,
        Tied,
        Undetermined
    }
    public SimBuilder Builder1 = new SimBuilder();
    public SimBuilder Builder2 = new SimBuilder();
    public States state = States.Undetermined;

    public abstract Coordinate SelectBuilder();
    public abstract Coordinate chooseMove(ref Coordinate builder, Game g);
    public abstract Coordinate chooseBuild(ref Coordinate builder, ref Coordinate oldLocatiion, Game g);
    public abstract string beginTurn(SimGame g, out bool isWin);

    /// <summary>
    /// Returns a string showing the current coordinates of this player's builders
    /// </summary>
    /// <returns></returns>
    public string getBuilderLocations()
    {
        return (Builder1.getLocation() + Builder2.getLocation());
    }

    //used to place builders at the beginning of the game
    public virtual void PlaceBuilder(int i, Coordinate c)
    {
        switch (i)
        {
            case 1:
                Builder1.move(c);
                break;
            case 2:
                Builder2.move(c);
                break;
        }
    }
    public void moveBuilder(Coordinate from, Coordinate to)
    {
        if (Builder1.getLocation() == Coordinate.coordToString(from))
        {
            Builder1.move(to);
        }
        else if (Builder2.getLocation() == Coordinate.coordToString(from))
        {
            Builder2.move(to);
        }
    }
}
