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
    public void moveBuidler(int Builder, Coordinate to, Game g)
    {
        Builder builderToMove = Builder == 1 ? Builder1 : Builder2;

        builderToMove.move(to, g);
    }
    
    public virtual Turn getNextTurn()
    {
        return turns[turns.Count - 1];
    }

    public virtual string PlaceBuilder(int v, Game game)
    {
        throw new NotImplementedException();
    }

    public int getBuilderInt(Coordinate location)
    {
        Debug.Log(getBuilderLocations() + " " + Coordinate.coordToString(location));
        if (Coordinate.Equals(Builder1.getLocation(),location)) return 1;
        if (Coordinate.Equals(Builder2.getLocation(),location)) return 2;
        throw new Exception("No builder at this location");
    }
}
