using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringGameReader
{
    //TODO: SEPARTE THE FILE IN LINES
    //static TextAsset myfile = (TextAsset)Resources.Load("TwoPlayerGame");
    public static string[] gameLines;
    static string[] watchGameLines = {
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

    static string[] tutorialGameLines =
    {
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
    //should be 1 for tutorial and 2 for watch -- relates to how many players
    public static Coordinate player1builder1Location;
    public static Coordinate player1builder2Location; 
    public static Coordinate player2builder1Location; 
    public static Coordinate player2builder2Location; 

    public static Turn getCurrentTurn()
    {
        string currentLine = gameLines[MoveCount++ + 2];
        return new Turn(currentLine.Substring(currentLine.LastIndexOf(' ')+1));
    }

    public static Tuple<Coordinate,Coordinate> BuilderLocation(int player)
    {
        if (player == 1) return new Tuple<Coordinate, Coordinate>(player1builder1Location, player1builder2Location);
        return new Tuple<Coordinate, Coordinate>(player2builder1Location, player2builder2Location);
    }

    public static void setGameLines()
    {
        switch (GameSettings.gameType)
        {
            case GameSettings.GameType.Tutorial:
                gameLines = tutorialGameLines;
                break;
            case GameSettings.GameType.Watch:
                gameLines = watchGameLines;
                break;
        }
        player1builder1Location = Coordinate.stringToCoord(gameLines[0].Substring(gameLines[0].LastIndexOf(' ') + 1));
        player1builder2Location = Coordinate.stringToCoord(gameLines[0].Substring(gameLines[0].LastIndexOf(' ') + 3));
        player2builder1Location = Coordinate.stringToCoord(gameLines[1].Substring(gameLines[1].LastIndexOf(' ') + 1));
        player2builder2Location = Coordinate.stringToCoord(gameLines[1].Substring(gameLines[1].LastIndexOf(' ') + 3));
    }
}
