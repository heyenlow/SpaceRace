using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    int[,] Board;
    Player Player1;
    IPlayer Player2;

    public static Coordinate clickLocation;

    private void Start()
    {
        Board = new int[5, 5];
        Player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<Player>();
        Player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<StringPlayer>();
        ClearBoard();
        StartCoroutine(PlayGameToEnd());
    }

    public bool processTurnString(Turn turn, IPlayer curPlayer, Game g)
    {
        //players already move the builders
        curPlayer.moveBuidler(curPlayer.getBuilderInt(turn.BuilderLocation), turn.MoveLocation, g);
        
       
        // If not over Where to build?
        if (!turn.isWin)
        {
            BuildLevel(turn.BuildLocation);
        }

        // build should be completed?
        return true;
    }

    public IEnumerator PlayGameToEnd()
    {
        IPlayer curPlayer;
        IPlayer winner = null;
        bool won = false;

        yield return null;
        yield return StartCoroutine(PlaceBuilders());

        // Play until we have a winner or tie?
        for (int moveNum = 0; winner == null; moveNum++)
        {

            // Determine who's turn it is.
            curPlayer = (moveNum % 2 == 0) ? Player1 : Player2;

            // string turn BUILDERMOVEBUILD string
            if (curPlayer is Player)
            {
                 yield return StartCoroutine(curPlayer.beginTurn(this));
            }
            else
            {
                yield return new WaitForSeconds(2);
            }

            // update the board with the current player's move
            Turn t = curPlayer.getNextTurn();
            Debug.Log(t.ToString());
            processTurnString(t, curPlayer, this);

            if (won == true)
                winner = curPlayer;

        }
        yield return winner;
    }

    //returns the board height at a given coordinate
    public float heightAtCoordinate(Coordinate c)
    {
        //The Scale Y * 2 of the level object
        const float gamepeiceHeight = (float)0.35;
        const float level0Height = (float)0.5001;
        const float level1Height = (float)0.5;
        const float level2Height = (float)0.4;
        const float level3Height = (float)0.3;
        const float level4Height = (float)0.000;

        float newHeightToMoveTo = 0;

        int BHeight = Board[c.x, c.y];
        switch (BHeight)
        {
            case 4:
                newHeightToMoveTo += level4Height;
                goto case 3;
            case 3:
                newHeightToMoveTo += level3Height;
                goto case 2;
            case 2:
                newHeightToMoveTo += level2Height;
                goto case 1;
            case 1:
                newHeightToMoveTo += level1Height;
                goto case 0;
            case 0:
                newHeightToMoveTo += gamepeiceHeight;
                newHeightToMoveTo += level0Height;
                break;
        }
        return newHeightToMoveTo;
    }

    private IEnumerator PlaceBuilders()
    {
        yield return StartCoroutine(Player1.PlaceBuilder(1, this));
        Player2.PlaceBuilder(1, this);
        Player2.PlaceBuilder(2, this);
        yield return StartCoroutine(Player1.PlaceBuilder(2, this));
        yield return null;
    }

    public void addAllLocationsWithoutBuildersToHighlight()
    {
        var locations = GameObject.FindGameObjectsWithTag("Square");
        var buildersLocations = getAllBuildersString();
        foreach (var l in locations)
        {
            if (!buildersLocations.Contains(l.name)) { HighlightManager.highlightedObjects.Add(l); }
        }
    }

    //set board back to 0
    private void ClearBoard()
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
    public void BuildLevel(Coordinate c)
    {
        Board[c.x, c.y] += 1;
        GameObject level = GameObject.Find(Coordinate.coordToString(c));
        level.transform.GetChild(Board[c.x, c.y] - 1).gameObject.SetActive(true);
    }

    public bool isWin(Coordinate c)
    {
        return Board[c.x, c.y] == 3;
    }

    // PULLS 4 BUILDER LOCATIONS and returns string ie. A2B3C4D3
    string getAllBuildersString()
    {
        return Player1.getBuilderLocations() + Player2.getBuilderLocations();
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

    public List<GameObject> getAllPossibleBuildLevels(List<string> locations)
    {
        List<GameObject> Levels = new List<GameObject>();
        foreach (string coord in locations)
        {
            Coordinate c = Coordinate.stringToCoord(coord);
            int height = Board[c.x, c.y];
            GameObject level = GameObject.Find(coord).transform.GetChild(height).gameObject;
            Levels.Add(level);
        }
        return Levels;
    }
    //takes a list of possible moves and highlights the moves

    public static void recieveLocationClick(Coordinate location)
    {
        clickLocation = location;
        HighlightManager.highlightedObjects.Clear();
    }

}