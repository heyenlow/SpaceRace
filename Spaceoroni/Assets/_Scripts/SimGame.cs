using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimGame : MonoBehaviour
{
    static long[,,] ZobristTable = null;
    public int[,] state
    {
        get
        {
            return Board;
        }
        set { }
    }

    public enum PlayerState
    {
        Winner,
        Loser,
        Playing
    };

    int[,] Board;
    long Hash;
    MCTSPlayer player1, player2;
    public SimGame() { }
    public SimGame(Game g)
    {
        this.Board = g.Board;
        player1 = new MCTSPlayer (g.Player1);
        player2 = new MCTSPlayer(g.Player2);
        Hash = g.Hash;
    }

    public PlayerState playerState { get; set; }
    public SimIPlayer Player1;
    public SimIPlayer Player2;
    public int moveNum = 0;
    public int timeToTurn = 2;

    public SimIPlayer CurrentPlayer => (moveNum % 2 == 0) ? Player1 : Player2;
    public SimIPlayer Rival => (moveNum % 2 == 0) ? Player2 : Player1;
    public static bool cancelTurn = false;

    public bool processTurnString(string turn)
    {
        // when a null build location is chosen, the string is "".
        // string is also "" if nothing has happened...
        // sooooo......
        if (turn.Equals(string.Empty))
        {
            CurrentPlayer.state = SimIPlayer.States.Loser;
            Rival.state = SimIPlayer.States.Winner;
            return false;
        }
        string builderCoord = turn.Substring(0, 2);
        Coordinate builderLoc = Coordinate.stringToCoord(builderCoord);
        SimBuilder builder = null;

        // get builder represented by first 2 characters
        if (CurrentPlayer.getBuilderLocations().Contains(Coordinate.coordToString(builderLoc)))
        {
            // checks if first 2 characters of string correlate to the location of one of curPlayer's builders
            if (CurrentPlayer.Builder1.getLocation() == Coordinate.coordToString(builderLoc))
            {
                // MOVE CORRESPONDING BUILDER1
                //curPlayer.Builder1.move(builderLoc); <-- what the heck was this?
                builder = CurrentPlayer.Builder1;
            }
            else if (CurrentPlayer.Builder2.getLocation() == Coordinate.coordToString(builderLoc))
            {
                // MOVE CORRESPONDING BUILDER2
                //curPlayer.Builder2.move(Coordinate.stringToCoord(Coordinate.coordToString(builderLoc))); <-- what the heck was this?
                builder = CurrentPlayer.Builder2;
            }
        }
        else
        {
            // error, no builder found matching first location.
            throw new System.ArgumentOutOfRangeException("NO BUILDER FOUND MATCHING FIRST LOCATION");
        }
        string mv = turn.Substring(2, 2);
        Coordinate move = Coordinate.stringToCoord(mv);
        // need to check if the proposed move is valid.
        if (!Coordinate.inBounds(move) && !(builder.Location.x + 1 >= move.x && builder.Location.x - 1 <= move.x &&
                                            builder.Location.y + 1 >= move.y && builder.Location.y - 1 <= move.y))
        {
            // if user passes a move that is out of bounds or something similar, it's a loser.
            throw new ArgumentOutOfRangeException("INVALID MOVE INTERCEPTED");
        }
        builder.move(move);
        // builder has been moved.
        // did the move result in a win? out parameter isOver can help with that?

        if (state[move.x, move.y] == 3)
        {
            CurrentPlayer.state = SimIPlayer.States.Winner;
            Rival.state = SimIPlayer.States.Loser;
            return true;
        }
        // If not over Where to build?
        Coordinate build = Coordinate.stringToCoord(turn.Substring(4, 2));

        BuildLevel(build);

        // build should be completed?
        return true;
    }

    void BuildLevel(Coordinate c)
    {
        if (Board[c.x, c.y] + 1 >= 5) throw new ArgumentOutOfRangeException("BuildLevel() tried to build too high");
        Board[c.x, c.y] += 1;
    }

    public long computeHash()
    {
        char[,] Board = TranslateState();
        long h = 0;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (Board[i, j] != '-')
                {
                    int piece = indexOf(Board[i, j]);
                    h ^= ZobristTable[i, j, piece];
                }
            }
        }
        return h;
    }

    public char[,] TranslateState()
    {
        char[,] ret = new char[5, 5];

        // takes the Game g.state int matrix and converts it to a char matrix with representations for builders on board
        Coordinate tmp = new Coordinate();
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                tmp.x = i; tmp.y = j;
                if (locationClearOfAllBuilders(tmp))
                {
                    ret[i, j] = (char)(state[i, j] + 48);
                }
                else
                {
                    char piece = '0';
                    switch (state[i, j])
                    {
                        case 0:
                            if (Equals(tmp, Coordinate.stringToCoord(Player1.getBuilderLocations().Substring(0, 2)))) piece = 'A';
                            else if (Equals(tmp, Coordinate.stringToCoord(Player1.getBuilderLocations().Substring(2, 2)))) piece = 'E';
                            else if (Equals(tmp, Coordinate.stringToCoord(Player2.getBuilderLocations().Substring(0, 2)))) piece = 'a';
                            else if (Equals(tmp, Coordinate.stringToCoord(Player2.getBuilderLocations().Substring(2, 2)))) piece = 'e';
                            break;
                        case 1:
                            if (Equals(tmp, Coordinate.stringToCoord(Player1.getBuilderLocations().Substring(0, 2)))) piece = 'B';
                            else if (Equals(tmp, Coordinate.stringToCoord(Player1.getBuilderLocations().Substring(2, 2)))) piece = 'F';
                            else if (Equals(tmp, Coordinate.stringToCoord(Player2.getBuilderLocations().Substring(0, 2)))) piece = 'b';
                            else if (Equals(tmp, Coordinate.stringToCoord(Player2.getBuilderLocations().Substring(2, 2)))) piece = 'f';
                            break;
                        case 2:
                            if (Equals(tmp, Coordinate.stringToCoord(Player1.getBuilderLocations().Substring(0, 2)))) piece = 'C';
                            else if (Equals(tmp, Coordinate.stringToCoord(Player1.getBuilderLocations().Substring(2, 2)))) piece = 'G';
                            else if (Equals(tmp, Coordinate.stringToCoord(Player2.getBuilderLocations().Substring(0, 2)))) piece = 'c';
                            else if (Equals(tmp, Coordinate.stringToCoord(Player2.getBuilderLocations().Substring(2, 2)))) piece = 'g';
                            break;
                        case 3:
                            if (Equals(tmp, Coordinate.stringToCoord(Player1.getBuilderLocations().Substring(0, 2)))) piece = 'D';
                            else if (Equals(tmp, Coordinate.stringToCoord(Player1.getBuilderLocations().Substring(2, 2)))) piece = 'H';
                            else if (Equals(tmp, Coordinate.stringToCoord(Player2.getBuilderLocations().Substring(0, 2)))) piece = 'd';
                            else if (Equals(tmp, Coordinate.stringToCoord(Player2.getBuilderLocations().Substring(2, 2)))) piece = 'h';
                            break;
                    }
                    ret[i, j] = piece;
                }
            }

        }

        return ret;
    }

    public bool locationClearOfAllBuilders(Coordinate c)
    {
        return !getAllBuildersString().Contains(Coordinate.coordToString(c));
    }

    string getAllBuildersString()
    {
        return Player1.getBuilderLocations() + Player2.getBuilderLocations();
    }

    static int indexOf(char piece)
    {// this node's character array `state` uses letters to represent builders at different heights
     // convert these letters numbers we can use for easy height calculations and such
        switch (piece)
        {
            case 'A':               // player 1 builder 1 height 0
                return 0;
            case 'B':
                return 1;
            case 'C':
                return 2;
            case 'D':               // player 1 builder 1 height 3
                return 3;
            case 'E':               // player 1 builder 2 height 0
                return 4;
            case 'F':
                return 5;
            case 'G':
                return 6;
            case 'H':               // player 1 builder 2 height 3
                return 7;
            case 'a':               // player 2 builder 1 height 0
                return 8;
            case 'b':
                return 9;
            case 'c':
                return 10;
            case 'd':               // player 2 builder 1 height 3
                return 11;
            case 'e':               // player 2 builder 2 height 0
                return 12;
            case 'f':
                return 13;
            case 'g':
                return 14;
            case 'h':               // player 2 builder 2 height 3
                return 15;
            case '0':               // player - height 0
                return 16;
            case '1':
                return 17;
            case '2':
                return 18;
            case '3':
                return 19;
            case '4':               // player - height 4
                return 20;
            default:
                return -1;
        }
    }

}
