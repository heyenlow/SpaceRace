using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimGame //: MonoBehaviour
{
    char[,] Board;
    public int[,] state;

    public SimPlayer Player1;
    public SimPlayer Player2;
    public int moveNum = 0;
    public int timeToTurn = 2;
    public long Hash => computeHash();

    public SimPlayer CurrentPlayer => (moveNum % 2 == 0) ? Player1 : Player2;
    public SimPlayer Rival => (moveNum % 2 == 0) ? Player2 : Player1;
    public static bool cancelTurn = false;

    void PlaceBuilders()
    {
        Player1.PlaceBuilders();
        moveNum++; // increment move to flip CurrentPlayer and Rival properties.
        Player2.PlaceBuilders();
        moveNum--; // decrement moveNum back to 0 so that the game begins as normal with Player1
    }

    public SimGame(SimGame g)
    {
        Board = new char[5, 5];
        Board = g.TranslateState();
        
        state = new int[5, 5];
        System.Array.Copy(g.state, g.state.GetLowerBound(0), state, state.GetLowerBound(0), 25);
        
        Player1 = new SimPlayer(g.Player1);
        Player2 = new SimPlayer(g.Player2);
        
        moveNum = g.moveNum;
    }

    public SimGame(Game g)
    {
        Board = new char[5, 5];
        Board = g.TranslateState();
        
        state = new int[5, 5];
        System.Array.Copy(g.state, g.state.GetLowerBound(0), state, state.GetLowerBound(0), 25);

        Player1 = new SimPlayer(g.Player1);
        Player1.ID = 0;
        Player2 = new SimPlayer(g.Player2);
        Player2.ID = 1;
        
        moveNum = g.moveNum;
    }


    public IEnumerable<SimPlayer> PlayGameToEnd()
    {
        SimPlayer winner = null;

        PlaceBuilders();


        yield return winner;
    }

    internal SimGame DeepCopy()
    {
        return new SimGame(this);

    }

    public List<UCB1Tree.Transition> GetLegalTransitions()
    {
        List<UCB1Tree.Transition> ret = new List<UCB1Tree.Transition>();
        if (CurrentPlayer is null) throw new System.NullReferenceException();
        // get all possible moves from each builder including new build locations....
        // builder 1...
        List<string> moves = getAllPossibleMoves(CurrentPlayer.Builder1.Location);
        for (int i = 0; i < moves.Count; i++)
        {
            if (Equals(CurrentPlayer.Builder1.Location, Coordinate.stringToCoord(moves[i])))
                Debug.Log("Break"); // TODO: error!
            List<string> builds = getAllPossibleBuilds(Coordinate.stringToCoord(moves[i]), CurrentPlayer.Builder1.Location);
            for (int j = 0; j < builds.Count; j++)
            {
                // create transition representing this possible move
                UCB1Tree.Transition tmp = new UCB1Tree.Transition(CurrentPlayer.Builder1.Location, Coordinate.stringToCoord(moves[i]), Coordinate.stringToCoord(builds[j]), Hash);
                // Hash Values are currently this current game's state hash

                // need to change it to be the hash value of a game resulting from the move taking place.  without actually making the move?
                var tmpState = new SimGame(DeepCopy());
                string turnString = Coordinate.coordToString(tmp.Builder) + Coordinate.coordToString(tmp.Move) + Coordinate.coordToString(tmp.Build); // create turn string from this transition
                tmpState.processTurnString(turnString); // execute transition on copy board
                
                tmpState.computeHash(); // hopefully update hash
                tmp.Hash = tmpState.Hash; // this hash is the hash of the copy's current state

                ret.Add(tmp);
            }
        }
        // builder 2...
        moves = getAllPossibleMoves(CurrentPlayer.Builder2.Location);
        for (int i = 0; i < moves.Count; i++)
        {
            if (Equals(CurrentPlayer.Builder2.Location, moves[i]))
                Debug.Log("Break");

            List<string> builds = getAllPossibleBuilds(Coordinate.stringToCoord(moves[i]), CurrentPlayer.Builder2.Location);
            for (int j = 0; j < builds.Count; j++)
            {
                UCB1Tree.Transition tmp = new UCB1Tree.Transition(CurrentPlayer.Builder2.Location, Coordinate.stringToCoord(moves[i]), Coordinate.stringToCoord(builds[j]), Hash);
                // Hash Values are currently this current game's state hash
                // need to change it to be the hash value of a game resulting from the move taking place.  without actually making the move?
                SimGame tmpState = DeepCopy();
                tmpState.processTurnString(Coordinate.coordToString(tmp.Builder) + Coordinate.coordToString(tmp.Move) + Coordinate.coordToString(tmp.Build));
                tmp.Hash = tmpState.computeHash();

                ret.Add(tmp);
            }
        }
        if (ret.Count == 0)
        {
            CurrentPlayer.state = SimIPlayer.States.Loser; // if you can't move you're a loser.
            Rival.state = SimIPlayer.States.Winner;
        }
        return ret;
    }

    /// <summary>
    /// returns the 2D character matrix representing the current game board for both players.
    /// </summary>
    /// <returns></returns>
    public char[,] TranslateState()
    {
        // TODO: this function isn't returning the right array
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
                    switch (state[i, j])
                    {
                        case 0:
                            if (Coordinate.Equals(tmp, Player1.Builder1.Location)) ret[i, j] = 'A';
                            else if (Coordinate.Equals(tmp, Player1.Builder2.Location)) ret[i, j] = 'E';
                            else if (Coordinate.Equals(tmp, Player2.Builder1.Location)) ret[i, j] = 'a';
                            else if (Coordinate.Equals(tmp, Player2.Builder2.Location)) ret[i, j] = 'e';
                            break;
                        case 1:
                            if (Coordinate.Equals(tmp, Player1.Builder1.Location)) ret[i, j] = 'B';
                            else if (Coordinate.Equals(tmp, Player1.Builder2.Location)) ret[i, j] = 'F';
                            else if (Coordinate.Equals(tmp, Player2.Builder1.Location)) ret[i, j] = 'b';
                            else if (Coordinate.Equals(tmp, Player2.Builder2.Location)) ret[i, j] = 'f';
                            break;
                        case 2:
                            if (Coordinate.Equals(tmp, Player1.Builder1.Location)) ret[i, j] = 'C';
                            else if (Coordinate.Equals(tmp, Player1.Builder2.Location)) ret[i, j] = 'G';
                            else if (Coordinate.Equals(tmp, Player2.Builder1.Location)) ret[i, j] = 'c';
                            else if (Coordinate.Equals(tmp, Player2.Builder2.Location)) ret[i, j] = 'g';
                            break;
                        case 3:
                            if (Coordinate.Equals(tmp, Player1.Builder1.Location)) ret[i, j] = 'D';
                            else if (Coordinate.Equals(tmp, Player1.Builder2.Location)) ret[i, j] = 'H';
                            else if (Coordinate.Equals(tmp, Player2.Builder1.Location)) ret[i, j] = 'd';
                            else if (Coordinate.Equals(tmp, Player2.Builder2.Location)) ret[i, j] = 'h';
                            break;
                    }
                }
            }

        }

        return ret;
    }

    public long computeHash()
    {
        if (Game.ZobristTable == null) return (long)0;
        Board = TranslateState();
        long h = 0;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (Board[i, j] != '-')
                {
                    int piece = indexOf(Board[i, j]);
                    h ^= Game.ZobristTable[i, j, piece];
                }
            }
        }
        return h;
    }

    /// <summary>
    /// THIS FUNCTION IS ONLY USED AS A HELPER FOR ZOBRIST HASHING
    /// </summary>
    /// <param name="piece"></param>
    /// <returns></returns>
    static int indexOf(char piece)
    {// this node's character array `state` uses letters to represent builders at different heights
     // convert these letters numbers we can use for easy height calculations and such
        switch (piece)
        {
            case 'A':               // minMax player builder 1 height 0
                return 0;
            case 'B':
                return 1;
            case 'C':
                return 2;
            case 'D':               // minmax player builder 1 height 3
                return 3;
            case 'E':               // minmax player builder 2 height 0
                return 4;
            case 'F':
                return 5;
            case 'G':
                return 6;
            case 'H':               // minmax player builder 2 height 3
                return 7;
            case 'a':               // opponent builder 1 height 0
                return 8;
            case 'b':
                return 9;
            case 'c':
                return 10;
            case 'd':               // opponent builder 1 height 3
                return 11;
            case 'e':               // opponent builder 2 height 0
                return 12;
            case 'f':
                return 13;
            case 'g':
                return 14;
            case 'h':               // opponent builder 2 height 3
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

    /// <summary>
    ///  Method takes in a string representing a turn
    ///  First 2 chars are the builder being selected (make sure it is valid player moving proper builder etc)
    /// </summary>
    /// <param name="turn"></param>
    /// <param name="curPlayer"></param>
    /// <param name="isOver">out variable to be used as a flag, indicating if an emergency stop is necessary to end the game.</param>
    /// <returns>bool did the Turn get processed successfully?</returns>
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
            throw new System.ArgumentOutOfRangeException("INVALID MOVE INTERCEPTED");
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
        if (state[c.x, c.y] + 1 >= 5) throw new System.ArgumentOutOfRangeException("BuildLevel() tried to build too high");
        state[c.x, c.y] += 1;
    }

    //disraguards the location of the current builder. this is used when looking for possible builds since we are moving after we look for possible moves
    public bool locationClearOfAllOtherBuilders(Coordinate c, Coordinate currBuilderLocation)
    {
        string s = getAllBuildersString();
        int i = s.IndexOf(Coordinate.coordToString(currBuilderLocation));
        s = s.Remove(i, 2);
        return !s.Contains(Coordinate.coordToString(c));
    }

    //returns a string of all the coords someone in that position could build in
    public List<string> getAllPossibleBuilds(Coordinate c, Coordinate buildersOldLocation)
    {
        List<string> allBuilds = new List<string>();
        for (int i = -1; i <= 1; ++i)
        {
            for (int j = -1; j <= 1; ++j)
            {
                Coordinate test = new Coordinate(c.x + i, c.y + j);
                if (Coordinate.inBounds(test) && Board[test.x, test.y] <= 3 && locationClearOfAllOtherBuilders(test, buildersOldLocation) && !(test.x == c.x && test.y == c.y))
                {
                    allBuilds.Add(Coordinate.coordToString(test));
                }
            }
        }

        //Console.WriteLine("All possible builds: ");
        //foreach (var s in allBuilds) {
        //    Console.WriteLine(s);
        //}
        if (allBuilds.Count < 1) allBuilds.Add(Coordinate.coordToString(buildersOldLocation));
        if (allBuilds.TrueForAll((pos) => { Coordinate w = Coordinate.stringToCoord(pos); return state[w.x, w.y] + 1 <= 4; }))
        {
            // do nothing
        }
        else
        {
            // build detected out of bounds... investigate?
            Debug.Log("Investigate....");
        }
        return allBuilds;
    }

    System.Tuple<string, string> getAllBuilderLocations()
    {
        return new System.Tuple<string, string>(Player1.getBuilderLocations(), Player2.getBuilderLocations());
    }

    /// <summary>
    /// Returns true if the current gamestate is a terminal (game-over) gamestate. Returns false if there are still possible moves to perform.
    /// </summary>
    /// <returns></returns>
    public bool IsGameOver()
    {
        // DEBUG
        if (CurrentPlayer.state != SimIPlayer.States.Undetermined)
        {
            return true;
        }
        else if (Rival.state != SimIPlayer.States.Undetermined)
        {
            return true;
        }
        return false;
    }

    // PULLS 4 BUILDER LOCATIONS and returns string ie. A2B3C4D3
    string getAllBuildersString()
    {
        var b = getAllBuilderLocations();
        return b.Item1 + b.Item2;
    }

    //makes sure there are no builders in that location
    public bool locationClearOfAllBuilders(Coordinate c)
    {
        return !getAllBuildersString().Contains(Coordinate.coordToString(c));
    }

    public List<string> getAllPossibleMoves(Coordinate c)
    {
        List<string> allMoves = new List<string>();
        for (int i = -1; i <= 1; ++i)
        {
            for (int j = -1; j <= 1; ++j)
            {
                Coordinate test = new Coordinate(c.x + i, c.y + j);
                //isValidMove Function candidate
                if (Coordinate.inBounds(test) && state[test.x, test.y] <= (state[c.x, c.y] + 1) && state[test.x, test.y] < 4 && locationClearOfAllBuilders(test))
                {
                    allMoves.Add(Coordinate.coordToString(test));
                }
            }
        }

        return allMoves;
    }

    public List<string> getAllPossibleBuilds(Coordinate c)
    {
        List<string> allBuilds = new List<string>();
        for (int i = -1; i <= 1; ++i)
        {
            for (int j = -1; j <= 1; ++j)
            {
                Coordinate test = new Coordinate(c.x + i, c.y + j);
                if (Coordinate.inBounds(test) && state[test.x, test.y] < 4 && locationClearOfAllBuilders(test) && !Coordinate.Equals(test, c))
                {
                    allBuilds.Add(Coordinate.coordToString(test));
                }
            }
        }

        return allBuilds;
    }

    /// <summary>
    /// Execute the given move on the internal game state and update its hash with the given value. It is parameterized over the client-defined transition type.
    /// </summary>
    /// <param name="t"></param>
    public void Transition(UCB1Tree.Transition t)
    {
        var tmpBuilder = new Coordinate(t.Builder);
        bool isWin = false;
        if (!Equals(CurrentPlayer.Builder1.Location, t.Builder) && !Equals(CurrentPlayer.Builder2.Location, t.Builder)) throw new System.ArgumentOutOfRangeException();
        string turn = Coordinate.coordToString(t.Builder) + Coordinate.coordToString(t.Move) + Coordinate.coordToString(t.Build);
        if (!processTurnString(turn))
        {
            throw new System.NullReferenceException();
        }
        else
        {
            moveNum++;
            t.Builder = tmpBuilder;
            if (isWin)
            {
                CurrentPlayer.state = SimIPlayer.States.Winner;
                Rival.state = SimIPlayer.States.Loser;
            }
        }
    }

    /// <summary>
    /// attempts to finish playing a game based on a copy that was made from DeepCopy()
    /// </summary>
    public void Rollout()
    {
        SimIPlayer winner = null;
        System.Random rnd = new System.Random();
        for (; winner == null; moveNum++)
        {
            if (CurrentPlayer is null) throw new System.NullReferenceException();
            // string turn BUILDERMOVEBUILD string
            var turn = string.Empty;

            var allPossibleMoves = GetLegalTransitions();
            if (allPossibleMoves.Count == 0)
            {
                winner = Rival;
                CurrentPlayer.state = SimIPlayer.States.Loser;
                Rival.state = SimIPlayer.States.Winner;
                break;
            }
            var randomMove = allPossibleMoves[rnd.Next(0, allPossibleMoves.Count)];
            turn = Coordinate.coordToString(randomMove.Builder) + Coordinate.coordToString(randomMove.Move) + Coordinate.coordToString(randomMove.Build);

            bool won = isWin(randomMove.Move);
            if (won)
            {
                CurrentPlayer.state = SimIPlayer.States.Winner;
                Rival.state = SimIPlayer.States.Loser;
                winner = CurrentPlayer;
                break;
            }

            processTurnString(turn);

            if (CurrentPlayer.state == SimIPlayer.States.Winner)
            {
                Rival.state = SimIPlayer.States.Loser;
                winner = CurrentPlayer;
            }
            else if (CurrentPlayer.state == SimIPlayer.States.Loser)
            {
                Rival.state = SimIPlayer.States.Winner;
                winner = Rival;
            }

        }
    }

    public bool isWin(Coordinate c)
    {
        if (ReferenceEquals(c, null))
        {
            return true; // if this function is passed a null move, it means a win did occur.  however the object that called this function is the loser....
        } // conflicting comments :x
        if (c.x == -1 && c.y == -1) return false; //if null move gets passed for any reason, it's not a winning move.
        return Board[c.x, c.y] == 3;
    }

    /// <summary>
    /// Returns whether the input player is victorious in the current gamestate. It is parameterized over the client-defined player type.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool IsWinner(SimIPlayer player)
    {
        SimIPlayer winner;
        if (Player1.state == SimIPlayer.States.Winner) winner = Player1;
        else if (Player2.state == SimIPlayer.States.Winner) winner = Player2;
        else if (Player1.state == SimIPlayer.States.Loser) winner = Player2;
        else if (Player2.state == SimIPlayer.States.Loser) winner = Player1;
        else winner = null;

        if (winner != null && player.ID == winner.ID) return true;

        return false;
    }
}
