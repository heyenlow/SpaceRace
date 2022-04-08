using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimPlayer : SimIPlayer
{
    public SimPlayer()
    {
        // nothing happens here...
    }

    public SimPlayer(int id)
    {
        ID = id;
    }

    public SimPlayer(IPlayer other)
    {
        Builder1 = new SimBuilder(Coordinate.stringToCoord(other.getBuilderLocations().Substring(0, 2)));
        Builder2 = new SimBuilder(Coordinate.stringToCoord(other.getBuilderLocations().Substring(2, 2)));
        ID = other.ID;
    }

    public SimPlayer(SimIPlayer other)
    {
        Builder1 = new SimBuilder(other.Builder1);
        Builder2 = new SimBuilder(other.Builder2);
        state = other.state;
        ID = other.ID;
    }

    public override string beginTurn(SimGame g, out bool isWin)
    {
        throw new System.NotImplementedException();
    }

    public override Coordinate chooseBuild(ref Coordinate builder, ref Coordinate oldLocatiion, Game g)
    {
        throw new System.NotImplementedException();
    }

    public override Coordinate chooseMove(ref Coordinate builder, Game g)
    {
        throw new System.NotImplementedException();
    }

    internal void PlaceBuilders()
    {
        throw new NotImplementedException();
    }

    public override Coordinate SelectBuilder()
    {
        throw new System.NotImplementedException();
    }
}
