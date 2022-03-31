using Assets._Scripts.MCTS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UCB1Tree : MonoBehaviour
{
    public class CoroutineWithData
    {
        public Coroutine coroutine { get; private set; }
        public Transition result;
        private IEnumerator target;
        public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
        {
            this.target = target;
            this.coroutine = owner.StartCoroutine(Run());
        }

        private IEnumerator Run()
        {
            while (target.MoveNext())
            {
                result = (Transition)target.Current;
                yield return result;
            }
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
        public IPlayer player;

        public double CurrentPlayerScore() => (double)wins / plays;

        public double ParentUCBScore(int parentPlays)
        {
            int parentWins = plays - wins;
            return UCB1(parentWins, plays, parentPlays);
        }

        private Node() { }

        public Node(IPlayer player)
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
        Dictionary<long, Node> tree = new Dictionary<long, Node>
        {
            { game.Hash, new Node(game.CurrentPlayer) }
        };
        List<Node> path = new List<Node>();
        Game copy;
        List<Transition> allTransitions;
        List<Transition> transitionNoStats;
        System.Random rng = new System.Random();

        for (int i = 0; i < simulations; i++)
        {
            copy = game.DeepCopy();
            path.Clear();
            path.Add(tree[game.Hash]);

            while (!copy.IsGameOver())
            {
                allTransitions = copy.GetLegalTransitions();
                if (allTransitions.Count == 0) break;
                transitionNoStats = new List<Transition>();
                foreach (Transition t in allTransitions)
                {
                    if (!tree.ContainsKey(t.Hash))
                        transitionNoStats.Add(t);
                }

                // SELECTION
                if (transitionNoStats.Count == 0)
                {
                    double bestScore = float.MinValue;
                    int parentPlays = path[path.Count - 1].plays;
                    double ucb1Score;
                    int indexOfBestTransition = 0;
                    for (int j = 0; j < allTransitions.Count; j++)
                    {
                        ucb1Score = tree[allTransitions[i].Hash].ParentUCBScore(parentPlays);
                        if (ucb1Score > bestScore)
                        {
                            bestScore = ucb1Score;
                            indexOfBestTransition = j;
                        }
                    }
                    Transition bestTransition = allTransitions[indexOfBestTransition];
                    copy.Transition(bestTransition);
                    path.Add(tree[bestTransition.Hash]);
                }

                // EXPANSION
                else
                {
                    Transition t = transitionNoStats.RandomItem(rng);
                    copy.Transition(t);
                    Node node = new Node(copy.CurrentPlayer);

                    path.Add(node);
                    tree.Add(t.Hash, node);

                    break;
                }
            }

            // ROLLOUT
            copy.Rollout();
            while (!copy.simulationRunning)

            // BACKPROP
            foreach (Node node in path)
            {
                node.plays++;
                if (copy.IsWinner(node.player))
                    node.wins++;
            }
        }

        // Simulations are over. Pick the best move, then return it.
        allTransitions = game.GetLegalTransitions();
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
