using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    int[,] Board;
    Player Player1;
    Player Player2;

    // Start is called before the first frame update
    void Start()
    {
        Board = new int[5,5];
        NewGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void NewGame()
    {
        ClearBoard();
        PlaceBuilders();
        RunGame();
    }

    void RunGame()
    {
        while(!Winner)
        {
            Player1.Turn();
            Player2.Turn();
        }
    }

    void PlaceBuilders()
    {
        Player1.PlaceBuilder(1);
        Player2.PlaceBuilder(1);
        Player1.PlaceBuilder(2);
        Player2.PlaceBuilder(2);
    }

    //set board back to 0
    void ClearBoard()
    {
        foreach(int i : Board){
            foreach(int)
        }
    }

    //increase a location to 0
    bool BuildLevel(int x, int y)
    {
        if(Board[x][y] < 4)
        {
            Board[x][y] = Board[x][y] + 1;
        }
        else
        {
            return false;
        }
        return true;
    }
}
