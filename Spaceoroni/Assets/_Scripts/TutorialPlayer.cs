using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlayer : Player
{
    public GameObject PlacingBuilderOverlay;
    public GameObject SelectBuilderOverlay;
    public GameObject SelectMoveOverlay;
    public GameObject SelectBuildOverlay;


    public override IEnumerator PlaceBuilder(int builder, int player, Game g)
    {
        if (builder == 1) PlacingBuilderOverlay.SetActive(true);
        while (PlacingBuilderOverlay.activeInHierarchy)
        {
            yield return new WaitForEndOfFrame();
        }

        turnText.text = "Place Builders";

        g.addAllLocationsWithoutBuildersToHighlight();

        Coordinate builderLocation = (builder == 1) ? StringGameReader.player1builder1Location : StringGameReader.player1builder2Location;

        GameObject.Find(Coordinate.coordToString(builderLocation)).GetComponent<Location>().Blink();
        Location.LocationBlinking = GameObject.Find(Coordinate.coordToString(builderLocation)).GetComponent<Location>();


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

        }

        turnText.text = "";
        yield return true;
    }

    public override IEnumerator SelectBuilder(Game g)
    {
        if (StringGameReader.MoveCount == 1) SelectBuilderOverlay.SetActive(true);
        while (SelectBuilderOverlay.activeInHierarchy)
        {
            yield return new WaitForEndOfFrame();
        }

        turnText.text = "Select a Builder";
        //Select Builder
        Game.clickLocation = null;   //Reset click

        if (Coordinate.Equals(Builder1.getLocation(), currentTurn.BuilderLocation)) Builder1.Blink();
        if (Coordinate.Equals(Builder2.getLocation(), currentTurn.BuilderLocation)) Builder2.Blink();

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
        turnText.text = "";
        yield return null;
    }

    public override IEnumerator chooseMove(Game g)
    {
        if (StringGameReader.MoveCount == 1) SelectMoveOverlay.SetActive(true);
        while (SelectMoveOverlay.activeInHierarchy)
        {
            yield return new WaitForEndOfFrame();
        }
        turnText.text = "Select a Move";

        Coordinate temp = new Coordinate(currentTurn.BuilderLocation);
        List<string> allMoves = g.getAllPossibleMoves(currentTurn.BuilderLocation);

        //Debug.Log((this.gameObject.GetComponentsInChildren<Builder>())[0]);
        //Debug.Log(Coordinate.coordToString(this.gameObject.GetComponentsInChildren<Builder>()[1].coord));
        //allMoves.ForEach(m => Debug.Log(m));

        if (g.canMove(currentTurn.BuilderLocation))
        {
            HighlightManager.highlightPlayersBuilder(this);
            HighlightManager.highlightAllPossibleMoveLocations(allMoves);

            GameObject.Find(Coordinate.coordToString(currentTurn.MoveLocation)).GetComponent<Location>().Blink();

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
        turnText.text = "";

    }

    public override IEnumerator chooseBuild(Game g)
    {
        if (StringGameReader.MoveCount == 1) SelectBuildOverlay.SetActive(true);
        while (SelectBuildOverlay.activeInHierarchy)
        {
            yield return new WaitForEndOfFrame();
        }
        turnText.text = "Select a Rocket to Build or Blast Off";

        //Build Block
        Game.clickLocation = null;
        List<string> allBuilds = g.getAllPossibleBuilds(currentTurn.MoveLocation);
        List<GameObject> allBuildLevels = g.getAllPossibleBuildLevels(allBuilds);

        HighlightManager.highlightAllPossibleBuildLocations(allBuildLevels);

        //only blink the top layer
        var levels = GameObject.Find(Coordinate.coordToString(currentTurn.BuildLocation)).GetComponentsInChildren<Level>();
        levels[levels.Length -1].Blink();

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

        turnText.text = "";
    }

    public override IEnumerator beginTurn(Game g)
    {
        currentTurn = StringGameReader.getCurrentTurn();
        //play the first 2 turns
        if (StringGameReader.MoveCount <= 3)
        {
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

            yield return null;
        }
        else
        {
            yield return new WaitForSeconds(1);
            moveBuidler(getBuilderInt(currentTurn.BuilderLocation), currentTurn.MoveLocation, g); ;
            turns.Add(currentTurn);
        }
    }

}
