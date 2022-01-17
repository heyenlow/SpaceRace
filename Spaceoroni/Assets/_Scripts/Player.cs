using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    Builder Builder1;
    Builder Builder2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //used to place builders at the beginning of the game
    void PlaceBuilder(int i)
    {

    }

    //manages what goes on during the normal turn
    void Turn()
    {
        SelectBuilder();
        Move();
        Build();
    }

    //select one of the two builders
    void SelectBuilder()
    {

    }

    //move the selected builder to osne of the possible locations
    void Move()
    {

    }

    //build from one of the possible squares
    void Build()
    {
        //select coords of a square to build on
        int x,y;
        Game.BuildLevel(x,y);
    }
}
