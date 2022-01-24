using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    Tuple<Builder, Builder> Builder;
    public string name { get; set; }

    public Player(string n)
    {
        name = n;
    }

    // Start is called before the first frame update
    public void Start()
    {
        setBuilders();
    }

    Tuple<Builder, Builder> setBuilders()
    {
        var PlayerBuilders = GameObject.FindGameObjectsWithTag(this.tag);
        GameObject builder1 = new GameObject();
        GameObject builder2 = new GameObject();

        foreach(GameObject g in PlayerBuilders)
        {
            if (g.name == "Builder1") builder1 = g;
            if (g.name == "Builder2") builder2 = g;
        }
        return new Tuple<Builder, Builder>(builder1.GetComponent<Builder>(), builder2.GetComponent<Builder>());
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
                Builder.Item1.move(c);
                break;
            case 2:
                Builder.Item2.move(c);
                break;
        }
    }

    public void moveBuidler(Coordinate from, Coordinate to)
    {
        if (Builder.Item1.getLocation() == Coordinate.coordToString(from))
        {
            Builder.Item2.move(to);
        }
        else if (Builder.Item2.getLocation() == Coordinate.coordToString(from))
        {
            Builder.Item2.move(to);
        }
    }

    public string getBuilderLocations()
    {
        return (Builder.Item1.getLocation() + Builder.Item2.getLocation());
    }
}
