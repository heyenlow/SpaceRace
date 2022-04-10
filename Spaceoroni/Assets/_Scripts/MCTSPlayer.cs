using System;
using System.Collections;
using UnityEngine;

public class MCTSPlayer : IPlayer
{
    public class CoroutineWithData
    {
        public Coroutine coroutine { get; private set; }
        public Turn result;
        private IEnumerator target;
        public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
        {
            this.target = target;
            coroutine = owner.StartCoroutine(Run());
            result = target.Current as Turn;
        }

        private IEnumerator Run()
        {
            yield return target.MoveNext();

        }
    }
    protected Turn currentTurn;
    public MCTSPlayer() { }

    public MCTSPlayer(IPlayer p)
    {
        Builder1 = new Builder(Coordinate.stringToCoord(p.getBuilderLocations().Substring(0, 2)));
        Builder2 = new Builder(Coordinate.stringToCoord(p.getBuilderLocations().Substring(2, 2)));
        state = p.state;
        ID = p.ID;
    }

    public override IEnumerator PlaceBuilder(int builder, int player, Game g)
    {
        //Coordinate builder1 = new Coordinate(0, 0);
        //Coordinate builder2 = new Coordinate(1, 1);
        //if (builder == 1) { moveBuidler(builder, builder1, g); } else { moveBuidler(builder, builder2, g); }
        // this player is now temporarily the current player
        // the rival is whichever player this isn't
        System.Random seed = new System.Random();
        System.Random rnd1 = new System.Random(seed.Next(0, (int)(int.MaxValue / 2)));
        System.Random rnd2 = new System.Random(seed.Next((int)(int.MaxValue / 2), int.MaxValue));
        Coordinate tmp = new Coordinate();
        int x, y;
        if (g.Rival.getBuilders().Item1.coord.x == -1 && g.Rival.getBuilders().Item1.coord.y == -1)
        {
            if (builder == 1)
            {
                tmp.x = rnd2.Next() % 4;
                tmp.y = rnd1.Next() % 4;
                moveBuilder(builder, tmp, g);
            }
            else if (builder == 2)
            {
                // nothing should happen here because if we're placing our second builder the opponent must have already defined at least their first builder.
                Debug.Log("This error shouldn't be happening... look in NeatPlayer.cs");
            }
        }
        else
        {
            // opponent's first builder is defined, but not guaranteed the second is defined...
            if (g.Rival.getBuilders().Item2.coord.x == -1 && g.Rival.getBuilders().Item2.coord.y == -1)
            {
                // place a builder near the opponent's first builder.
                findFreeSpots(g, g.Rival.getBuilders().Item1.getLocation().x, g.Rival.getBuilders().Item1.getLocation().y, builder);
            }
            else
            {
                x = Math.Abs((g.Rival.getBuilders().Item1.coord.x + g.Rival.getBuilders().Item2.coord.x) / 2);
                y = Math.Abs((g.Rival.getBuilders().Item1.coord.y + g.Rival.getBuilders().Item2.coord.y) / 2);
                findFreeSpots(g, x, y, builder); // neat player places second builder close to opponent's builder averaged out
            }
        }
        yield return true;
    }

    private void findFreeSpots(Game g, int x, int y, int builderID)
    {
        Coordinate tmp = new Coordinate { x = x, y = y };
        bool found1 = false;
        if (!g.locationClearOfAllBuilders(tmp))
        {
            for (int i = x - 1; (i <= x + 1) && !found1; i++)
                for (int j = y - 1; j <= y + 1; j++)
                {
                    tmp.x = i; tmp.y = j;
                    if (!Coordinate.inBounds(tmp))
                        continue;
                    if (x == i && y == j)
                        continue;

                    found1 = g.locationClearOfAllBuilders(tmp);

                    if (found1)
                    {
                        moveBuilder(builderID, tmp, g);
                        return;
                    }
                }
        }
        else
        {
            // place builder at x y
            moveBuilder(builderID, tmp, g);
            return;
        }
    }

    public override IEnumerator beginTurn(Game g)
    {
        // this is the current player
        // opp is rival
        if (state != States.Undetermined)
        {
            yield return null;
        }

        currentTurn = new Turn();
        //UCB1Tree.CoroutineWithData cd = new UCB1Tree.CoroutineWithData(this, tree.Search(tmpState, 500));
        UCB1Tree tree = new UCB1Tree();

        // start child coroutine with data
        // match syntax from previous use, but this time with a Turn object.
        // to be called on g.StartMCTSTurn()
        g.StartChildCoroutine(tree.Search(g, 200, turns, currentTurn));

        Debug.Log("MCTS move is made");
        currentTurn = turns[turns.Count - 1];
        //yield return cd.coroutine;

        //UCB1Tree.Transition t = cd.result;
        // search a new clone of the current state.
        // DEBUG Console.WriteLine(Coordinate.coordToString(t.Builder) + Coordinate.coordToString(t.Move) + Coordinate.coordToString(t.Build));

        //currentTurn = new Turn(t.Builder, t.Build, t.Move);

        //turns.Add(currentTurn);
        //yield return null;
}

public void moveBuilder(Coordinate builder, Coordinate newLoc)
{

}
}
