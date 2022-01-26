using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Material highlight;

    int[,] Board;
    public GameObject Player1Object;
    public GameObject Player2Object;
    Player Player1;
    Player Player2;
    Player Winner;
    Player TurnPlayer;
    public float Speed;

    [Header("Test Objects")]

    bool highlightAll = false;
    Coordinate clickLocation;

    GameObject movingBuilder;
    Vector3 newLocation;
    int waitToStart = 0;
    public bool GameRunning = false;

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
        Board[2, 3] = 3;
        yield return null;
        yield return StartCoroutine(PlaceBuilders());
        yield return StartCoroutine(RunGame());
    }
    IEnumerator RunGame()
    {
        while (Winner == null)
        {
            yield return StartCoroutine(Turn(Player1));
            if (Winner == null) yield return StartCoroutine(Turn(Player2));
        }
        yield return null;
    }

    public void moveToNewSquare(GameObject GamePiece, GameObject Square)
    {
        Coordinate coordinateOfSquare = Coordinate.stringToCoord(Square.name);
        //this next line will need to be adjusted for the height of each level object
        Vector3 heightDiff = new Vector3(0, (GamePiece.transform.position.y - Square.transform.position.y) + (heightAtCoordinate(coordinateOfSquare)), 0);
        newLocation = Square.transform.position + heightDiff;
        movingBuilder = GamePiece;
    }

    //returns the board height at a given coordinate
    float heightAtCoordinate(Coordinate c)
    {
        const float gamePieceHeight = 0;
        const float level0Height = (float)0.001;
        const float level1Height = (float)5.0;
        const float level2Height = (float)0.001;
        const float level3Height = (float)0.001;
        const float level4Height = (float)0.001;

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
        highlightAll = true;
        while (clickLocation == null)
        {
            yield return new WaitForSeconds(1);
        }
        if (clickLocation != null)
        {
            Debug.Log("new lovation is: " + Coordinate.coordToString(clickLocation));
            p.PlaceBuilder(i, clickLocation);

            clickLocation = null;
            highlightAll = false;
        }
        yield return true;
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

    IEnumerator Turn(Player p)
    {
        Debug.Log(p + " Turn Start:");
        TurnPlayer = p;
        Coordinate builderLocation = null;
        while (clickLocation == null)
        {
            builderLocation = clickLocation;
            yield return new WaitForSeconds(1);
        }

        //###################################################################
        //Where to?
        Coordinate moveLocation = null;
        if (builderLocation != null)
        {
            Debug.Log("BuilderLocation: " + Coordinate.coordToString(builderLocation));
            List<string> allMoves = getAllPossibleMoves(builderLocation);
            highlightPossibleMoveLocations(allMoves);
            while (clickLocation == null)
            {
                moveLocation = clickLocation;
                yield return new WaitForSeconds(1);
            }
            p.moveBuidler(builderLocation, moveLocation);
        }

        //################################################################
        // get build location if not win
        Coordinate buildLocation = null;
        if (moveLocation != null && !isWin(moveLocation))
        {
            Debug.Log("MoveLocation: " + Coordinate.coordToString(moveLocation));
            List<string> allBuilds = getAllPossibleBuilds(moveLocation);
            highlightPossibleMoveLocations(allBuilds);
            while (clickLocation == null)
            {
                moveLocation = clickLocation;
                yield return new WaitForSeconds(1);
            }
            BuildLevel(buildLocation);
        }
        if(buildLocation != null) Debug.Log("BuildLocation: " + Coordinate.coordToString(buildLocation));

        yield return null;

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
    public void highlightPossibleMoveLocations(List<string> locations)
    {
        foreach (string coord in locations)
        {
            GameObject.Find(coord).GetComponent<Renderer>().material = highlight;
        }
    }
    public void highlightPossibleMoveLocations(Coordinate location)
    {
       GameObject.Find(Coordinate.coordToString(location)).GetComponent<Renderer>().material = highlight;
    }

    public void recieveLocationClick(Coordinate location)
    {
        clickLocation = location;
    }

    public void canHighlightBuilderPlacement(Coordinate c)
    {
        if(!getAllBuildersString().Contains(Coordinate.coordToString(c)) && highlightAll) 
            GameObject.Find(Coordinate.coordToString(c)).GetComponent<Renderer>().material = highlight;

    }

    public GameObject findSquare(Coordinate c)
    {
        return GameObject.Find(Coordinate.coordToString(c));
    }

    public bool isTurn(Coordinate c)
    {
        Debug.Log(TurnPlayer.getBuilderLocations() + " " + Coordinate.coordToString(c));
        return TurnPlayer.getBuilderLocations().Contains(Coordinate.coordToString(c));
    }

    public Material getHighlightMat()
    {
        return highlight;
    }
}
