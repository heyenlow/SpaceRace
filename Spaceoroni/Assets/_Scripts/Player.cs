using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : IPlayer
{

    public GameObject Builder1GameObject;
    public GameObject Builder2GameObject;
    private Turn currentTurn;

    // Start is called before the first frame update
    void Start()
    {
        Builder1 = Builder1GameObject.GetComponent<Builder>();
        Builder2 = Builder2GameObject.GetComponent<Builder>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public new IEnumerator PlaceBuilder(int i, Game g)
    {
        g.addAllLocationsWithoutBuildersToHighlight();
        while (Game.clickLocation == null)
        {
            yield return new WaitForEndOfFrame();
        }
        Coordinate moveLocation = Game.clickLocation;
        Game.clickLocation = null;

        if (moveLocation != null)
        {
            moveBuidler(i, moveLocation, g);

            HighlightManager.highlightedObjects.Clear();
        }
        yield return true;
    }

    public IEnumerator SelectBuilder()
    {
        //Select Builder
        Game.clickLocation = null;   //Reset click
        HighlightManager.highlightPlayersBuilder(this);
        while (Game.clickLocation == null)
        {
            yield return new WaitForEndOfFrame();
        }
        currentTurn.BuilderLocation = Game.clickLocation;
        Game.clickLocation = null;
    }

    public IEnumerator chooseMove(Game g)
    {
        List<string> allMoves = g.getAllPossibleMoves(currentTurn.BuilderLocation);

        while (Game.clickLocation == null)
        {
            HighlightManager.highlightAllPossibleMoveLocations(allMoves);
            yield return new WaitForEndOfFrame();
        }

        currentTurn.MoveLocation = Game.clickLocation;
        Game.clickLocation = null;

        HighlightManager.unhighlightAllPossibleMoveLocations(allMoves);

        //bug vvvv
        moveBuidler(getBuilderInt(currentTurn.BuilderLocation), currentTurn.MoveLocation, g);

    }

    public IEnumerator chooseBuild(Game g)
    {

        //Build Block
        Game.clickLocation = null;
        List<string> allBuilds = g.getAllPossibleBuilds(currentTurn.MoveLocation);
        List<GameObject> allBuildLevels = g.getAllPossibleBuildLevels(allBuilds);

        while (Game.clickLocation == null)
        {
            HighlightManager.highlightAllPossibleBuildLocations(allBuildLevels);
            yield return new WaitForEndOfFrame();
        }
        currentTurn.BuildLocation = Game.clickLocation;
        Game.clickLocation = null;

        HighlightManager.unhighlightAllPossibleBuildLocations(allBuildLevels);

    }

    public override IEnumerator beginTurn(Game g)
    {
        currentTurn = new Turn();
        
        // after activation choose a builder
        yield return StartCoroutine(SelectBuilder());

        // after choosing a builder, find the best square you can move to from it.
        yield return StartCoroutine(chooseMove(g));


        // after choosing a move, need to find the best square to build on. What do I do about this?
        if (g.isWin(currentTurn.MoveLocation))
        {
            currentTurn.isWin = true;
            turns.Add(currentTurn);
            yield return null;
        }
        else
        {
            yield return StartCoroutine(chooseBuild(g));
            turns.Add(currentTurn);
        }
    }

}
