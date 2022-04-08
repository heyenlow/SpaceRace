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

    public SimPlayer(SimIPlayer other)
    {
        Builder1 = new SimBuilder(other.Builder1);
        Builder2 = new SimBuilder(other.Builder2);
        state = other.state;
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

    public override Coordinate SelectBuilder()
    {
        throw new System.NotImplementedException();
    }
}
