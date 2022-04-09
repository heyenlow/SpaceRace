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
using Assets._Scripts.MCTS;

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

    public static long[,,] ZobristTable = null;

    [SerializeField]
    private TextMeshProUGUI WinText;
    public bool simulationRunning;

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
    public long Hash => computeHash();
    public int timeToTurn = 2;

    int[,] Board;
    public int[,] state { 
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

    public SantoriniCoevolutionExperiment _experiment { get; private set; }
    
    public Game(Game g)
    {
        Board = new int[5, 5];
        System.Array.Copy(g.state, g.state.GetLowerBound(0), Board, Board.GetLowerBound(0), 25);
        Player1 = new MCTSPlayer(g.Player1);
        Player2 = new MCTSPlayer(g.Player2);

    }


    public static Coordinate clickLocation;
    private bool isDebug = false;
    private void Start()
    {

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
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineCamSwitcher>().MoveToGameBoard();

        initTable();
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
        
        //needs to reset the reader to the first move
        StringGameReader.MoveCount = 0;
        resetAllLocationsAlive();

        if (Player1 != null && Player2!= null) clearPlayersTurnsAndSendBuildersHome();

        HighlightManager.unHighlightEverything();
        HighlightManager.highlightedObjects.Clear();

        //reset game variables
        Player1 = null;
        Player2 = null;
        clickLocation = null;
        
    }

    public void RestartGame()
    {
        cancelTurn = true;
        StopAllCoroutines();
        resetAllLocationsAlive();

        //needs to reset the reader to the first move
        StringGameReader.MoveCount = 0;
        HighlightManager.unHighlightEverything();
        HighlightManager.highlightedObjects.Clear();
        if (Player1 != null && Player2 != null) clearPlayersTurnsAndSendBuildersHome();
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
        Player1.ClearTurnText();
        Player2.ClearTurnText();
        Location.LocationBlinking = null;
        Rocket.blinkingRocket = null;
        Builder.BlinkingBuilder = null;
        Level.BlinkingLevel = null;

        if(GameSettings.gameType == GameSettings.GameType.Multiplayer)
        {
            NetworkingManager.LeaveRoom();
        }
        Player1.GetComponent<TutorialPlayer>().setAllOverlaysInactive();

        SettingChanger.resetGameSettings();
        ResetGame();
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineCamSwitcher>().MoveToStart();
        EndOfGameScreen.SetActive(false);
        PauseButton.SetActive(false);
        JoinMenu.SetActive(false);
        Rotator.SetActive(true);
        MainMenu.SetActive(true);

    }

    private void setAIDifficulty()
    {
        switch(GameSettings.difficulty)
        {
            case GameSettings.AIDifficulty.Easy:
                Player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<Player>();
                Player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<NeatPlayer>();
                Player2.loadNEATPlayer("coevolution_champion");
                break;
            case GameSettings.AIDifficulty.Hard:
            case GameSettings.AIDifficulty.Med:

                Player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<Player>();
                Player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<MCTSPlayer>();
                break;

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
                setAIDifficulty();
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

    public void processTurnString(Turn turn, IPlayer curPlayer, Game g)
    {
        //players already move the builders and so does neatplayer
        if (!(curPlayer is Player) && !(curPlayer is NeatPlayer))
        {
            curPlayer.moveBuilder(curPlayer.getBuilderInt(turn.BuilderLocation), turn.MoveLocation, g);
        }


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

    private void BlastOffRocket(Coordinate c)
    {
        Debug.Log("Blasting Off Rocket at " + Coordinate.coordToString(c));
        var location = GameObject.Find(Coordinate.coordToString(c)).GetComponent<Location>();
        location.blastOffRocket();
    }

    bool built = true;

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
                        
                        built = false;
                        processTurnString(t, curPlayer, this);
                        
                        if (t.isWin)
                        {
                            built = true;
                            winner = curPlayer;
                            //wait to move then set buidler inactive
                            yield return new WaitForSeconds(0.6f);
                            curPlayer.setBuilderAtLocationInactive(t.MoveLocation);
                            
                            yield return new WaitForSeconds(2);
                            BlastOffRocket(t.MoveLocation);
                        }
                    }
                    else { winner = othPlayer; }

                if(winner != null){
                    if (winner == Player1) { WinText.text = "You Win!"; }
                    else { WinText.text = "Better Luck Next Time"; }
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
        while (Player1.BuildersAreMoving() && Player2.BuildersAreMoving())
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

    private IEnumerator PlaceBuilders()
    {
        yield return StartCoroutine(waitForBuildersToMove());
        yield return StartCoroutine(Player1.PlaceBuilder(1, 1, this));
        yield return StartCoroutine(waitForBuildersToMove());
        yield return StartCoroutine(Player1.PlaceBuilder(2, 1, this));
        yield return StartCoroutine(waitForBuildersToMove());
        moveNum++;
        yield return StartCoroutine(Player2.PlaceBuilder(1, 2, this));
        yield return StartCoroutine(waitForBuildersToMove());
        yield return StartCoroutine(Player2.PlaceBuilder(2, 2, this));
        yield return StartCoroutine(waitForBuildersToMove());
        moveNum--;
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
        if(Board[c.x,c.y] < 4) level.transform.GetChild(0).GetChild(Board[c.x, c.y] - 1).gameObject.SetActive(true);
        else { BlastOffRocket(c); }
        built = true;
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

    public void pauseGame() { HighlightManager.pauseGameHighlights(); }
    public void resumeGame() { HighlightManager.resumeGameHighlights(); }

    // ===================================================================================

    public long computeHash()
    {
        if (ZobristTable == null) initTable();
        char[,] board = TranslateState();
        long h = 0;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (board[i, j] != '-')
                {
                    int piece = indexOf(board[i, j]);
                    h ^= ZobristTable[i, j, piece];
                }
            }
        }
        return h;
    }

    public char[,] TranslateState()
    {
        char[,] ret = new char[5, 5];

        // takes the Game g.state int matrix and converts it to a char matrix with representations for builders on board
        Coordinate tmp = new Coordinate();
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                tmp.x = i; tmp.y = j;
                if (locationClearOfAllBuilders(tmp))
                {
                    ret[i, j] = (char)(state[i, j] + 48);
                }
                else
                {
                    char piece = '0';
                    switch (state[i, j])
                    {
                        case 0:
                            if (Equals(tmp, Coordinate.stringToCoord(Player1.getBuilderLocations().Substring(0, 2)))) piece = 'A';
                            else if (Equals(tmp, Coordinate.stringToCoord(Player1.getBuilderLocations().Substring(2, 2)))) piece = 'E';
                            else if (Equals(tmp, Coordinate.stringToCoord(Player2.getBuilderLocations().Substring(0, 2)))) piece = 'a';
                            else if (Equals(tmp, Coordinate.stringToCoord(Player2.getBuilderLocations().Substring(2, 2)))) piece = 'e';
                            break;
                        case 1:
                            if (Equals(tmp, Coordinate.stringToCoord(Player1.getBuilderLocations().Substring(0, 2)))) piece = 'B';
                            else if (Equals(tmp, Coordinate.stringToCoord(Player1.getBuilderLocations().Substring(2, 2)))) piece = 'F';
                            else if (Equals(tmp, Coordinate.stringToCoord(Player2.getBuilderLocations().Substring(0, 2)))) piece = 'b';
                            else if (Equals(tmp, Coordinate.stringToCoord(Player2.getBuilderLocations().Substring(2, 2)))) piece = 'f';
                            break;
                        case 2:
                            if (Equals(tmp, Coordinate.stringToCoord(Player1.getBuilderLocations().Substring(0, 2)))) piece = 'C';
                            else if (Equals(tmp, Coordinate.stringToCoord(Player1.getBuilderLocations().Substring(2, 2)))) piece = 'G';
                            else if (Equals(tmp, Coordinate.stringToCoord(Player2.getBuilderLocations().Substring(0, 2)))) piece = 'c';
                            else if (Equals(tmp, Coordinate.stringToCoord(Player2.getBuilderLocations().Substring(2, 2)))) piece = 'g';
                            break;
                        case 3:
                            if (Equals(tmp, Coordinate.stringToCoord(Player1.getBuilderLocations().Substring(0, 2)))) piece = 'D';
                            else if (Equals(tmp, Coordinate.stringToCoord(Player1.getBuilderLocations().Substring(2, 2)))) piece = 'H';
                            else if (Equals(tmp, Coordinate.stringToCoord(Player2.getBuilderLocations().Substring(0, 2)))) piece = 'd';
                            else if (Equals(tmp, Coordinate.stringToCoord(Player2.getBuilderLocations().Substring(2, 2)))) piece = 'h';
                            break;
                    }
                    ret[i, j] = piece;
                }
            }

        }

        return ret;
    }

    public Game DeepCopy()
    {
        Game copy = new Game(this);

        return copy;
    }

    void initTable()
    {
        if (ZobristTable != null) return;
        ZobristTable = new long[5,5,21];
        System.Random rnd = new System.Random(System.Guid.NewGuid().GetHashCode());
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                for (int k = 0; k < 21; k++)
                {
                    byte[] bytes = new byte[8];
                    rnd.NextBytes(bytes);
                    long uint64 = System.BitConverter.ToInt64(bytes, 0);
                    ZobristTable[i, j, k] = uint64;
                    rnd.Next();
                }

            }

        }
    }

    static int indexOf(char piece)
    {// this node's character array `state` uses letters to represent builders at different heights
     // convert these letters numbers we can use for easy height calculations and such
        switch (piece)
        {
            case 'A':               // player 1 builder 1 height 0
                return 0;
            case 'B':
                return 1;
            case 'C':
                return 2;
            case 'D':               // player 1 builder 1 height 3
                return 3;
            case 'E':               // player 1 builder 2 height 0
                return 4;
            case 'F':
                return 5;
            case 'G':
                return 6;
            case 'H':               // player 1 builder 2 height 3
                return 7;
            case 'a':               // player 2 builder 1 height 0
                return 8;
            case 'b':
                return 9;
            case 'c':
                return 10;
            case 'd':               // player 2 builder 1 height 3
                return 11;
            case 'e':               // player 2 builder 2 height 0
                return 12;
            case 'f':
                return 13;
            case 'g':
                return 14;
            case 'h':               // player 2 builder 2 height 3
                return 15;
            case '0':               // player - height 0
                return 16;
            case '1':
                return 17;
            case '2':
                return 18;
            case '3':
                return 19;
            case '4':               // player - height 4
                return 20;
            default:
                return -1;
        }
    }

    public bool IsWinner(IPlayer player)
    {
        IPlayer winner;
        if (Player1.state == IPlayer.States.Winner) winner = Player1;
        else if (Player2.state == IPlayer.States.Winner) winner = Player2;
        else if (Player1.state == IPlayer.States.Loser) winner = Player2;
        else if (Player2.state == IPlayer.States.Loser) winner = Player1;
        else winner = null;

        if (winner != null) return true;

        return false;
    }

    public List<UCB1Tree.Transition> GetLegalTransitions()
    {
        List<UCB1Tree.Transition> ret = new List<UCB1Tree.Transition>();
        // get all possible moves from each builder including new build locations....
        // builder 1...
        List<string> moves = getAllPossibleMoves(Coordinate.stringToCoord(CurrentPlayer.getBuilderLocations().Substring(0, 2)));
        
        for (int i = 0; i < moves.Count; i++)
        {
            List<string> builds = getAllPossibleBuilds(Coordinate.stringToCoord(moves[i]));
            for (int j = 0; j < builds.Count; j++)
            {
                
                // create transition representing this possible move
                UCB1Tree.Transition tmp = new UCB1Tree.Transition(Coordinate.stringToCoord(CurrentPlayer.getBuilderLocations().Substring(0, 2)), Coordinate.stringToCoord(moves[i]), Coordinate.stringToCoord(builds[j]), Hash);
                // Hash Values are currently this current game's state hash

                // need to change it to be the hash value of a game resulting from the move taking place.  without actually making the move?
                Game tmpState = DeepCopy(); //copy current game state
                string turnString = Coordinate.coordToString(tmp.Builder) + Coordinate.coordToString(tmp.Move) + Coordinate.coordToString(tmp.Build); // create turn string from this transition
                Turn currTurn = new Turn(tmp.Builder, tmp.Move, tmp.Build);
                tmpState.processTurnString(currTurn, tmpState.CurrentPlayer, tmpState); // execute transition on copy board
                tmp.Hash = tmpState.computeHash(); // this transition hash equals the hash of the copy state's Hash
                ret.Add(tmp);
            }
        }
        // builder 2...
        moves = getAllPossibleMoves(Coordinate.stringToCoord(CurrentPlayer.getBuilderLocations().Substring(2, 2)));
        for (int i = 0; i < moves.Count; i++)
        {
            List<string> builds = getAllPossibleBuilds(Coordinate.stringToCoord(moves[i]));
            for (int j = 0; j < builds.Count; j++)
            {
                UCB1Tree.Transition tmp = new UCB1Tree.Transition(Coordinate.stringToCoord(CurrentPlayer.getBuilderLocations().Substring(2, 2)), Coordinate.stringToCoord(moves[i]), Coordinate.stringToCoord(builds[j]), Hash);
                // Hash Values are currently this current game's state hash
                // need to change it to be the hash value of a game resulting from the move taking place.  without actually making the move?
                Game tmpState = DeepCopy();
                Turn currTurn = new Turn(tmp.Builder, tmp.Move, tmp.Build);
                tmpState.processTurnString(currTurn, tmpState.CurrentPlayer, tmpState);
                tmp.Hash = tmpState.computeHash();

                ret.Add(tmp);
            }
        }
        if (ret.Count == 0)
        {
            CurrentPlayer.state = IPlayer.States.Loser; // if you can't move you're a loser.
            Rival.state = IPlayer.States.Winner;
        }
        return ret;
    }

    public void Rollout()
    {
        IPlayer winner = null;

        System.Random rnd = RandomFactory.Create();
        simulationRunning = false;
        for (; winner == null; moveNum++)
        {
            var allPossibleMoves = GetLegalTransitions();
            if (allPossibleMoves.Count == 0)
            {
                winner = Rival;
                CurrentPlayer.state = IPlayer.States.Loser;
                Rival.state = IPlayer.States.Winner;
                break;
            }
            var randomMove = allPossibleMoves[rnd.Next(0, allPossibleMoves.Count)];
            Turn currTurn = new Turn(randomMove.Builder, randomMove.Move, randomMove.Build);

            bool won = isWin(randomMove.Move);
            if (won)
            {
                CurrentPlayer.state = IPlayer.States.Winner;
                Rival.state = IPlayer.States.Loser;
                winner = CurrentPlayer;
                break;
            }

            processTurnString(currTurn, CurrentPlayer, this);

            if (CurrentPlayer.state == IPlayer.States.Winner)
            {
                Rival.state = IPlayer.States.Loser;
                winner = CurrentPlayer;
            }
            else if (CurrentPlayer.state == IPlayer.States.Loser)
            {
                Rival.state = IPlayer.States.Winner;
                winner = Rival;
            }

        }

    }

    public void Transition(UCB1Tree.Transition t)
    {
        processTurnString(new Turn(t.Builder, t.Move, t.Build), CurrentPlayer, this);

        moveNum++;
    }

    public bool IsGameOver()
    {
        if (CurrentPlayer.state != IPlayer.States.Undetermined)
        {
            return true;
        }
        else if (Rival.state != IPlayer.States.Undetermined)
        {
            return true;
        }
        return false;
    }
}