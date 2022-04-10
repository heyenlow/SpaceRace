using System;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField]
    private GameObject JoinMenu;
    [SerializeField]
    private GameObject ThingsToRemember;
    [SerializeField]
    private TextMeshProUGUI TurnIndicator;
    [SerializeField]
    private AudioSource panNoise;
    [SerializeField]
    private AudioSource Build;
    [SerializeField]
    private CountDown countDown;

    [SerializeField]
    private TextMeshProUGUI WinText;
    Color blueColor; 


    public enum PlayerState
    {
        Winner,
        Loser,
        Playing
    };
    public PlayerState playerState { get; set; }
    public float boardScale = 4f;
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
    public IPlayer curPlayer, rival;
    public static bool cancelTurn = false;
    public static bool PAUSED = false;
    public static bool PLAYERinTURN = false;



    public SantoriniCoevolutionExperiment _experiment { get; private set; }
    

    public static Coordinate clickLocation;
    private bool isDebug = false;
    private void Start()
    {
        blueColor = TurnIndicator.color;
        if (isDebug)
        {
            GameSettings.gameType = GameSettings.GameType.Watch;
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineCamSwitcher>().MoveToGameBoard();
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
        panNoise.Play();
        PAUSED = false;
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineCamSwitcher>().MoveToGameBoard();
        TutorailRestart = false;

        Board = new int[5, 5];
        ClearBoard();

        cancelTurn = false;
        Rotator.SetActive(false);
        PauseButton.SetActive(true);
        playerState = PlayerState.Playing;
        setupGameSettings();
        Debug.Log("Starting " + GameSettings.gameType +  " Game" + Player1 + Player2);
        StartCoroutine(PlayGameToEnd());
    }
    public void ResetGame()
    {
        
        //cancel current turn
        cancelTurn = true;
        StopAllCoroutines();
        TurnIndicator.text = "";


        //needs to reset the reader to the first move
        StringGameReader.MoveCount = 0;
        resetAllLocationsAlive();

        if (Player1 != null && Player2!= null) clearPlayersTurnsAndSendBuildersHome();

        HighlightManager.unHighlightEverything();
        HighlightManager.highlightedObjects.Clear();

        //reset game variables
        Player1 = null;
        Player2 = null;
        curPlayer = null;
        rival = null;
        clickLocation = null;
        
    }

    public bool restartAfterMultiplayer = false;
    public void RestartGame()
    {
        if (GameSettings.gameType == GameSettings.GameType.Multiplayer && GameSettings.netMode != GameSettings.NetworkMode.Local && !restartAfterMultiplayer)
        {
            
        }
        else 
        {
            restartAfterMultiplayer = false;
            cancelTurn = true;
            StopAllCoroutines();
            resetAllLocationsAlive();

            //needs to reset the reader to the first move
            StringGameReader.MoveCount = 0;
            HighlightManager.unHighlightEverything();
            HighlightManager.highlightedObjects.Clear();
            if (Player1 != null && Player2 != null) clearPlayersTurnsAndSendBuildersHome();
            curPlayer = null;
            rival = null;
            clickLocation = null;

            StartGame();
        }
    }

    private void clearPlayersTurnsAndSendBuildersHome()
    {
        Player1.resetPlayer();
        Player2.resetPlayer();
    }
    public void QuitGame()
    {
        Player1.ClearTurnText();
        Player2.ClearTurnText();
        TurnIndicator.text = "";
        Location.LocationBlinking = null;
        Rocket.blinkingRocket = null;
        Builder.BlinkingBuilder = null;
        Level.BlinkingLevel = null;

        if(GameSettings.gameType == GameSettings.GameType.Multiplayer && GameSettings.netMode != GameSettings.NetworkMode.Local)
        {
            NetworkingManager.LeaveRoom();
        }
        Player1.GetComponent<TutorialPlayer>().setAllOverlaysInactive();
        bringUpAFewThingsScreen();
        //if tutorial we wait for the okay to be clicked on the few things to rememeber
        if (!ThingsToRemeberActive) { GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineCamSwitcher>().MoveToMainMenu(); }

        SettingChanger.resetGameSettings();
        ResetGame();
        EndOfGameScreen.SetActive(false);
        PauseButton.SetActive(false);
        JoinMenu.SetActive(false);
        Rotator.SetActive(true);
        MainMenu.SetActive(true);
    }

    bool TutorailRestart = false;
    bool ThingsToRemeberActive = false;
    public void bringUpAFewThingsScreen()
    {
        if (GameSettings.gameType == GameSettings.GameType.Tutorial && !PAUSED)
        {
            ThingsToRemeberActive = true;
            ThingsToRemember.SetActive(true);
        }
    }
    public void restartTuotialBoolTrue() { TutorailRestart = true; }
    public void manageAfewThingsToRemeberOkayClick()
    {
        ThingsToRemember.SetActive(false);
        ThingsToRemeberActive = false;
        if (TutorailRestart)
        {
            RestartGame();
        }
        else
        {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineCamSwitcher>().MoveToMainMenu();
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
                //Player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<NeatPlayer>();
                //Player1.loadNEATPlayer("coevolution_champion");
                //Player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<NeatPlayer>();
                //Player2.loadNEATPlayer("coevolution_champion");
                Player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<StringPlayer>();
                Player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<StringPlayer>();
                StringGameReader.setGameLines();
                break;
            case GameSettings.GameType.Tutorial:
                Player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<TutorialPlayer>();
                Player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<StringPlayer>();
                StringGameReader.setGameLines();
                break;
            case GameSettings.GameType.Singleplayer:
                Player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<Player>();
                Player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<NeatPlayer>();
                Player2.loadNEATPlayer("coevolution_champion");
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
            case GameSettings.NetworkMode.Local:
                Player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<Player>();
                Player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<Player>();
                break;
        }


        Debug.Log("Player1: " + Player1 + ", Player2: " + Player2);
    }

    public void processTurnString(Turn turn, IPlayer curPlayer, Game g)
    {
        //players already move the builders and so does neatplayer
        if (!(curPlayer is Player) && !(curPlayer is NeatPlayer)) curPlayer.moveBuidler(curPlayer.getBuilderInt(turn.BuilderLocation), turn.MoveLocation, g);


        // if (!(curPlayer is Player)) yield return new WaitForSeconds(timeToTurn/2);
        if (curPlayer is Player)
        {
            // If not over Where to build?
            if (!turn.isWin)
            {
                BuildLevel(turn.BuildLocation);
            }
        }
        else
        { 
            StartCoroutine(WaitBuildLevel(turn.BuildLocation));
        }
    }

    private IEnumerator WaitBuildLevel(Coordinate c)
    {
        yield return new WaitForSeconds(1.3f);
        BuildLevel(c);
    }

    private void runEndOfGameAnimation(Coordinate c, bool elon)
    {
        Debug.Log("Blasting Off Rocket at " + Coordinate.coordToString(c));
        var location = GameObject.Find(Coordinate.coordToString(c)).GetComponent<Location>();
        location.runEndOfGameAnimation(elon);
    }

    private void BlastOffRocket(Coordinate c)
    {
        Debug.Log("Blasting Off Rocket at " + Coordinate.coordToString(c));
        var location = GameObject.Find(Coordinate.coordToString(c)).GetComponent<Location>();
        location.blastOffRocket();
    }

    public static bool built = true;
    public static bool countDownActive = false;

    public IEnumerator PlayGameToEnd()
    {
        IPlayer curPlayer;
        IPlayer othPlayer;
        IPlayer winner = null;

        yield return null;
        yield return StartCoroutine(PlaceBuilders());
        built = true;

        // Play until we have a winner or tie?
        for (int moveNum = 0; winner == null; moveNum++)
        {

            yield return StartCoroutine(waitForBuildersToMove());
            yield return StartCoroutine(waitForBuildersToBuild());
            // Determine who's turn it is.
            curPlayer = (moveNum % 2 == 0) ? Player1 : Player2;
            othPlayer = (moveNum % 2 == 0) ? Player2 : Player1;

            if (GameSettings.netMode == GameSettings.NetworkMode.Local) { setTurnIndicator(curPlayer); }

                //Check if last turn lost
                if (playerState == PlayerState.Loser)
            {
                winner = curPlayer;
            }
            else
            {
                if (hasPossibleTurns(curPlayer))
                {
                    // string turn BUILDERMOVEBUILD string
                    PhotonView photonView = PhotonView.Get(this);

                    //if it is a string player we just need to wait before getting the turn and processing it
                    if (curPlayer is StringPlayer)
                    {
                        yield return new WaitForSeconds(timeToTurn);
                    }
                    else //if it is any other player we need to start getting the turn
                    {
                        yield return StartCoroutine(curPlayer.beginTurn(this));
                    }

                    Turn t = curPlayer.getNextTurn();
                    // update the board with the current player's move
                    Debug.Log("Processing Turn: " + t.ToString());
    
                    while (PAUSED) { yield return new WaitForEndOfFrame(); }

                    if (t.isWin)
                    {
                        countDown.gameObject.SetActive(true);
                        countDownActive = true;
                        TurnIndicator.text = "";
                        StartCoroutine(countDown.startCountdown());


                        built = true;
                        winner = curPlayer;
                        //wait to move then set buidler inactive
                        curPlayer.setBuilderAtLocationInactive(t.BuilderLocation);
                        runEndOfGameAnimation(t.MoveLocation, winner == Player1);

                        while (countDownActive)
                        {
                            yield return new WaitForEndOfFrame();
                        }
                        if (!countDownActive)
                        {
                            yield return new WaitForSeconds(0.4f);
                            BlastOffRocket(t.MoveLocation);
                            countDown.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        built = false;
                        processTurnString(t, curPlayer, this);
                    }
                }
                else { winner = othPlayer; }

                if (winner != null){
                    if (GameSettings.gameType == GameSettings.GameType.Multiplayer)
                    {
                        if (winner == Player1 && GameSettings.netMode == GameSettings.NetworkMode.Host) { WinText.text = "You Win!"; }
                        else if (winner == Player2 && GameSettings.netMode == GameSettings.NetworkMode.Host) { WinText.text = "Better Luck Next Time"; }
                        else if (winner == Player2 && GameSettings.netMode == GameSettings.NetworkMode.Join) { WinText.text = "You Win!"; }
                        else if (winner == Player1 && GameSettings.netMode == GameSettings.NetworkMode.Join) { WinText.text = "Better Luck Next Time"; }
                    }
                    else
                    {
                        if (winner == Player1) { WinText.text = "You Win!"; }
                        else { WinText.text = "Better Luck Next Time"; }
                    }
                    yield return new WaitForSeconds(1.5f);
                    goToEndOfGameScreen();
                }

                //Check if win
                if (playerState == PlayerState.Winner)
                {
                    winner = curPlayer;
                }
            }
        }
        
        yield return winner;
    }

    public void PAUSEGAME() { PAUSED = true; HighlightManager.pauseGameHighlights(); }
    public void RESUMEGAME() { PAUSED = false; HighlightManager.resumeGameHighlights(); }


    private void setTurnIndicator(IPlayer curPlayer)
    {
        if (GameSettings.netMode == GameSettings.NetworkMode.Local)
        {
            if (curPlayer == Player1) { TurnIndicator.text = "Player 1's Turn"; TurnIndicator.color = Color.white; }
            else if (curPlayer == Player2) { TurnIndicator.text = "Player 2's Turn"; TurnIndicator.color = blueColor; }
        }
    }
    private void setTurnIndicator(int PlayerInt)
    {
        if (GameSettings.netMode == GameSettings.NetworkMode.Local)
        {
            if (PlayerInt == 1) { TurnIndicator.text = "Player 1's Turn"; TurnIndicator.color = Color.white; }
            else if (PlayerInt == 2) { TurnIndicator.text = "Player 2's Turn"; TurnIndicator.color = blueColor; }
        }
    }
    private bool hasPossibleTurns(IPlayer p)
    {
        Coordinate BuilderOneLocation = Coordinate.stringToCoord(p.getBuilderLocations().Substring(0,2));
        Coordinate BuilderTwoLocation = Coordinate.stringToCoord(p.getBuilderLocations().Substring(2, 2));

        var builderOneMoves = getAllPossibleMoves(BuilderOneLocation);
        foreach (var coord in builderOneMoves) {
            if(getAllPossibleBuilds(Coordinate.stringToCoord(coord)).Count > 0) return true;
        }

        var builderTwoMoves = getAllPossibleMoves(BuilderTwoLocation);
        foreach (var coord in builderTwoMoves)
        {
            if (getAllPossibleBuilds(Coordinate.stringToCoord(coord)).Count > 0) return true;
        }

        return false;
    }

    private IEnumerator waitForBuildersToMove()
    {
        while (Player1.BuildersAreMoving() || Player2.BuildersAreMoving())
        {
            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator waitForBuildersToBuild()
    {
        while (!built)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    public void goToEndOfGameScreen()
    {
        PauseButton.SetActive(false);
        EndOfGameScreen.SetActive(true);
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineCamSwitcher>().MoveToEnd();
    }

    //returns the board height at a given coordinate
    public float heightAtCoordinate(Coordinate c)
    {
        //The Scale Y * 2 of the level object
        const float gamepeiceHeight = (float)0;
        const float level0Height = (float)0.5;
        const float level4Height = (float)0.000;

        float newHeightToMoveTo = 0;

        int BHeight = Board[c.x, c.y];
        switch (BHeight)
        {
            case 4:
                newHeightToMoveTo += level4Height * boardScale;
                goto case 3;
            case 3:
                newHeightToMoveTo += level3Height * boardScale;
                goto case 2;
            case 2:
                newHeightToMoveTo += level2Height * boardScale;
                goto case 1;
            case 1:
                newHeightToMoveTo += level1Height * boardScale;
                goto case 0;
            case 0:
                newHeightToMoveTo += gamepeiceHeight * boardScale;
                newHeightToMoveTo += level0Height * boardScale;
                break;
        }
        return newHeightToMoveTo;
    }

    public int levelAtCoord(Coordinate c)
    {
        return Board[c.x, c.y];
    }

    private IEnumerator PlaceBuilders()
    {
        setTurnIndicator(1);
        yield return StartCoroutine(waitForBuildersToMove());
        yield return StartCoroutine(Player1.PlaceBuilder(1, 1, this));
        yield return StartCoroutine(waitForBuildersToMove());
        yield return StartCoroutine(Player1.PlaceBuilder(2, 1, this));
        yield return StartCoroutine(waitForBuildersToMove());
        setTurnIndicator(2);
        yield return StartCoroutine(Player2.PlaceBuilder(1, 2, this));
        yield return StartCoroutine(Player2.PlaceBuilder(2, 2, this));
        yield return StartCoroutine(waitForBuildersToMove());
        yield return null;
    }

    /// <summary>
    /// Resets the bool DeadLocation to false when a game is restarted.
    /// </summary>
    public void resetAllLocationsAlive()
    {
        var locations = GameObject.FindGameObjectsWithTag("Square");
        foreach (var l in locations)
        {
            l.GetComponent<Location>().setLocationAlive();
        }
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
    public void blinkAllLocationsWithoutBuildersToHighlight()
    {
        var locations = GameObject.FindGameObjectsWithTag("Square");
        var buildersLocations = getAllBuildersString();
        foreach (var l in locations)
        {
            if (!buildersLocations.Contains(l.name)) { l.GetComponent<Location>().Blink(); }
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

        if (isAtThree()) { GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineCamSwitcher>().moveToHigh(); }
        else { GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineCamSwitcher>().moveToLow(); }

        if (Board[c.x, c.y] < 4) level.GetComponent<Location>().buildLevel(Board[c.x, c.y]); // level.transform.GetChild(0).GetChild(Board[c.x, c.y] - 1).gameObject.SetActive(true);
        else { BlastOffRocket(c); built = true; }
        Build.Play();
    }
    public int getBoardHeightAtCoord(Coordinate c)
    {
        return Board[c.x, c.y];
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

    private bool isAtThree()
    {
        for(int i = 0; i < 5; i++)
        {
            for(int j = 0; j < 5; j++)
            {
                //(5 - i) + j < 5 && 
                if (Board[i, j] == 3) return true;
            }
        }
        return false;
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
                if (Coordinate.inBounds(test) && Board[test.x, test.y] < 4 && locationClearOfAllBuilders(test) && !Coordinate.Equals(test, c))
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
}