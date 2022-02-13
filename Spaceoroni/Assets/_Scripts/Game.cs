using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class Game : MonoBehaviour
{
    public const byte RAISE_TURN = 1;

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

    List<Turn> turns = new List<Turn>();
    int[,] Board;
    IPlayer Player1;

    IPlayer Player2;

    public static Coordinate clickLocation;

    private void Start()
    {
        GameSettings.gameType = GameSettings.GameType.Multiplayer;

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
    
    //will get called by the main menu
    public void startGame()
    {

    }

    public IEnumerator processTurnString(Turn turn, IPlayer curPlayer, Game g)
    {
        //players already move the builders
        curPlayer.moveBuidler(curPlayer.getBuilderInt(turn.BuilderLocation), turn.MoveLocation, g);

        if(GameSettings.gameType == GameSettings.GameType.Watch) yield return new WaitForSeconds(1);

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
                PhotonView photonView = PhotonView.Get(this);
                if (curPlayer is Player && photonView.IsMine)
                {
                    yield return StartCoroutine(curPlayer.beginTurn(this));
                    Turn t = curPlayer.getNextTurn();
                    if (GameSettings.gameType == GameSettings.GameType.Multiplayer) RaiseTurnSelected(t);
                }
                else
                {
                    //this will recieve a turn from a event then add it to the end of the list
                    //RecieveTurn();
                    yield return new WaitForSeconds(timeToTurn);
                }
                // update the board with the current player's move
                Debug.Log(turns[turns.Count - 1].ToString()); //BUG
                StartCoroutine(processTurnString(turns[turns.Count - 1], curPlayer, this));
                //Testing yield return StartCoroutine(curPlayer.beginTurn(this));

                //Check if win
                if(playerState == PlayerState.Winner)
                        winner = curPlayer;
            }

        }
        yield return winner;
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == RAISE_TURN)
        {
            object[] data = (object[])photonEvent.CustomData;

            Turn turn = (Turn)data[0];

            turns.Add(turn);
        }
    }

    private void RaiseTurnSelected(Turn t)
    {
        object[] datas = new object[] { t };
        PhotonNetwork.RaiseEvent(RAISE_TURN, datas, RaiseEventOptions.Default, SendOptions.SendReliable);
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
        //Player 1 Builder 1
        if (Player1 is Player) yield return StartCoroutine(Player1.PlaceBuilder(1,1, this));
        else if (Player1 is StringPlayer) Player1.PlaceBuilder(1,1, this);


        //Player 2 Builder 1
        if (Player2 is Player) yield return StartCoroutine(Player2.PlaceBuilder(1,2, this));
        else if (Player2 is StringPlayer) Player2.PlaceBuilder(1,2, this);

        //Player 2 Builder 2
        if (Player2 is Player) yield return StartCoroutine(Player2.PlaceBuilder(2,2, this));
        else if (Player2 is StringPlayer) Player2.PlaceBuilder(2,2, this);

        //Player 1 Builder 2
        if (Player1 is Player) yield return StartCoroutine(Player1.PlaceBuilder(2,1, this));
        else if (Player2 is StringPlayer) Player1.PlaceBuilder(2,1, this);


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