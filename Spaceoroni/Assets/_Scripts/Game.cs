using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;
using SharpNeat.Core;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using AI_SpaceRace;

public class Game : MonoBehaviour
{
    [SerializeField]
    private GameObject NewtorkingInfo;
    [SerializeField]
    private GameObject Rotator;
    [SerializeField]
    private GameObject PauseButton;
    [SerializeField]
    private GameObject DebugInfo;
    [SerializeField]
    private GameObject EndOfGameScreen;
    [SerializeField]
    private GameObject MainMenu;

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

    int[,] Board;
    public int[,] state { get
        {
            return Board;
        } }
    public IPlayer Player1;
    public IPlayer Player2;
    public IPlayer curPlayer;
    public static bool cancelTurn = false;

    public SantoriniCoevolutionExperiment _experiment { get; private set; }
    

    public static Coordinate clickLocation;
    private bool isDebug = false;
    private void Start()
    {

        if (isDebug)
        {
            GameSettings.gameType = GameSettings.GameType.Watch;
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>().moveCameraToGameBoard();
            StartGame();
        }
        else
        {
            var debugObjects = GameObject.FindGameObjectsWithTag("Debug");
            foreach (GameObject d in debugObjects) { d.SetActive(false); }
        }
    }
        string lastClick = "";
    private void Update()
    {
        if (clickLocation != null) lastClick = Coordinate.coordToString(clickLocation);
        else lastClick = "null";
        DebugInfo.GetComponent<TextMeshProUGUI>().text = HighlightManager.highlightedObjects.Count.ToString() + " " + lastClick;

    }

    public void StartGame()
    {
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>().moveCameraToGameBoard();

        Debug.Log("StartingGame");

        Board = new int[5, 5];
        ClearBoard();

        cancelTurn = false;
        Rotator.SetActive(false);
        PauseButton.SetActive(true);
        playerState = PlayerState.Playing;
        setupGameSettings();
        StartCoroutine(PlayGameToEnd());
    }
    public void ResetGame()
    {
        
        //cancel current turn
        cancelTurn = true;
        StopAllCoroutines();
        
        if(Player1 != null && Player2!= null) clearPlayersTurnsAndSendBuildersHome();

        HighlightManager.unHighlightEverything();
        HighlightManager.highlightedObjects.Clear();

        //reset game variables
        Player1 = null;
        Player2 = null;
        curPlayer = null;
        clickLocation = null;
        
    }

    public void RestartGame()
    {
        cancelTurn = true;
        StopAllCoroutines();
        HighlightManager.unHighlightEverything();
        HighlightManager.highlightedObjects.Clear();
        if (Player1 != null && Player2 != null) clearPlayersTurnsAndSendBuildersHome();
        curPlayer = null;
        clickLocation = null;

        StartGame();
    }

    private void clearPlayersTurnsAndSendBuildersHome()
    {
        Player1.resetPlayer();
        Player2.resetPlayer();
    }
    public void QuitGame()
    {
        if(GameSettings.gameType == GameSettings.GameType.Multiplayer)
        {
            NetworkingManager.LeaveRoom();
        }

        SettingChanger.resetGameSettings();
        ResetGame();
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>().moveCameraToStart();
        Rotator.SetActive(true);
        if(!MainMenu.activeSelf)
        {
            MainMenu.SetActive(true);
        }

    }

    //reads the settings that will be set by the UI
    private void setupGameSettings()
    {
        switch (GameSettings.gameType)
        {
            case GameSettings.GameType.NotSet:
                Player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<Player>();
                Player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<Player>();
                break;
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
                Player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<NeatPlayer>();
                Player2.loadNEATPlayer("coevolution_champion.xml");
                break;
            case GameSettings.GameType.Multiplayer:
                setupMultiplayerSettings();
                break;
        }
    }

    private void setupMultiplayerSettings()
    {
        switch (GameSettings.netMode)
        {
            case GameSettings.NetworkMode.Host:
                Player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<Player>();
                Player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<OtherPlayer>();
                break;
            case GameSettings.NetworkMode.Join:
                Player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<OtherPlayer>();
                Player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<Player>();
                break;
        }


        Debug.Log("Player1: " + Player1 + ", Player2: " + Player2);
    }
    
    public IEnumerator processTurnString(Turn turn, IPlayer curPlayer, Game g)
    {
        //players already move the builders
        if (!(curPlayer is Player)) curPlayer.moveBuidler(curPlayer.getBuilderInt(turn.BuilderLocation), turn.MoveLocation, g);

        if(GameSettings.gameType == GameSettings.GameType.Watch) yield return new WaitForSeconds(timeToTurn/2);

        // If not over Where to build?
        if (!turn.isWin)
        {
            BuildLevel(turn.BuildLocation);
        }


        yield return true;
    }

    private void BlastOffRocket(Coordinate c)
    {
        Debug.Log("Blasting Off Rocket at " + Coordinate.coordToString(c));
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
                if(curPlayer is StringPlayer)
                {
                    yield return new WaitForSeconds(timeToTurn);
                }
                else
                {
                    yield return StartCoroutine(curPlayer.beginTurn(this));

                }
                
                Turn t = curPlayer.getNextTurn();
                // update the board with the current player's move
                Debug.Log("Processing Turn: " + t.ToString());
                StartCoroutine(processTurnString(t, curPlayer, this));

                if (t.isWin)
                {
                    BlastOffRocket(t.MoveLocation);
                    winner = curPlayer;
                }

                //Check if win
                if(playerState == PlayerState.Winner)
                        winner = curPlayer;
            }

        }
        goToEndOfGameScreen();
        yield return winner;
    }

    public void goToEndOfGameScreen()
    {
        PauseButton.SetActive(false);
        EndOfGameScreen.SetActive(true);
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>().moveCameraToPostGame();
    }

    //returns the board height at a given coordinate
    public float heightAtCoordinate(Coordinate c)
    {
        //The Scale Y * 2 of the level object
        const float gamepeiceHeight = (float)0;
        const float level0Height = (float)0.500;
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
        yield return StartCoroutine(Player1.PlaceBuilder(1, 1, this));
        yield return StartCoroutine(Player2.PlaceBuilder(1, 2, this));
        yield return StartCoroutine(Player2.PlaceBuilder(2, 2, this));
        yield return StartCoroutine(Player1.PlaceBuilder(2, 1, this));
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
            if (height == 3)
            {
                //add full rocket, the highlight manager gets the individual levels
                Levels.Add(GameObject.Find(coord).transform.GetChild(0).gameObject);
            }
            else
            {
                Levels.Add(GameObject.Find(coord).transform.GetChild(0).GetChild(height).gameObject);
            }
        }
        return Levels;
    }
    //takes a list of possible moves and highlights the moves

    public static void recieveLocationClick(Coordinate location)
    {
        clickLocation = location;
        HighlightManager.highlightedObjects.Clear();
    }

    public void pauseGame() { HighlightManager.pauseGameHighlights(); }
    public void resumeGame() { HighlightManager.resumeGameHighlights(); }

}