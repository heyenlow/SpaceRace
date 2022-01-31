using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Material highlight;
    public Material possibleHighlight;

    int[,] Board;
    public GameObject Player1Object;
    public GameObject Player2Object;
    Player Player1;
    Player Player2;
    Player Winner;
    Player TurnPlayer;
    public float Speed;

    [Header("Test Objects")]

    private List<GameObject> highlightedObjects = new List<GameObject>();
    Coordinate clickLocation;

    GameObject movingBuilder;
    Vector3 newLocation;
    private bool GameRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        Winner = null;
        Board = new int[5, 5];
        Player1 = Player1Object.GetComponent<Player>();
        Player2 = Player2Object.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        //gotta wait for the other objects to be set
        if (Input.GetKeyDown("space") && !GameRunning) StartCoroutine(NewGame());

        if (movingBuilder != null)
        {
            movingBuilder.transform.position = Vector3.MoveTowards(movingBuilder.transform.position, newLocation, Speed * Time.deltaTime);

            if (movingBuilder.transform.position == newLocation) movingBuilder = null;
        }
    }

    IEnumerator NewGame()
    {
        Debug.Log("Starting New Game.");
        GameRunning = true;
        ClearBoard();
        
        BuildLevel(Coordinate.stringToCoord("E1"));

        BuildLevel(Coordinate.stringToCoord("E2"));
        BuildLevel(Coordinate.stringToCoord("E2"));

        BuildLevel(Coordinate.stringToCoord("E3"));
        BuildLevel(Coordinate.stringToCoord("E3"));
        BuildLevel(Coordinate.stringToCoord("E3"));

        BuildLevel(Coordinate.stringToCoord("E4"));
        BuildLevel(Coordinate.stringToCoord("E4"));
        BuildLevel(Coordinate.stringToCoord("E4"));
        BuildLevel(Coordinate.stringToCoord("E4"));

        yield return null;
        yield return StartCoroutine(PlaceBuilders());
        Debug.Log("Done Placing Builders");
        yield return StartCoroutine(RunGame());
    }
    IEnumerator RunGame()
    {
        while (Winner == null)
        {
            yield return StartCoroutine(Turn(Player1));

        }
        yield return null;
    }

    public void moveToNewSquare(GameObject GamePiece, GameObject Square)
    {
        Coordinate coordinateOfSquare = Coordinate.stringToCoord(Square.name);
        //this next line will need to be adjusted for the height of each level object
        Vector3 heightDiff = new Vector3(0, (heightAtCoordinate(coordinateOfSquare)), 0);
        newLocation = Square.transform.position + heightDiff;
        movingBuilder = GamePiece;
    }

    //returns the board height at a given coordinate
    float heightAtCoordinate(Coordinate c)
    {
        //The Scale Y * 2 of the level object
        const float gamepeiceHeight = (float) 0.35;
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

    IEnumerator PlaceBuilders()
    {
        yield return StartCoroutine(PlaceBuilder(Player1, 1));
        yield return StartCoroutine(PlaceBuilder(Player2, 1));
        yield return StartCoroutine(PlaceBuilder(Player2, 2));
        yield return StartCoroutine(PlaceBuilder(Player1, 2));
        yield return null;
    }

    private IEnumerator PlaceBuilder(Player p, int i)
    {
        Debug.Log("Player: " + p + " Placing builder: " + i);
        Debug.Log("waiting for location.....");
        addAllLocationsWithoutBuildersToHighlight();
        while (clickLocation == null)
        {
            yield return new WaitForSeconds(1);
        }
        if (clickLocation != null)
        {
            Debug.Log("new location is: " + Coordinate.coordToString(clickLocation));
            p.PlaceBuilder(i, clickLocation);

            clickLocation = null;
            highlightedObjects.Clear();
        }
        yield return true;
    }

    private void addAllLocationsWithoutBuildersToHighlight()
    {
        var locations = GameObject.FindGameObjectsWithTag("Square");
        var buildersLocations = getAllBuildersString();
        foreach (var l in locations)
        {
            if (!buildersLocations.Contains(l.name)) { highlightedObjects.Add(l); }
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
    void BuildLevel(Coordinate c)
    {
        Debug.Log(c);
        Board[c.x, c.y] += 1;
        GameObject level = GameObject.Find(Coordinate.coordToString(c));
        level.transform.GetChild(Board[c.x,c.y] - 1).gameObject.SetActive(true);
    }

    bool isWin(Coordinate c)
    {
        return Board[c.x, c.y] == 3;
    }

    IEnumerator Turn(Player p)
    {
        TurnPlayer = p;
        Coordinate builderLocation = null;
        Coordinate moveLocation = null;
        Coordinate buildLocation = null;

    //Select Builder
        clickLocation = null;   //Reset click
        highlightPlayersBuilder(TurnPlayer);
        while (clickLocation == null)
        {
            yield return new WaitForSeconds(1);
        }
        builderLocation = clickLocation;
        clickLocation = null;


        List<string> allMoves = getAllPossibleMoves(builderLocation);

    //Move Builder
        Debug.Log("Waiting for move");
        while (clickLocation == null)
        {
            highlightAllPossibleMoveLocations(allMoves);
            yield return new WaitForSeconds(1);
        }

        moveLocation = clickLocation;
        Debug.Log(Coordinate.coordToString(moveLocation));
        unhighlightAllPossibleMoveLocations(allMoves);
        p.moveBuidler(builderLocation, moveLocation);


    //Check win
        if (isWin(moveLocation))
        {
            //TODO: Endgame function
            Debug.Log("WIN!!!");
        }
        else
        {
            if (moveLocation != null)
            {
                //Build Block
                clickLocation = null;
                List<string> allBuilds = getAllPossibleBuilds(moveLocation);
                List<GameObject> allBuildLevels = getAllPossibleBuildLevels(allBuilds);

                while (clickLocation == null)
                {
                    highlightAllPossibleBuildLocations(allBuildLevels);
                    yield return new WaitForSeconds(1);
                }
                buildLocation = clickLocation;
                unhighlightAllPossibleBuildLocations(allBuildLevels);
                BuildLevel(buildLocation);


                //Next Players turn
                if (p == Player1)
                {
                    yield return StartCoroutine(Turn(Player2));
                }
                else
                {
                    yield return StartCoroutine(Turn(Player1));
                }
            }

            
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

    private void highlightPossibleMoveLocations(Coordinate location)
    {
        GameObject obj = GameObject.Find(Coordinate.coordToString(location));
        obj.GetComponent<Renderer>().material = possibleHighlight;
        highlightedObjects.Add(obj);
    }
    private void highlightAllPossibleMoveLocations(List<string> locations)
    {
        foreach (string coord in locations)
        {
            GameObject obj = GameObject.Find(coord);
            obj.GetComponent<Renderer>().material = possibleHighlight;
            highlightedObjects.Add(obj);
        }
    }
    private void unhighlightAllPossibleMoveLocations(List<string> locations)
    {
        foreach (string coord in locations)
        {
            GameObject obj = GameObject.Find(coord);
            obj.GetComponent<Location>().resetMaterial();
            highlightedObjects.Remove(obj);
        }
    }

    private void highlightAllPossibleBuildLocations(List<GameObject> Levels)
    {
        foreach (GameObject l in Levels)
        {
            l.SetActive(true);
            l.GetComponent<Level>().makeOpaque();
            highlightedObjects.Add(l);
        }
    }

    private void unhighlightAllPossibleBuildLocations(List<GameObject> Levels)
    {
        foreach (GameObject l in Levels)
        {
            l.SetActive(false);
            l.GetComponent<Level>().resetMaterial();
            highlightedObjects.Remove(l);
        }
    }
    private void highlightPlayersBuilder(Player p)
    {
        highlightedObjects.Add(p.Builder1GameObject);
        highlightedObjects.Add(p.Builder2GameObject);
    }

    public void recieveLocationClick(Coordinate location)
    {
        clickLocation = location;
        highlightedObjects.Clear();
    }

    public GameObject findSquare(Coordinate c)
    {
        return GameObject.Find(Coordinate.coordToString(c));
    }

    public bool isTurn(Coordinate c)
    {
        return TurnPlayer.getBuilderLocations().Contains(Coordinate.coordToString(c));
    }

    public Material getHighlightMat()
    {
        return highlight;
    }

    public bool isHighlightObj(GameObject obj)
    {
        return highlightedObjects.Contains(obj);
    }
}
