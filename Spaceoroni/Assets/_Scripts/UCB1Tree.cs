using Assets._Scripts.MCTS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UCB1Tree //: MonoBehaviour
{
    public class CoroutineWithData
    {
        public Coroutine coroutine { get; private set; }
        public List<Transition> result;
        private IEnumerator target;
        public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
        {
            this.target = target;
            coroutine = owner.StartCoroutine(Run());
            result = new List<Transition>(target.Current as IEnumerable<Transition>);
        }

        private IEnumerator Run()
        {
            yield return target.MoveNext();

        }
    }

    public class Transition
    {
        public Coordinate Builder;

        public Coordinate Move;

        public Coordinate Build;

        public long Hash;

        public Transition(Coordinate builder, Coordinate move, Coordinate build, long hash)
        {
            Builder = builder;
            Move = move;
            Build = build;
            Hash = hash;
        }

        public override string ToString() => $"Move: {Move} - Hash: {Hash}";
    }

    private class Node
    {
        public int plays;
        public int wins;
        public SimPlayer player;

        public double CurrentPlayerScore() => (double)wins / plays;

        public double ParentUCBScore(int parentPlays)
        {
            int parentWins = plays - wins;
            return UCB1(parentWins, plays, parentPlays);
        }

        private Node() { }

        public Node(SimPlayer player)
        {
            this.player = player;
            plays = 0;
            wins = 0;
        }
    }

    public UCB1Tree() { }

    private static double UCB1(double childWins, double childPlays, double parentPlays) => (childWins / childPlays) + Math.Sqrt(2f * Math.Log(parentPlays) / childPlays);

    public IEnumerator Search(Game game, int simulations, Turn currentTurn, List<Turn> turns)
    {
        if (Game.ZobristTable == null) throw new System.NullReferenceException("MCTS Search has an uninitialized zobrist table");
        SimGame g = new SimGame(game);
        Dictionary<long, Node> tree = new Dictionary<long, Node>
        {
            { g.Hash, new Node(g.CurrentPlayer) }
        };

        List<Node> path = new List<Node>();
        SimGame copy;
        List<Transition> allTransitions;
        List<Transition> transitionNoStats;
        System.Random rng = new System.Random();

        for (int i = 0; i < simulations; i++)
        {
            copy = new SimGame(g.DeepCopy());
            path.Clear();
            path.Add(tree[g.Hash]);

            List<Transition> LegalTransitions = new List<Transition>();

            while (!copy.IsGameOver())
            {
                bool running = true;
                CoroutineWithData co_data = new CoroutineWithData(game, copy.GetLegalTransitions());
                LegalTransitions = co_data.result;
                //CoroutineWithData co_data = new CoroutineWithData(game, copy.GetLegalTransitions(LegalTransitions));
                //LegalTransitions = co_data.result;

                if (LegalTransitions.Count == 0)
                {
                    break;
                }
                
                transitionNoStats = new List<Transition>();
                foreach (Transition t in LegalTransitions)
                {
                    if (!tree.ContainsKey(t.Hash))
                        transitionNoStats.Add(t);

                    Debug.Log(Coordinate.coordToString(t.Builder) + Coordinate.coordToString(t.Move) + Coordinate.coordToString(t.Build));
                }

                // SELECTION
                if (transitionNoStats.Count == 0)
                {
                    Debug.Log("Count ==0 ");
                    double bestScore = float.MinValue;
                    int parentPlays = path[path.Count - 1].plays;
                    double ucb1Score;
                    int indexOfBestTransition = 0;
                    for (int j = 0; j < LegalTransitions.Count; j++)
                    {
                        ucb1Score = tree[LegalTransitions[j].Hash].ParentUCBScore(parentPlays);
                        if (ucb1Score > bestScore)
                        {
                            bestScore = ucb1Score;
                            indexOfBestTransition = j;
                        }
                    }
                    Transition bestTransition = LegalTransitions[indexOfBestTransition];
                    copy.Transition(bestTransition);
                    path.Add(tree[bestTransition.Hash]);
                }

                // EXPANSION
                else
                {
                    Debug.Log("Expansion");
                    Transition t = transitionNoStats.RandomItem(rng);
                    copy.Transition(t);
                    Node node = new Node(copy.CurrentPlayer);

                    path.Add(node);
                    tree.Add(t.Hash, node);

                    break;
                }
            }

            // ROLLOUT
            copy.Rollout(game);
            //while (!copy.simulationRunning) is needed?

            // BACKPROP
            foreach (Node node in path)
            {
                node.plays++;
                if (copy.IsWinner(node.player))
                    node.wins++;
            }
        }

        // Simulations are over. Pick the best move, then return it.
        allTransitions = new List<UCB1Tree.Transition>();
        game.StartLegalTransitionSearch(g.GetLegalTransitions(), allTransitions);

        double worstScoreFound = double.MaxValue;
        double score;
        int bestIndex = 0;

        for (int i = 0; i < allTransitions.Count; i++)
        {
            Transition t = allTransitions[i];
            score = tree[t.Hash].CurrentPlayerScore();
            if (score < worstScoreFound)
            {
                worstScoreFound = score;
                bestIndex = i;
            }
        }
        if (allTransitions.Count == 0) yield return null;
        
        Transition tran = allTransitions[bestIndex];

        //yield return tran;

        currentTurn = new Turn(tran.Builder, tran.Build, tran.Move);

        turns.Add(currentTurn);
    }
}
