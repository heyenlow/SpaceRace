using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    Tuple<Builder, Builder> Builder;
    public GameObject Builder1GameObject;
    public GameObject Builder2GameObject;
    Builder Builder1;
    Builder Builder2;
    public string name { get; set; }

    public Player(string n)
    {
        name = n;
    }

    // Start is called before the first frame update
    void Start()
    {
        Builder1 = Builder1GameObject.GetComponent<Builder>();
        Builder2 = Builder2GameObject.GetComponent<Builder>();
        Debug.Log(Builder1.name + " ------ " + Builder2.name);

    }

    // Update is called once per frame
    void Update()
    {

    }

    //used to place builders at the beginning of the game
    public void PlaceBuilder(int i, Coordinate c)
    {
        Debug.Log(Builder1.name + " ------ " + Builder2.name);
        Debug.Log(Coordinate.coordToString(c));
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
        if (Builder1.getLocation() == Coordinate.coordToString(from))
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
        return (Builder1.getLocation() + Builder1.getLocation());
    }
}
