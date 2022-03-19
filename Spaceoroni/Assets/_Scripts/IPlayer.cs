using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class IPlayer : MonoBehaviour
{
    protected Builder Builder1;
    protected Builder Builder2;
    protected List<Turn> turns;
    protected TextMeshProUGUI turnText;


    private void Start()
    {
        turns = new List<Turn>();
        Builder1 = this.gameObject.GetComponentsInChildren<Builder>()[0];
        Builder2 = this.gameObject.GetComponentsInChildren<Builder>()[1];
        turnText = GameObject.FindGameObjectWithTag("TurnText").GetComponent<TextMeshProUGUI>();
    }

    public abstract IEnumerator beginTurn(Game g);
    /// <summary>
    /// Returns a string showing the current coordinates of this player's builders
    /// </summary>
    /// <returns></returns>
    public string getBuilderLocations()
    {
        return (Coordinate.coordToString(Builder1.getLocation()) + Coordinate.coordToString(Builder2.getLocation()));
    }

    public void moveBuidler(int Builder, Coordinate to, Game g)
    {
        Builder builderToMove = Builder == 1 ? Builder1 : Builder2;

        builderToMove.move(to, g);
    }

    public virtual Turn getNextTurn()
    {
        return turns[turns.Count - 1];
    }
    public virtual IEnumerator PlaceBuilder(int builder, int player, Game game)
    {
        throw new NotImplementedException();
    }

    public int getBuilderInt(Coordinate location)
    {
        if (Coordinate.Equals(Builder1.getLocation(), location)) return 1;
        if (Coordinate.Equals(Builder2.getLocation(), location)) return 2;
        throw new Exception("No builder at this location");
    }

    public Tuple<Builder, Builder> getBuilders() { return new Tuple<Builder, Builder>(Builder1, Builder2); }

    public virtual IEnumerator SelectBuilder(Game g)
    {
        throw new NotImplementedException();
    }

    public virtual IEnumerator chooseMove(Game g)
    {
        throw new NotImplementedException();
    }

    public virtual IEnumerator chooseBuild(Game g)
    {
        throw new NotImplementedException();
    }

    public virtual void loadNEATPlayer(string path)
    {
        throw new NotImplementedException();
    }
    public void resetPlayer()
    {
        setAllBuildersActive();
        turns.Clear();
        Builder1.returnHome();
        Builder2.returnHome();
    }

    public void ClearTurnText() { turnText.text = ""; }

    /// <summary>
    /// used to make the builder disapear on a win
    /// </summary>
    public void setBuilderAtLocationInactive(Coordinate c)
    {
        if (Coordinate.Equals(Builder1.getLocation(), c))
        {
            Builder1.gameObject.SetActive(false);
        }
        else if (Coordinate.Equals(Builder1.getLocation(), c))
        {
            Builder2.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("No builder found at the given coordinate. Give Coordinate:" + Coordinate.coordToString(c) + " Builder Locations: " + getBuilderLocations());
        }
    }
    private void setAllBuildersActive() { Builder1.gameObject.SetActive(true); Builder2.gameObject.SetActive(true); }
    public bool BuildersAreMoving() { return Builder1.currentlyMoving || Builder2.currentlyMoving; }
}
