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


    public override IEnumerator PlaceBuilder(int builder, int player, Game g)
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
            moveBuidler(builder, moveLocation, g);

            HighlightManager.highlightedObjects.Clear();
        }

        if (GameSettings.gameType == GameSettings.GameType.Multiplayer) RaiseBuilderLocation(moveLocation, builder);

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

    public override IEnumerator chooseMove(Game g)
    {
        Coordinate temp = new Coordinate(currentTurn.BuilderLocation);
        List<string> allMoves = g.getAllPossibleMoves(currentTurn.BuilderLocation);
        
        if(g.canMove(currentTurn.BuilderLocation))
        {
            HighlightManager.highlightAllPossibleMoveLocations(allMoves);
            while (Game.clickLocation == null)
            {
                yield return new WaitForEndOfFrame();
            }

            currentTurn.MoveLocation = Game.clickLocation;
            Game.clickLocation = null;

            HighlightManager.unhighlightAllPossibleMoveLocations(allMoves);

            if (g.canBuild(currentTurn.BuilderLocation))
            {
                // bughere
                moveBuidler(getBuilderInt(new Coordinate(currentTurn.BuilderLocation.x, currentTurn.BuilderLocation.y)), currentTurn.MoveLocation, g);
            }
            else
            {
                currentTurn.canPerformTurn = false;
                Debug.Log("Cant Build");
            }   
        }
        else
        {
            currentTurn.canPerformTurn = false;
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
        while (Game.clickLocation == null)
        {
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
        if (!currentTurn.canPerformTurn)
        {
            turns.Add(currentTurn);
            g.playerState = Game.PlayerState.Loser;
            yield return null;
        }
        else if (g.isWin(currentTurn.MoveLocation))
        {
            currentTurn.isWin = true;
            turns.Add(currentTurn);
            g.playerState = Game.PlayerState.Loser;
            yield return null;
        }
        else
        {
            g.playerState = Game.PlayerState.Playing;
            yield return StartCoroutine(chooseBuild(g));
            turns.Add(currentTurn);
        }

        if (GameSettings.gameType == GameSettings.GameType.Multiplayer) RaiseTurnSelected(currentTurn);
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
