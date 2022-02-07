using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coordinate
{
    public int x { get; set; }
    public int y { get; set; }

    public Coordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Coordinate()
    {
        this.x = -1;
        this.y = -1;
    }

    public static Coordinate stringToCoord(string s)
    {
        Coordinate c = new Coordinate();
        c.x = rowToInt(s.Substring(0, 1));
        c.y = Int32.Parse(s.Substring(1, 1));
        return c;
    }

    public static string coordToString(Coordinate c)
    {
        return ((char)(c.x + 65)).ToString() + c.y.ToString();
    }

    static int rowToInt(string c)
    {
        int n = 0;
        c.ToUpper();
        n = (int)char.Parse(c);
        return (n - 65);// ;
    }

    public static bool inBounds(Coordinate c)
    {
        return (c.x < 5 && c.x > -1) && (c.y < 5 && c.y > -1);
    }
}
