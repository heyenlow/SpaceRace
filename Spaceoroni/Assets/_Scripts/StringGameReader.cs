using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringGameReader
{
    //TODO: SEPARTE THE FILE IN LINES
    static TextAsset myfile = (TextAsset)Resources.Load("TwoPlayerGame");
    
    static string[] lines = {
        "Player1: C0A0",
        "Player2: B0B1",
        "NEAT MOVE: A0A1B2",
        "NEAT MOVE: B1B2C1",
        "NEAT MOVE: A1B1C2",
        "NEAT MOVE: B2C3D4",
        "NEAT MOVE: B1C2D1",
        "NEAT MOVE: C3D4E3",
        "NEAT MOVE: C2D3E2",
        "NEAT MOVE: D4E3D4",
        "NEAT MOVE: D3E4D3",
        "NEAT MOVE: E3D4E3",
        "NEAT MOVE: E4D3E2",
        "NEAT MOVE: D4E3D4",
        "NEAT MOVE: D3E4D3",
        "NEAT MOVE: E3D4"
    };

    public static int MoveCount = 0;
    public static Coordinate player1builder1Location = Coordinate.stringToCoord(lines[0].Substring(lines[0].IndexOf(' ') + 1));
    public static Coordinate player1builder2Location = Coordinate.stringToCoord(lines[0].Substring(lines[0].IndexOf(' ') + 3));
    public static Coordinate player2builder1Location = Coordinate.stringToCoord(lines[1].Substring(lines[1].IndexOf(' ') + 1));
    public static Coordinate player2builder2Location = Coordinate.stringToCoord(lines[1].Substring(lines[1].IndexOf(' ') + 3));

    public static string getMoves()
    {
        string line = lines[MoveCount++ + 2];
        return line.Substring(line.LastIndexOf(' ')+1);
    }

    public static Tuple<Coordinate,Coordinate> BuilderLocation(int player)
    {
        if (player == 1) return new Tuple<Coordinate, Coordinate>(player1builder1Location, player1builder2Location);
        return new Tuple<Coordinate, Coordinate>(player2builder1Location, player2builder2Location);
    }
}
