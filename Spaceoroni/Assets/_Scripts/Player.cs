using System;
using System.Collections;
using System.Collections.Generic;
using SpaceConsole1;
//using UnityEngine;

public class Player //: MonoBehaviour
{

    Builder Builder1;
    Builder Builder2;
    public string name { get; set; }

    public Player(string n)
    {
        name = n;
    }

    // Start is called before the first frame update
    public void Start()
    {
        Builder1 = new Builder();
        Builder2 = new Builder();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //used to place builders at the beginning of the game
    public void PlaceBuilder(int i, Coordinate c)
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

    public void moveBuidler(Coordinate from, Coordinate to)
    {
        if(Builder1.getLocation() == Coordinate.coordToString(from))
        {
            Builder1.move(to);
        }
        else if (Builder2.getLocation() == Coordinate.coordToString(from))
        {
            Builder2.move(to);
        }
    }

    public string getBuilderLocations()
    {
        return (Builder1.getLocation() + Builder2.getLocation());
    }
}
