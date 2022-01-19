//using UnityEngine;

using System;
using SpaceConsole1;

public class Game //: MonoBehaviour
{
    int[,] Board;
    Player Player1;
    Player Player2;
    Player Winner;

    // Start is called before the first frame update
    public void Start()
    {
        Winner = null;
        Board = new int[5,5];
        Player1 = new Player("Player 1");
        Player2 = new Player("Player 2");
        Player1.Start();
        Player2.Start();
        NewGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void print() {

        for (int i = 0; i < 5; ++i)
        {
            Console.Write((char)('A'+i) + ":");
            for (int j = 0; j < 5; ++j)
            {
                Console.Write(Board[i, j] + " ");
            }
            Console.WriteLine();
        }
        Console.WriteLine("  0 1 2 3 4");
        printBuilderLocations();
    }

    void NewGame()
    {
        ClearBoard();
        print();
        PlaceBuilders();
        RunGame();
    }

    void RunGame()
    {
        while(Winner == null)
        {
            print();
            Turn(Player1);
            print();
            if(Winner == null) Turn(Player2);
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
        for(int i = 0; i < 5; ++i)
        {
            for(int j = 0; j < 5; ++j)
            {
                Board[i, j] = 0;
            }
        }
    }

    //increase a location to 0
    void BuildLevel(Coordinate c)
    {
        Board[c.x,c.y] += 1;
    }

    bool isWin(Coordinate c)
    {
        return Board[c.x, c.y] == 3;
    }

    void Turn(Player p)
    {
        string builderLocation;
        Console.Write("Builder Location: ");
        do
        {
            builderLocation = Console.ReadLine().ToUpper();
        } while (!p.getBuilderLocations().Contains(builderLocation));

        string moveLocation;
        string allMoves = getAllPossibleMoves(Coordinate.stringToCoord(builderLocation));

        Console.Write("New Location: ");
        do
        {
            moveLocation = Console.ReadLine().ToUpper();
        } while (!allMoves.Contains(moveLocation));

        if (!isWin(Coordinate.stringToCoord(moveLocation)))
        {
            string buildLocation;
            string allBuilds = getAllPossibleBuilds(Coordinate.stringToCoord(moveLocation));
            Console.Write("Build Location: ");
            do
            {
                buildLocation = Console.ReadLine().ToUpper();
            } while (!allBuilds.Contains(buildLocation));
            BuildLevel(Coordinate.stringToCoord(buildLocation));
            Winner = p;
        }

        p.moveBuidler(Coordinate.stringToCoord(builderLocation), Coordinate.stringToCoord(moveLocation));
    }


    Tuple<string,string> getAllBuilderLocations()
    {
        return new Tuple<string, string>(Player1.getBuilderLocations(), Player2.getBuilderLocations());
    }

    string getAllBuildersString()
    {
        var b = getAllBuilderLocations();
        return b.Item1 + b.Item2;
    }

    public void printBuilderLocations()
    {
        var builders = getAllBuilderLocations();
        Console.WriteLine("Player 1: " + builders.Item1.Substring(0, 2) + " " + builders.Item1.Substring(2, 2));
        Console.WriteLine("Player 2: " + builders.Item2.Substring(0, 2) + " " + builders.Item2.Substring(2, 2));
    }

    public bool isMoveValid(Coordinate c)
    {
        var builders = getAllBuilderLocations();
        var allBuilders = builders.Item1 + builders.Item2;
        return Coordinate.inBounds(c) && Board[c.x, c.y] < 4 && !allBuilders.Contains(Coordinate.coordToString(c));
    }

    public bool locationClearOfAllBuilders(Coordinate c)
    {
        return !getAllBuildersString().Contains(Coordinate.coordToString(c));
    }

    public string getAllPossibleMoves(Coordinate c)
    {
        string allMoves = "";
        for(int i = -1; i <= 1; ++i)
        {
            for (int j = -1; j <= 1; ++j)
            {
                Coordinate test = new Coordinate(c.x + i, c.y + j);
                if (Coordinate.inBounds(test) && Board[test.x, test.y] <= (Board[c.x,c.y] + 1) && locationClearOfAllBuilders(test))
                {
                    allMoves += Coordinate.coordToString(test);
                }
            }
        }
        Console.WriteLine("All possible moves: " + allMoves);
        return allMoves;
    }

    public string getAllPossibleBuilds(Coordinate c)
    {
        string allBuilds = "";
        for (int i = -1; i <= 1; ++i)
        {
            for (int j = -1; j <= 1; ++j)
            {
                Coordinate test = new Coordinate(c.x + i, c.y + j);
                if (Coordinate.inBounds(test) && Board[test.x, test.y] < 4 && locationClearOfAllBuilders(test))
                {
                    allBuilds += Coordinate.coordToString(test);
                }
            }
        }
        Console.WriteLine("All possible builds: " + allBuilds);
        return allBuilds;
    }
}
