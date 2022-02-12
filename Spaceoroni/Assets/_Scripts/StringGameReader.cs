using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringGameReader
{
    static string[] lines = System.IO.File.ReadAllLines(@"C:\Users\Team03\Documents\GitHub\SpaceRace\Spaceoroni\Assets\_Scripts");
    public static int MoveCount = 0;
    public static Coordinate builder1Location = Coordinate.stringToCoord(lines[0].Substring(lines[0].IndexOf(' ')));
    public static Coordinate builder2Location = Coordinate.stringToCoord(lines[0].Substring(lines[1].IndexOf(' ')));

    public static string getMoves()
    {
        string line = lines[MoveCount + 2];
        return line.Substring(line.IndexOf(' '));
    }

}
