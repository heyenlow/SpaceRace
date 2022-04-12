﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SimPlayer
{
    public SimBuilder Builder1, Builder2;
    public int ID;
    public IPlayer.States state;
    public SimPlayer()
    {
        // nothing happens here...
    }
    public string getBuilderLocations()
    {
        return (Builder1.getLocation() + Builder2.getLocation());
    }
    public void moveBuilder(int Builder, Coordinate to, Game g)
    {
        SimBuilder builderToMove = Builder == 1 ? Builder1 : Builder2;
        builderToMove.move(to, g);
    }
    public IEnumerator PlaceBuilder(int builder, int player, Game g)
    {
        //Coordinate builder1 = new Coordinate(0, 0);
        //Coordinate builder2 = new Coordinate(1, 1);
        //if (builder == 1) { moveBuidler(builder, builder1, g); } else { moveBuidler(builder, builder2, g); }
        // this player is now temporarily the current player
        // the rival is whichever player this isn't
        System.Random seed = new System.Random();
        System.Random rnd1 = new System.Random(seed.Next(0, (int)(int.MaxValue / 2)));
        System.Random rnd2 = new System.Random(seed.Next((int)(int.MaxValue / 2), int.MaxValue));
        Coordinate tmp = new Coordinate();
        int x, y;
        if (g.Rival.getBuilders().Item1.coord.x == -1 && g.Rival.getBuilders().Item1.coord.y == -1)
        {
            if (builder == 1)
        }
        else
        {
            // opponent's first builder is defined, but not guaranteed the second is defined...
            if (g.Rival.getBuilders().Item2.coord.x == -1 && g.Rival.getBuilders().Item2.coord.y == -1)
            {
                // place a builder near the opponent's first builder.
                findFreeSpots(g, g.Rival.getBuilders().Item1.getLocation().x, g.Rival.getBuilders().Item1.getLocation().y, builder);
            }
            else
            {
                x = Math.Abs((g.Rival.getBuilders().Item1.coord.x + g.Rival.getBuilders().Item2.coord.x) / 2);
                y = Math.Abs((g.Rival.getBuilders().Item1.coord.y + g.Rival.getBuilders().Item2.coord.y) / 2);
                findFreeSpots(g, x, y, builder); // neat player places second builder close to opponent's builder averaged out
            }
        }
        yield return true;
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
    public SimPlayer(SimPlayer other)
    {
        Builder1 = new SimBuilder(other.Builder1);
        Builder2 = new SimBuilder(other.Builder2);
        state = other.state;
        ID = other.ID;
    }
}