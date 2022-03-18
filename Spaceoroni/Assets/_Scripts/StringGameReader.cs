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
    "Player1: B1D3",
    "Player2: D1B3",
    "move and build: B1C2C3",
    "p2: B3C3D2",
    "moving up a level: D3D2C1",
    "p2: D1D0C1",
    "move to level 2: D2C1D2",
    "p2: C3D2E2",
    "p1: C2C3C2",
    "p2: D0E1D1",
    "MOVING DOWN 2 LEVELS: C1B2D3",
    "P2 BUILD ROCKET: D2C1D2",
    "BLOCK ROCKET: C3D3D2",
    "P2: C1D0C1",
    "P1: C2B2C2",
    "P2: E1E2D1",
    "P1: D3C2D3",
    "P2: E2D1E2",
    "WIN: C2C1"


    /*
    "NEAT  MOVE: D0E1D2",
    "NEAT MOVE: C0D1C2",
    "NEAT MOVE: E1E0D0",
    "NEAT MOVE: D1D0E1",
    "NEAT MOVE: E0D1C2",
    "NEAT MOVE: D0E1D2",
    "NEAT MOVE: D1E0D1",
    "NEAT MOVE: C1C0D1",
    "NEAT MOVE: B0B1A2",
    "NEAT MOVE: C0D0C1",
    "NEAT MOVE: B1A2B1",
    "NEAT MOVE: D0D1C2",
    "NEAT MOVE: A2B1C2",
    "NEAT MOVE: D1D0C1",
    "NEAT MOVE: B1A2B1",
    "NEAT MOVE: D0D1E2",
    "NEAT MOVE: A2B1A2",
    "NEAT MOVE: D1D0C1",
    "NEAT MOVE: B1A2B1",
    "NEAT MOVE: D0C0B1",
    "NEAT MOVE: A2B2A3",
    "NEAT MOVE: C0B0A1",
    "NEAT MOVE: B2B3A4",
    "NEAT MOVE: B0C0D1",
    "NEAT MOVE: B3B2A3",
    "NEAT MOVE: C0B0A1",
    "NEAT MOVE: B2B3A4",
    "NEAT MOVE: B0A0A1",
    "NEAT MOVE: B3C3B4",
    "NEAT MOVE: A0B0A1",
    "NEAT MOVE: C3B4C3",
    "NEAT MOVE: B0A0B0",
    "NEAT MOVE: B4B3A4",
    "NEAT MOVE: A0B0C1",
    "NEAT MOVE: B3C3B4",
    "NEAT MOVE: B0A0B0",
    "NEAT MOVE: C3B4C3",
    "NEAT MOVE: E1E2D3",
    "NEAT MOVE: B4B3A4",
    "NEAT MOVE: E2E1D2",
    "NEAT MOVE: B3B2A3",
    "NEAT MOVE: E1D0E1",
    "NEAT MOVE: B2B3C4",
    "NEAT MOVE: D0C0D1",
    "NEAT MOVE: B3B2A3",
    "NEAT MOVE: C0D0E1",
    "NEAT MOVE: B2B3C4",
    "NEAT MOVE: D0C0B0",
    "NEAT MOVE: B3B2C3",
    "NEAT MOVE: C0D0E1",
    "NEAT MOVE: B2B3C4",
    "NEAT MOVE: D0C0B0",
    "NEAT MOVE: B3B2C3",
    "NEAT MOVE: C0D0C0",
    "NEAT MOVE: B2B3C4",
    "NEAT MOVE: D0C0D0",
    "NEAT MOVE: B3B2A2",
    "NEAT MOVE: C0D0C0",
    "NEAT MOVE: B2B3A2",
    "NEAT MOVE: D0C0D0",
    "NEAT MOVE: B3B2B3",
    "NEAT MOVE: C0D0"
    */
    };

    public static int BlastOffMove = 13;
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
                gameLines = tutorialGameLines;//watchGameLines;
                break;
        }
        player1builder1Location = Coordinate.stringToCoord(gameLines[0].Substring(gameLines[0].LastIndexOf(' ') + 1));
        player1builder2Location = Coordinate.stringToCoord(gameLines[0].Substring(gameLines[0].LastIndexOf(' ') + 3));
        player2builder1Location = Coordinate.stringToCoord(gameLines[1].Substring(gameLines[1].LastIndexOf(' ') + 1));
        player2builder2Location = Coordinate.stringToCoord(gameLines[1].Substring(gameLines[1].LastIndexOf(' ') + 3));
    }
    public static int lengthOfTutorialMoves() { return tutorialGameLines.Length - 2; }
}
