using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public enum PlayerState
    {
        Winner,
        Loser,
        Playing
    };
    public PlayerState playerState { get; set; }
    public float level1Height = 0.8f;
    public float level2Height = 0.5f;
    public float level3Height = 0.3f;
    public int timeToTurn = 2;

    public bool watchGame = false;
    string[] lines = System.IO.File.ReadAllLines(@"C:\Users\Public\TestFolder\WriteLines2.txt");
    static string startLocaitons = "A2A1C2B0";
    int[,] Board;
    IPlayer Player1;
    //Testing Player Player2;
    IPlayer Player2;

    public static Coordinate clickLocation;

    private void Start()
    {
        GameSettings.gameType = GameSettings.GameType.Watch;


        Board = new int[5, 5];
        playerState = PlayerState.Playing;
        setupGameSettings();
        ClearBoard();
        StartCoroutine(PlayGameToEnd());
    }

    //reads the settings that will be set by the UI
    private void setupGameSettings()
    {
        switch (GameSettings.gameType)
        {
            case GameSettings.GameType.Watch:
                Player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<StringPlayer>();
                Player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<StringPlayer>();
                break;
            case GameSettings.GameType.Tutorial:
                Player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<Player>();
                Player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<StringPlayer>();
                break;
            case GameSettings.GameType.Singleplayer:
                Player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<Player>();
                Player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<StringPlayer>();
                break;
            case GameSettings.GameType.Multiplayer:
                Player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<Player>();
                Player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<Player>();
                break;
        }
    }

    public static string getStartLocations(int i)
    {
        if (i == 1) return startLocaitons.Substring(0, 4);
        return startLocaitons.Substring(4, 4);
    }

    //will get called by the main menu
    public void startGame()
    {

    }

    public IEnumerator processTurnString(Turn turn, IPlayer curPlayer, Game g)
    {
        //players already move the builders
        curPlayer.moveBuidler(curPlayer.getBuilderInt(turn.BuilderLocation), turn.MoveLocation, g);

        if(watchGame) yield return new WaitForSeconds(1);
        // If not over Where to build?
        if (!turn.isWin)
        {
            BuildLevel(turn.BuildLocation);
        }

        // build should be completed?
        yield return true;
    }

    private void BlastOffRocket(Coordinate c)
    {
        var location = GameObject.Find(Coordinate.coordToString(c)).GetComponent<Location>();
        location.blastOffRocket();
    }

    public IEnumerator PlayGameToEnd()
    {
        IPlayer curPlayer;
        IPlayer winner = null;

        yield return null;
        yield return StartCoroutine(PlaceBuilders()); 

        // Play until we have a winner or tie?
        for (int moveNum = 0; winner == null; moveNum++)
        {

            // Determine who's turn it is.
            curPlayer = (moveNum % 2 == 0) ? Player1 : Player2;

            //Check if last turn lost
            if (playerState == PlayerState.Loser)
            {
                winner = curPlayer;
            }
            else
            {
                // string turn BUILDERMOVEBUILD string
                if (curPlayer is Player)
                {
                    yield return StartCoroutine(curPlayer.beginTurn(this));
                }
                else
                {
                    yield return new WaitForSeconds(timeToTurn);
                }
                // update the board with the current player's move
                Turn t = curPlayer.getNextTurn();
                Debug.Log(t.ToString()); //BUG
                StartCoroutine(processTurnString(t, curPlayer, this));
                //Testing yield return StartCoroutine(curPlayer.beginTurn(this));

                //Check if win
                if(playerState == PlayerState.Winner)
                        winner = curPlayer;
            }

        }
        yield return winner;
    }

    //returns the board height at a given coordinate
    public float heightAtCoordinate(Coordinate c)
    {
        //The Scale Y * 2 of the level object
        const float gamepeiceHeight = (float)0.35;
        const float level0Height = (float)0.5001;
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
        Debug.Log(Player1);
        if (Player1 is Player) yield return StartCoroutine(Player1.PlaceBuilder(1, this));
        else if (Player2 is StringPlayer) Player1.PlaceBuilder(1, this);
        //Testing yield return StartCoroutine(Player2.PlaceBuilder(1, this));
        //Testing yield return StartCoroutine(Player2.PlaceBuilder(2, this));
        Player2.PlaceBuilder(1, this);
        Player2.PlaceBuilder(2, this);
        if (Player1 is Player) yield return StartCoroutine(Player1.PlaceBuilder(2, this));
        else if (Player2 is StringPlayer) Player1.PlaceBuilder(2, this);
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
        if(Board[c.x,c.y] < 4) level.transform.GetChild(0).GetChild(Board[c.x, c.y] - 1).gameObject.SetActive(true);
        else { BlastOffRocket(c); }
    }

    public bool isWin(Coordinate c)
    {
        return Board[c.x, c.y] == 3;
    }

    public bool canMove(Coordinate c)
    {
        int possibleMoves = getAllPossibleMoves(c).Count;

        return (possibleMoves > 0);
    }

    public bool canBuild(Coordinate c)
    {
        int possibleBuilds = getAllPossibleBuilds(c).Count;

        return (possibleBuilds > 0);
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
            GameObject level = GameObject.Find(coord).transform.GetChild(0).GetChild(height).gameObject;
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