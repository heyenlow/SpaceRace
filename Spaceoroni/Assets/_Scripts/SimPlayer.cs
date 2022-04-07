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
    public override Coordinate SelectBuilder()
    {
        //which builder do you want to move
        string builderLocation;
        do
        {
            //builderLocation = Console.ReadLine().ToUpper();
        } while (!getBuilderLocations().Contains(builderLocation));

        return Coordinate.stringToCoord(builderLocation);
    }

    public override Coordinate chooseMove(ref Coordinate builder, Game g)
    {
        //###################################################################
        //Where to?
        string moveLocation;
        List<string> allMoves = g.getAllPossibleMoves(builder);

        do
        {
            moveLocation = Console.ReadLine().ToUpper();
        } while (!allMoves.Contains(moveLocation));

        return Coordinate.stringToCoord(moveLocation);
    }

    public override Coordinate chooseBuild(ref Coordinate move, ref Coordinate oldLocation, Game g)
    {

        string buildLocation;
        List<string> allBuilds = g.getAllPossibleBuilds(move, oldLocation);
        Console.Write("Build Location: ");
        do
        {
            buildLocation = Console.ReadLine().ToUpper();
        } while (!allBuilds.Contains(buildLocation));

        return Coordinate.stringToCoord(buildLocation);
    }

    public override string beginTurn(SimGame g, out bool isWin)
    {
        // after activation choose a builder
        Coordinate builder = SelectBuilder();

        // after choosing a builder, find the best square you can move to from it.
        Coordinate move = chooseMove(ref builder, g);


        // after choosing a move, need to find the best square to build on. What do I do about this?
        if (g.isWin(move))
        {
            isWin = true;
            return Coordinate.coordToString(builder) + Coordinate.coordToString(move);
        }

        isWin = false;
        Coordinate build = chooseBuild(ref move, ref builder, g);
        return Coordinate.coordToString(builder) + Coordinate.coordToString(move) + Coordinate.coordToString(build);
    }
}
