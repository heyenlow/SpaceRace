using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimIPlayer : MonoBehaviour
{
    public int ID;
    public enum States
    {
        Winner,
        Loser,
        Tied,
        Undetermined
    }
    public Builder Builder1 = new Builder();
    public Builder Builder2 = new Builder();
    public States state = States.Undetermined;
    private bool _disposed;

    public abstract Coordinate SelectBuilder();
    public abstract Coordinate chooseMove(ref Coordinate builder, Game g);
    public abstract Coordinate chooseBuild(ref Coordinate builder, ref Coordinate oldLocatiion, Game g);
    public abstract string beginTurn(Game g, out bool isWin);
    /// <summary>
    /// Returns a string showing the current coordinates of this player's builders
    /// </summary>
    /// <returns></returns>
    public string getBuilderLocations()
    {
        return (Builder1.getLocation() + Builder2.getLocation());
    }
    //used to place builders at the beginning of the game
    public virtual void PlaceBuilder(int i, Coordinate c)
    {
        switch (i)
        {
            case 1:
                Builder1.move(c);
                break;
            case 2:
                Builder2.move(c);
                break;
        }
    }
    public void moveBuilder(Coordinate from, Coordinate to)
    {
        if (Builder1.getLocation() == Coordinate.coordToString(from))
        {
            Builder1.move(to);
        }
        else if (Builder2.getLocation() == Coordinate.coordToString(from))
        {
            Builder2.move(to);
        }
    }


    public T DeepCopy<T>()
        where T : IPlayer
    {
        T temp;
        using (MemoryStream ms = new MemoryStream())
        {
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(ms, this);
            ms.Position = 0;
            temp = (T)binaryFormatter.Deserialize(ms);
        }
        temp.Builder1.move(Builder1.Location);
        temp.Builder2.move(Builder2.Location);

        return temp;
    }

    public virtual void Dispose()
    {
        // Dispose of unmanaged resources.
        Dispose(true);
        // Suppress finalization.
        //GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) // if this has already been disposed do nothing...
        {
            return;
        }

        if (disposing)
        {
            // TODO: dispose managed state (managed objects).
            // no managed state in this class? managed means memory mostly...
            // Builder1 and Builder2 are using memory... but they're unmanaged because they're a custom class
            // so they need to be de-allocated outside of this conditional statement I think...
            // setting them to null should set them up for garbace collection...
        }

        Builder1 = null;
        Builder2 = null;

        _disposed = true;
    }

    // finalizer is called for very rare chance dispose is not called before the Garbage Collector comes for our object...
    ~IPlayer() // finalizer
    {
        // clean up statements
        Dispose(false);
    }
}
