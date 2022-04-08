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
    [SerializeField]
    private GameObject MoveUpALevel;
    [SerializeField]
    private GameObject MoveDownLevels;
    private bool OverlayISActive()
    {
        return SelectMoveOverlay.activeInHierarchy ||
                MoveUpALevel.activeInHierarchy ||
                MoveDownLevels.activeInHierarchy ||
                MoveToWinOverlay.activeInHierarchy ||
                PlacingBuilderOverlay.activeInHierarchy ||
                SelectBuilderOverlay.activeInHierarchy ||
                SelectBuildOverlay.activeInHierarchy ||
                BlockARocketOverlay.activeInHierarchy;
    }




    public override IEnumerator PlaceBuilder(int builder, int player, Game g)
    {
        if (builder == 1) PlacingBuilderOverlay.SetActive(true);
        while (OverlayISActive())
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
        if (StringGameReader.MoveCount == StringGameReader.firstMove) SelectBuilderOverlay.SetActive(true);
        while (OverlayISActive())
        {
            yield return new WaitForEndOfFrame();
        }

        turnText.text = "Select a Builder";
        //Select Builder
        Game.clickLocation = null;   //Reset click

        HighlightManager.highlightPlayersBuilder(this);
        if (Coordinate.Equals(Builder1.getLocation(), currentTurn.BuilderLocation)) Builder1.Blink();
        if (Coordinate.Equals(Builder2.getLocation(), currentTurn.BuilderLocation)) Builder2.Blink();

        //checks to make sure it is the blinking item in the builder script
        while (Game.clickLocation == null && !Game.cancelTurn)
        {
            yield return new WaitForEndOfFrame();
        }
        if (!Game.cancelTurn)
        {
            Game.clickLocation = null;
        }

        turnText.text = "";
        yield return null;
    }


    public override IEnumerator chooseMove(Game g)
    {
        if (StringGameReader.MoveCount == StringGameReader.firstMove) SelectMoveOverlay.SetActive(true);
        if (StringGameReader.MoveCount == StringGameReader.MoveUpaLevel) MoveUpALevel.SetActive(true);
        if (StringGameReader.MoveCount == StringGameReader.MoveDownALevel) MoveDownLevels.SetActive(true);
        if (StringGameReader.MoveCount == StringGameReader.MoveToWin) MoveToWinOverlay.SetActive(true);

        while (OverlayISActive())
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

            bool matchedMove = false;

            while (!matchedMove && !Game.cancelTurn)
            {
                while (Game.clickLocation == null && !Game.cancelTurn)
                {
                    yield return new WaitForEndOfFrame();
                }
                if (!Game.cancelTurn)
                {
                    if (Coordinate.Equals(Game.clickLocation, currentTurn.MoveLocation)) matchedMove = true;
                    else
                    {
                        Game.clickLocation = null;
                        HighlightManager.highlightPlayersBuilder(this);
                        HighlightManager.highlightAllPossibleMoveLocations(allMoves);
                        GameObject.Find(Coordinate.coordToString(currentTurn.MoveLocation)).GetComponent<Location>().Blink();
                    }
                }
            }
            if (!Game.cancelTurn)
            {
                //depreciated for tutorial
                if (false)//clickedBuilder(Game.clickLocation))
                {
                    Game.clickLocation = null;
                    HighlightManager.unhighlightAllPossibleMoveLocations(allMoves);
                    yield return StartCoroutine(chooseMove(g));
                }
                else
                {
                    Game.clickLocation = null;

                    HighlightManager.unhighlightAllPossibleMoveLocations(allMoves);

                    if (g.isWin(currentTurn.MoveLocation))
                    {

                    }
                    else if (g.canBuild(currentTurn.BuilderLocation))
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
        if (StringGameReader.MoveCount == StringGameReader.firstMove) SelectBuildOverlay.SetActive(true);
        if (StringGameReader.MoveCount == StringGameReader.BlastOffRocket) BlockARocketOverlay.SetActive(true);

        while (SelectBuildOverlay.activeInHierarchy || BlockARocketOverlay.activeInHierarchy)
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
            Game.clickLocation = null;

            HighlightManager.unhighlightAllPossibleBuildLocations(allBuildLevels);
        }

        turnText.text = "";
    }

    public override IEnumerator beginTurn(Game g)
    {
        while (Game.PAUSED) { yield return new WaitForEndOfFrame(); }
        currentTurn = StringGameReader.getCurrentTurn();
        //play the first 2 turns
        if (StringGameReader.isSpecialMove(StringGameReader.MoveCount))
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
            turnText.text = "Fast Forwarding...";
            yield return new WaitForSeconds(3);
            
            while (Game.PAUSED) { yield return new WaitForEndOfFrame(); }
            moveBuidler(getBuilderInt(currentTurn.BuilderLocation), currentTurn.MoveLocation, g);
            
            while (Game.PAUSED) { yield return new WaitForEndOfFrame(); }
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
