using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Material highlight;

    int[,] Board;
    Player Player1;
    Player Player2;
    Player Winner;
    // Start is called before the first frame update
    void Start()
    {
        Winner = null;
        Board = new int[5, 5];
        Player1 = GameObject.Find("Player1").GetComponent<Player>();
        Player2 = GameObject.Find("Player2").GetComponent<Player>();
        NewGame();
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    

    void NewGame()
    {
        ClearBoard();
        PlaceBuilders();
        RunGame();
    }

    void RunGame()
    {
        while (Winner == null)
        {
            Turn(Player1);
            if (Winner == null) Turn(Player2);
        }
    }

    void PlaceBuilders()
    {
        PlaceBuilder(Player1, 1);
        PlaceBuilder(Player2, 1);
        PlaceBuilder(Player1, 2);
        PlaceBuilder(Player2, 2);
    }

    public void PlaceBuilder(Player p, int i)
    {
        string s;
        do
        {
            Console.Write(p.name + ", Place Builder " + i + ": ");
            s = Console.ReadLine().ToUpper();
        }
        while (getAllBuildersString().Contains(s) || !Coordinate.inBounds(Coordinate.stringToCoord(s)));

        p.PlaceBuilder(i, Coordinate.stringToCoord(s));
    }

    //set board back to 0
    public void ClearBoard()
    {
        for (int i = 0; i < 5; ++i)
        {
            for (int j = 0; j < 5; ++j)
            {
                Board[i, j] = 0;
            }
        }
    }

    //increase a location to 0
    void BuildLevel(Coordinate c)
    {
        Board[c.x, c.y] += 1;
    }

    bool isWin(Coordinate c)
    {
        return Board[c.x, c.y] == 3;
    }

    void Turn(Player p)
    {
        //which builder do you want to move
        string builderLocation;
        Console.Write("Builder Location: ");
        do
        {
            builderLocation = Console.ReadLine().ToUpper();
        } while (!p.getBuilderLocations().Contains(builderLocation));

        //###################################################################
        //Where to?
        string moveLocation;
        List<string> allMoves = getAllPossibleMoves(Coordinate.stringToCoord(builderLocation));

        Console.Write("New Location: ");
        do
        {
            moveLocation = Console.ReadLine().ToUpper();
        } while (!allMoves.Contains(moveLocation));
        p.moveBuidler(Coordinate.stringToCoord(builderLocation), Coordinate.stringToCoord(moveLocation));

        //################################################################
        // get build location if not win
        if (!isWin(Coordinate.stringToCoord(moveLocation)))
        {
            string buildLocation;
            List<string> allBuilds = getAllPossibleBuilds(Coordinate.stringToCoord(moveLocation));
            Console.Write("Build Location: ");
            do
            {
                buildLocation = Console.ReadLine().ToUpper();
            } while (!allBuilds.Contains(buildLocation));
            BuildLevel(Coordinate.stringToCoord(buildLocation));
        }

    }

    // MARKED FOR DEPRECATION ?
    Tuple<string, string> getAllBuilderLocations()
    {
        return new Tuple<string, string>(Player1.getBuilderLocations(), Player2.getBuilderLocations());
    }

    // PULLS 4 BUILDER LOCATIONS and returns string ie. A2B3C4D3
    string getAllBuildersString()
    {
        var b = getAllBuilderLocations();
        return b.Item1 + b.Item2;
    }

    //makes sure there are no builders in that location
    public bool locationClearOfAllBuilders(Coordinate c)
    {
        return !getAllBuildersString().Contains(Coordinate.coordToString(c));
    }

    //returns all the posible positions able to move from the passed coord
    public List<string> getAllPossibleMoves(Coordinate c)
    {
        List<string> allMoves = new List<string>();
        for (int i = -1; i <= 1; ++i)
        {
            for (int j = -1; j <= 1; ++j)
            {
                Coordinate test = new Coordinate(c.x + i, c.y + j);
                //isValidMove Function candidate
                if (Coordinate.inBounds(test) && Board[test.x, test.y] <= (Board[c.x, c.y] + 1) && Board[test.x, test.y] < 4 && locationClearOfAllBuilders(test))
                {
                    allMoves.Add(Coordinate.coordToString(test));
                }
            }
        }

        return allMoves;
    }

    //returns a string of all the coords someone in that position could build in
    public List<string> getAllPossibleBuilds(Coordinate c)
    {
        List<string> allBuilds = new List<string>();
        for (int i = -1; i <= 1; ++i)
        {
            for (int j = -1; j <= 1; ++j)
            {
                Coordinate test = new Coordinate(c.x + i, c.y + j);
                if (Coordinate.inBounds(test) && Board[test.x, test.y] < 4 && locationClearOfAllBuilders(test))
                {
                    allBuilds.Add(Coordinate.coordToString(test));
                }
            }
        }

        return allBuilds;
    }

    //takes a list of possible moves and highlights the moves
    void highlightPossibleMoveLocations(List<string> locations)
    {
        foreach (string coord in locations)
        {
            GameObject.Find(coord).GetComponent<Renderer>().material = highlight;
        }
    }
}
