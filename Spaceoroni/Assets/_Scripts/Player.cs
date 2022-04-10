using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

public class Player : IPlayer
{

    protected Turn currentTurn;

    public bool clickedBuilder(Coordinate click)
    { 
        Builder[] builders = this.gameObject.GetComponentsInChildren<Builder>();

        for(int i = 0; i < builders.Length; i++)
        {
            if(click.x == builders[i].coord.x &&
                click.y == builders[i].coord.y)
            {
                return true;
            }
        }

        return false;
    }

    public override IEnumerator PlaceBuilder(int builder, int player, Game g)
    {
        turnText.text = "Place Builder";

        g.addAllLocationsWithoutBuildersToHighlight();

        while (Game.clickLocation == null && !Game.cancelTurn)
        {
            yield return new WaitForEndOfFrame();
        }
        if (!Game.cancelTurn)
        {

            Coordinate moveLocation = Game.clickLocation;
            Game.clickLocation = null;

            if (moveLocation != null)
            {
                moveBuidler(builder, moveLocation, g);

                HighlightManager.highlightedObjects.Clear();
            }

            if (GameSettings.gameType == GameSettings.GameType.Multiplayer) RaiseBuilderLocation(moveLocation, builder);
        }

        turnText.text = "";
        yield return true;
    }

    public override IEnumerator SelectBuilder(Game g)
    {
        turnText.text = "Select a Builder";
        //Select Builder
        Game.clickLocation = null;   //Reset click

        HighlightManager.highlightPlayersBuilder(this);
        
        bool locationHasMoves = false;
        while (!locationHasMoves && !Game.cancelTurn)
        {
            while (Game.clickLocation == null && !Game.cancelTurn)
            {
                yield return new WaitForEndOfFrame();
            }
            if (!Game.cancelTurn)
            {
                if (g.getAllPossibleMoves(Game.clickLocation).Count > 0) locationHasMoves = true;
                else
                {
                    HighlightManager.highlightPlayersBuilder(this);
                    Game.clickLocation = null;
                    turnText.text = "That Builder Cannot Move Select Another Builder";
                }
            }
        }
        if (!Game.cancelTurn)
        {
            currentTurn.BuilderLocation = new Coordinate(Game.clickLocation);
            Game.clickLocation = null;
        }
        // after choosing a builder, find the best square you can move to from it.
        turnText.text = "";
        yield return null;
    }

    public override IEnumerator chooseMove(Game g)
    {
        turnText.text = "Select a Move";

        Coordinate temp = new Coordinate(currentTurn.BuilderLocation);
        List<string> allMoves = g.getAllPossibleMoves(currentTurn.BuilderLocation);

        //Debug.Log(Coordinate.coordToString(this.gameObject.GetComponentsInChildren<Builder>()[1].coord));
        //allMoves.ForEach(m => Debug.Log(m));

            HighlightManager.highlightPlayersBuilder(this);
            HighlightManager.highlightAllPossibleMoveLocations(allMoves);
            while (Game.clickLocation == null && !Game.cancelTurn)
            {
                yield return new WaitForEndOfFrame();
            }
            if (!Game.cancelTurn)
            {

                if (clickedBuilder(Game.clickLocation))
                {
                    currentTurn.BuilderLocation = new Coordinate(Game.clickLocation);
                    Game.clickLocation = null;
                    HighlightManager.unhighlightAllPossibleMoveLocations(allMoves);
                    yield return StartCoroutine(chooseMove(g));
                }
                else
                {
                    currentTurn.MoveLocation = new Coordinate(Game.clickLocation);
                    Game.clickLocation = null;

                    HighlightManager.unhighlightAllPossibleMoveLocations(allMoves);
                    if (g.isWin(currentTurn.MoveLocation))
                    {
                        
                    }
                    else if (g.canBuild(currentTurn.BuilderLocation))
                    {
                        moveBuidler(getBuilderInt(new Coordinate(currentTurn.BuilderLocation.x, currentTurn.BuilderLocation.y)), currentTurn.MoveLocation, g);
                        while (BuildersAreMoving()) yield return new WaitForEndOfFrame();
                    }
                    else
                    {
                        currentTurn.canPerformTurn = false;
                        Debug.Log("Cant Build");
                    }

                    currentTurn.BuilderLocation = temp;
                    turnText.text = "";
                }
            }
    }

    public override IEnumerator chooseBuild(Game g)
    {
        turnText.text = "Select a Rocket to Build or Blast Off";

        //Build Block
        Game.clickLocation = null;
        List<string> allBuilds = g.getAllPossibleBuilds(currentTurn.MoveLocation);
        List<GameObject> allBuildLevels = g.getAllPossibleBuildLevels(allBuilds);
        
        HighlightManager.highlightAllPossibleBuildLocations(allBuildLevels);
        while (Game.clickLocation == null && !Game.cancelTurn)
        {
            yield return new WaitForEndOfFrame();
        }

        if (!Game.cancelTurn)
        {
            currentTurn.BuildLocation = new Coordinate(Game.clickLocation);
            Game.clickLocation = null;

            HighlightManager.unhighlightAllPossibleBuildLocations(allBuildLevels);
        }

        turnText.text = "";
    }

    public override IEnumerator beginTurn(Game g)
    {
        currentTurn = new Turn();
        
        // after activation choose a builder
        yield return StartCoroutine(SelectBuilder(g));

        //choose a move
        yield return StartCoroutine(chooseMove(g));
        //while (BuildersAreMoving())
        //{
        //    yield return new WaitForEndOfFrame();
        //}

        // after choosing a move, need to find the best square to build on. What do I do about this?
        if (!currentTurn.canPerformTurn)
        {
            turns.Add(currentTurn);
            g.playerState = Game.PlayerState.Loser;
        }
        else if (g.isWin(currentTurn.MoveLocation))
        {
            currentTurn.isWin = true;
            turns.Add(currentTurn);
            g.playerState = Game.PlayerState.Loser;
        }
        else
        {
            g.playerState = Game.PlayerState.Playing;
            yield return StartCoroutine(chooseBuild(g));
            turns.Add(currentTurn);
        }

        if (GameSettings.gameType == GameSettings.GameType.Multiplayer && GameSettings.netMode != GameSettings.NetworkMode.Local) RaiseTurnSelected(currentTurn);
        yield return null;
    }

    private void RaiseBuilderLocation(Coordinate BuilderCoordinate, int BuilderInt)
    {
        object[] datas = new object[] { BuilderInt, BuilderCoordinate.x, BuilderCoordinate.y };
        PhotonNetwork.RaiseEvent(NetworkingManager.RAISE_INITIAL_BUILDER, datas, RaiseEventOptions.Default, SendOptions.SendReliable);
        Debug.Log("Raising Builder Location");
    }

    private void RaiseTurnSelected(Turn t)
    {
        object[] datas = t.turnToObjectArray();
        PhotonNetwork.RaiseEvent(NetworkingManager.RAISE_TURN, datas, RaiseEventOptions.Default, SendOptions.SendReliable);
        Debug.Log("Raising Turn" + t.ToString());
    }

}
