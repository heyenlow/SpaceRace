using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimGame : MonoBehaviour
{
    int[,] Board;
    public int[,] state
    {
        get
        {
            return Board;
        }
        set { }
    }
    public IPlayer Player1;
    public IPlayer Player2;
    public int moveNum = 0;

    public IPlayer CurrentPlayer => (moveNum % 2 == 0) ? Player1 : Player2;
    public IPlayer Rival => (moveNum % 2 == 0) ? Player2 : Player1;
    public static bool cancelTurn = false;

    void PlaceBuilders()
    {
        PlaceBuilder(Player1, 1);
        PlaceBuilder(Player1, 2);
        moveNum++; // increment move to flip CurrentPlayer and Rival properties.
        PlaceBuilder(Player2, 1);
        PlaceBuilder(Player2, 2);
        moveNum--; // decrement moveNum back to 0 so that the game begins as normal with Player1
    }

    public SimGame(SimGame g)
    {
        Board = new int[5, 5];
        System.Array.Copy(g.state, g.state.GetLowerBound(0), Board, Board.GetLowerBound(0), 25);
        Player1 = new MCTSPlayer(g.Player1);
        Player2 = new MCTSPlayer(g.Player2);

    }


    public void PlaceBuilder(IPlayer p, int i)
    {
        if (p is Player)
        {
            string s;
            do
            {
                Console.Write(p.ToString() + " place Builder " + i + ": ");
                s = Console.ReadLine().ToUpper();
            }
            while (getAllBuildersString().Contains(s) || !Coordinate.inBounds(Coordinate.stringToCoord(s)));

            p.PlaceBuilder(i, Coordinate.stringToCoord(s));
        }
        else if (p is MMPlayer)
        {
            MMPlayer mmp = (MMPlayer)p;
            mmp.placeBuilders(this, i);
        }
        else if (p is NeatPlayer)
        {
            // ASSUMES NEAT PLAYER IS GOING SECOND
            NeatPlayer np = (NeatPlayer)p;
            np.placeBuilders(this, i);
        }
        else
        {
            MCTSPlayer pltmp = (MCTSPlayer)p;
            pltmp.placeBuilders(this, i);
        }
    }
}
