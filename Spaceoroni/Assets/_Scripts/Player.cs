using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : IPlayer
{
    private Turn currentTurn;
    private Builder builder;  //TODO: Marked for deprecation?

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
        yield return true;
    }

    public override IEnumerator SelectBuilder(Game g)
    {
        //Select Builder
        Game.clickLocation = null;   //Reset click
        HighlightManager.highlightPlayersBuilder(this);
        while (Game.clickLocation == null && !Game.cancelTurn)
        {
            yield return new WaitForEndOfFrame();
        }
        if (!Game.cancelTurn)
        {
            currentTurn.BuilderLocation = Game.clickLocation;
            Game.clickLocation = null;
        }
        // after choosing a builder, find the best square you can move to from it.
        yield return null;
    }

    public override IEnumerator chooseMove(Game g)
    {
        Coordinate temp = new Coordinate(currentTurn.BuilderLocation);
        List<string> allMoves = g.getAllPossibleMoves(currentTurn.BuilderLocation);

        //Debug.Log((this.gameObject.GetComponentsInChildren<Builder>())[0]);
        //Debug.Log(Coordinate.coordToString(this.gameObject.GetComponentsInChildren<Builder>()[1].coord));
        //allMoves.ForEach(m => Debug.Log(m));

        if (g.canMove(currentTurn.BuilderLocation))
        {
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
                    currentTurn.BuilderLocation = Game.clickLocation;
                    Game.clickLocation = null;
                    HighlightManager.unhighlightAllPossibleMoveLocations(allMoves);
                    yield return StartCoroutine(chooseMove(g));
                }
                else
                {
                    currentTurn.MoveLocation = Game.clickLocation;
                    Game.clickLocation = null;

                    HighlightManager.unhighlightAllPossibleMoveLocations(allMoves);

                    if (g.canBuild(currentTurn.BuilderLocation))
                    {
                        moveBuidler(getBuilderInt(new Coordinate(currentTurn.BuilderLocation.x, currentTurn.BuilderLocation.y)), currentTurn.MoveLocation, g);
                    }
                    else
                    {
                        currentTurn.canPerformTurn = false;
                        Debug.Log("Cant Build");
                    }
                }
            }
        }
        else
        {
            currentTurn.canPerformTurn = false;
            yield return StartCoroutine(SelectBuilder(g));
            Debug.Log("Cant Move");
        }

        currentTurn.BuilderLocation = temp;
    }

    public override IEnumerator chooseBuild(Game g)
    {

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
            currentTurn.BuildLocation = Game.clickLocation;
            Game.clickLocation = null;

            HighlightManager.unhighlightAllPossibleBuildLocations(allBuildLevels);
        }

    }

    public override IEnumerator beginTurn(Game g)
    {
        currentTurn = new Turn();
        
        // after activation choose a builder
        yield return StartCoroutine(SelectBuilder(g));

        //choose a move
        yield return StartCoroutine(chooseMove(g));

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

        if (GameSettings.gameType == GameSettings.GameType.Multiplayer) RaiseTurnSelected(currentTurn);
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
