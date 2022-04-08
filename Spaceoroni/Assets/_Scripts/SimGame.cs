using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimGame : MonoBehaviour
{
    int[,] Board;
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

    public PlayerState playerState { get; set; }
    public SimIPlayer Player1;
    public SimIPlayer Player2;
    public int moveNum = 0;
    public int timeToTurn = 2;
    public long Hash;

    public SimIPlayer CurrentPlayer => (moveNum % 2 == 0) ? Player1 : Player2;
    public SimIPlayer Rival => (moveNum % 2 == 0) ? Player2 : Player1;
    public static bool cancelTurn = false;

    void PlaceBuilders()
    {
        PlaceBuilder(Player1, 1);
        PlaceBuilder(Player1, 2);
        moveNum++; // increment move to flip CurrentPlayer and Rival properties.
        PlaceBuilder(Player2, 1);
        PlaceBuilder(Player2, 2);
        moveNum--; // decrement moveNum back to 0 so that the game begins as normal with Player1
    }

    public SimGame(SimGame g)
    {
        Board = new int[5, 5];
        System.Array.Copy(g.state, g.state.GetLowerBound(0), Board, Board.GetLowerBound(0), 25);
        Player1 = new MCTSPlayer(g.Player1);
        Player2 = new MCTSPlayer(g.Player2);

    }

    public void PlaceBuilder(IPlayer p, int i)
    {
        if (p is Player)
        {
            string s;
            do
            {
                Console.Write(p.ToString() + " place Builder " + i + ": ");
                s = Console.ReadLine().ToUpper();
            }
            while (getAllBuildersString().Contains(s) || !Coordinate.inBounds(Coordinate.stringToCoord(s)));

            p.PlaceBuilder(i, Coordinate.stringToCoord(s));
        }
        else if (p is MMPlayer)
        {
            MMPlayer mmp = (MMPlayer)p;
            mmp.placeBuilders(this, i);
        }
        else if (p is NeatPlayer)
        {
            // ASSUMES NEAT PLAYER IS GOING SECOND
            NeatPlayer np = (NeatPlayer)p;
            np.placeBuilders(this, i);
        }
        else
        {
            MCTSPlayer pltmp = (MCTSPlayer)p;
            pltmp.placeBuilders(this, i);
        }
    }


    public SimIPlayer PlayGameToEnd()
    {
        SimIPlayer winner = null;

        PlaceBuilders();

        // Play until we have a winner or tie?
        for (; winner == null; moveNum++)
        {
            // Check if last turn lost?
            if (Rival.state == SimIPlayer.States.Loser)
            {
                winner = CurrentPlayer;
            }
            else
            {
                if (hasPossibleTurns(CurrentPlayer))
                {

                    //if it is a string player we just need to wait before getting the turn and processing it
                    if (curPlayer is StringPlayer)
                    {
                        yield return new WaitForSeconds(timeToTurn);
                    }
                    else //if it is any other player we need to start getting the turn
                    {
                        curPlayer.beginTurn(this);
                    }

                    Turn t = curPlayer.getNextTurn();
                    // update the board with the current player's move
                    Debug.Log("Processing Turn: " + t.ToString());

                    processTurnString(t, curPlayer, this);

                    if (t.isWin)
                    {
                        built = true;
                        winner = curPlayer;
                        //wait to move then set buidler inactive
                        yield return new WaitForSeconds(0.6f);
                        curPlayer.setBuilderAtLocationInactive(t.MoveLocation);

                        yield return new WaitForSeconds(2);
                        BlastOffRocket(t.MoveLocation);
                    }
                }
                else { winner = othPlayer; }

                if (winner != null)
                {
                    if (winner == Player1) { WinText.text = "You Win!"; }
                    else { WinText.text = "Better Luck Next Time"; }
                    goToEndOfGameScreen();
                }

                //Check if win
                if (playerState == PlayerState.Winner)
                {
                    winner = curPlayer;
                }
            }
        }

        return winner;
    }

    internal Game DeepCopy()
    {
        throw new System.NotImplementedException();
    }

    private bool hasPossibleTurns(IPlayer p)
    {
        Coordinate BuilderOneLocation = Coordinate.stringToCoord(p.getBuilderLocations().Substring(0, 2));
        Coordinate BuilderTwoLocation = Coordinate.stringToCoord(p.getBuilderLocations().Substring(2, 2));

        var builderOneMoves = getAllPossibleMoves(BuilderOneLocation);
        foreach (var coord in builderOneMoves)
        {
            if (getAllPossibleBuilds(Coordinate.stringToCoord(coord)).Count > 0) return true;
        }

        var builderTwoMoves = getAllPossibleMoves(BuilderTwoLocation);
        foreach (var coord in builderTwoMoves)
        {
            if (getAllPossibleBuilds(Coordinate.stringToCoord(coord)).Count > 0) return true;
        }

        return false;
    }

    internal List<UCB1Tree.Transition> GetLegalTransitions()
    {
        throw new System.NotImplementedException();
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
                if (Coordinate.inBounds(test) && Board[test.x, test.y] <= (Board[c.x, c.y] + 1) && Board[test.x, test.y] < 4 && locationClearOfAllBuilders(test))
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
                if (Coordinate.inBounds(test) && Board[test.x, test.y] < 4 && locationClearOfAllBuilders(test) && !Coordinate.Equals(test, c))
                {
                    allBuilds.Add(Coordinate.coordToString(test));
                }
            }
        }

        return allBuilds;
    }
}
