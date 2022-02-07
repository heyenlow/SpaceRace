using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IPlayer : MonoBehaviour
{
    protected Builder Builder1;
    protected Builder Builder2;
    protected List<Turn> turns = new List<Turn>();

    public abstract IEnumerator beginTurn(Game g);
    /// <summary>
    /// Returns a string showing the current coordinates of this player's builders
    /// </summary>
    /// <returns></returns>
    public string getBuilderLocations()
    {
        return (Coordinate.coordToString(Builder1.getLocation()) + Coordinate.coordToString(Builder2.getLocation()));
    }
    public void moveBuidler(Coordinate from, Coordinate to, Game g)
    {
        ///buggg
        if (Builder1.getLocation() == from)
        {
            Builder1.move(to, g);
        }
        else if (Builder2.getLocation() == from)
        {
            Builder2.move(to, g);
        }

    }

    //places builders at 0,0 and 0,1 if not a player
    public string PlaceBuilder(int i, Game g)
    {
        moveBuidler(i == 1 ? Builder1.getLocation() : Builder2.getLocation(), i == 1 ? new Coordinate(0,0) : new Coordinate(0,1), g);
        return getBuilderLocations();
    }
    public Turn getLastTurn()
    {
        return turns[turns.Count - 1];
    }
}
