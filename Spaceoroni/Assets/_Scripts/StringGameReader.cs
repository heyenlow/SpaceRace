using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringGameReader
{
    static string[] lines = System.IO.File.ReadAllLines(@".\Assets\_Scripts\TwoPlayerGame.txt");
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
