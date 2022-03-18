using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlayer : Player
{
    [SerializeField]
    private GameObject PlacingBuilderOverlay;
    [SerializeField]
    private GameObject SelectBuilderOverlay;
    [SerializeField]
    private GameObject SelectMoveOverlay;
    [SerializeField]
    private GameObject SelectBuildOverlay;
    [SerializeField]
    private GameObject BlockARocketOverlay;
    [SerializeField]
    private GameObject MoveToWinOverlay;


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
        if (StringGameReader.MoveCount == StringGameReader.lengthOfTutorialMoves()) MoveToWinOverlay.SetActive(true);

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
        if (StringGameReader.MoveCount == StringGameReader.BlastOffMove) BlockARocketOverlay.SetActive(true);

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
        //blink whole rocket if it is the third level
        if (g.getBoardHeightAtCoord(currentTurn.BuildLocation) == 3)
        {
            levels[0].GetComponentInParent<Rocket>().Blink();
        }
        else
        {
            levels[levels.Length - 1].OnlyBlinkThisLevel();
        }

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
        if (StringGameReader.MoveCount <= 3 || StringGameReader.MoveCount == StringGameReader.BlastOffMove || StringGameReader.MoveCount == StringGameReader.lengthOfTutorialMoves())
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
            turnText.text = ">>";
            yield return new WaitForSeconds(1);
            moveBuidler(getBuilderInt(currentTurn.BuilderLocation), currentTurn.MoveLocation, g);
            yield return new WaitForSeconds(1);
            turns.Add(currentTurn);
        }
    }

    public void setAllOverlaysInactive()
    {
        PlacingBuilderOverlay.SetActive(false);
        SelectBuilderOverlay.SetActive(false);
        SelectMoveOverlay.SetActive(false);
        SelectBuildOverlay.SetActive(false);
        BlockARocketOverlay.SetActive(false);
        MoveToWinOverlay.SetActive(false);
    }
}
