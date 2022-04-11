using Assets._Scripts.MCTS;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

interface Task
{
   NativeArray<long> Execute();
}


public class UCB1Tree
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

    public class CoroutinePermanence
    {
        public Coroutine coroutine { get; private set; }
        public SimPlayer result;
        private IEnumerator target;
        public CoroutinePermanence(MonoBehaviour owner, IEnumerator target)
        {
            this.target = target;
            coroutine = owner.StartCoroutine(Run());
            result = new SimPlayer(target.Current as SimPlayer);
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

    public class Node
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

    ConcurrentDictionary<long, Node> tree;

    private static double UCB1(double childWins, double childPlays, double parentPlays) => (childWins / childPlays) + Math.Sqrt(2f * Math.Log(parentPlays) / childPlays);

    public IEnumerator Search(Game game, int simulations, List<Turn> turns, Turn currentTurn = null)
    {
        if (Game.ZobristTable == null) throw new System.NullReferenceException("MCTS Search has an uninitialized zobrist table"); // never happens (thankfully)
        SimGame g = new SimGame(game);
        if (tree == null) tree = new ConcurrentDictionary<long, Node>();
        else if (!tree.ContainsKey(g.Hash)) tree.TryAdd(g.Hash, new Node(g.CurrentPlayer));

        List<Node> path = new List<Node>();
        SimGame copy;
        List<Transition> allTransitions;
        List<Transition> transitionNoStats;
        Unity.Mathematics.Random rng = new Unity.Mathematics.Random();

        //for (int i = 0; i < simulations; i++)
        //{
        //    Debug.Log("Simulation " + i + " STARTED.");
        //    copy = new SimGame(g.DeepCopy());
        //    path.Clear();
        //    path.Add(tree[g.Hash]);

        //    List<Transition> LegalTransitions = new List<Transition>();

        //    while (!copy.IsGameOver())
        //    {
        //        bool running = true;
        //        CoroutineWithData copy_data = new CoroutineWithData(game, copy.GetLegalTransitions());
        //        LegalTransitions = copy_data.result;
        //        //CoroutineWithData co_data = new CoroutineWithData(game, copy.GetLegalTransitions(LegalTransitions));
        //        //LegalTransitions = co_data.result;

        //        if (LegalTransitions.Count == 0)
        //        {
        //            break;
        //        }

        //        transitionNoStats = new List<Transition>();
        //        foreach (Transition t in LegalTransitions)
        //        {
        //            if (!tree.ContainsKey(t.Hash))
        //                transitionNoStats.Add(t);

        //            //Debug.Log(Coordinate.coordToString(t.Builder) + Coordinate.coordToString(t.Move) + Coordinate.coordToString(t.Build));
        //        }

        //        // SELECTION
        //        if (transitionNoStats.Count == 0)
        //        {
        //            //Debug.Log("Count ==0 ");
        //            double bestScore = float.MinValue;
        //            int parentPlays = path[path.Count - 1].plays;
        //            double ucb1Score;
        //            int indexOfBestTransition = 0;
        //            for (int j = 0; j < LegalTransitions.Count; j++)
        //            {
        //                ucb1Score = tree[LegalTransitions[j].Hash].ParentUCBScore(parentPlays);
        //                if (ucb1Score > bestScore)
        //                {
        //                    bestScore = ucb1Score;
        //                    indexOfBestTransition = j;
        //                }
        //            }
        //            Transition bestTransition = LegalTransitions[indexOfBestTransition];
        //            copy.Transition(bestTransition);
        //            path.Add(tree[bestTransition.Hash]);
        //        }

        //        // EXPANSION
        //        else
        //        {
        //            //Debug.Log("Expansion");
        //            Transition t = transitionNoStats.RandomItem(rng);
        //            copy.Transition(t);
        //            Node node = new Node(copy.CurrentPlayer);

        //            path.Add(node);
        //            tree.TryAdd(t.Hash, node);

        //            break;
        //        }
        //    }

        //    // ROLLOUT
        //    CoroutinePermanence co_perm = new CoroutinePermanence(game, copy.Rollout(game));
        //    //while (!copy.simulationRunning) is needed?

        //    // BACKPROP
        //    foreach (Node node in path)
        //    {
        //        node.plays++;
        //        if (copy.IsWinner(node.player))
        //            node.wins++;
        //    }
        //    Debug.Log("Simulation " + i + " FINISHED.");
        //}

        ManagedCodeInJob manJob = new ManagedCodeInJob(game);
        manJob.LateUpdate();

        // Simulations are over. Pick the best move, then return it.
        
        CoroutineWithData game_data = new CoroutineWithData(game, g.GetLegalTransitions());
        allTransitions = game_data.result;

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

        currentTurn = new Turn(tran.Builder, tran.Move, tran.Build);

        turns.Add(currentTurn);
    }

    private struct SearchJob : IJobParallelFor
    {
        public NativeArray<GCHandle> handle; // this references the exact sim game
        public NativeArray<long> result;

        public void Execute(int index)
        {

            Task task = (Task)handle[index].Target;
            result = task.Execute();
        }
    }

    public class ManagedCodeInJob
    {
        Game g;
        SimGame simRoot;
        List<Transition> moves;
        ConcurrentDictionary<long, Node> _tree = new ConcurrentDictionary<long, Node>();
        public ManagedCodeInJob(Game g)
        {
            this.g = g;
            simRoot = new SimGame(g.DeepCopy());
            CoroutineWithData co_dat = new CoroutineWithData(g, simRoot.GetLegalTransitions());
            moves = co_dat.result;

            ScheduleTask();
        }

        private readonly List<GCHandle> gcHandles = new List<GCHandle>();
        private readonly List<JobHandle> jobHandles = new List<JobHandle>();

        public void ScheduleTask()
        {
            for (int i = 0; i < 100; i++)
            {
                Task task = new SingleSearchTask(g, _tree, moves);
                GCHandle gcHandle = GCHandle.Alloc(task);
                this.gcHandles.Add(gcHandle);

            }

            var datum = new NativeArray<GCHandle>(gcHandles.Count, Allocator.TempJob);

            for (int i = 0; i < this.gcHandles.Count; ++i)
            {
                datum[i] = this.gcHandles[i];
            }

            SearchJob job = new SearchJob()
            {
                handle = datum,
                result = new NativeArray<long>(500, Allocator.TempJob)
            };

            this.jobHandles.Add(job.Schedule(gcHandles.Count, 2));
        }

        public ConcurrentDictionary<long, Node> LateUpdate()
        {
            // Wait for jobs to complete before freeing and removing them
            // Iterate from the end so we can easily remove from lists
            for (int i = this.jobHandles.Count - 1; i >= 0; --i)
            {
                if (!this.jobHandles[i].IsCompleted)
                {
                    while (this.jobHandles[i].IsCompleted == false)
                    {
                        // do something
                        Console.WriteLine("Pending");
                    }
                    // now it should be complete, right?

                }
                if (this.jobHandles[i].IsCompleted)
                {
                    this.jobHandles[i].Complete();
                    this.jobHandles.RemoveAt(i);

                    this.gcHandles[i].Free();
                    this.gcHandles.RemoveAt(i);
                }
            }

            return _tree;
        }
    }

    class SingleSearchTask : Task
    {
        static Game _root;
        SimGame simRoot;
        static ConcurrentDictionary<long, Node> _tree;
        List<Transition> _moves;
        public SingleSearchTask(Game root, ConcurrentDictionary<long,Node> tree, List<Transition> moves)
        {
            if (tree is null) tree = new ConcurrentDictionary<long, Node>();
            _root = root;
            _tree = tree;
            _moves = moves;
            simRoot = new SimGame(_root.DeepCopy());
            _tree.TryAdd(simRoot.Hash, new Node(simRoot.CurrentPlayer));
        }

        public NativeArray<long> Execute()
        {
            if (_tree == null) _tree = new ConcurrentDictionary<long, Node>();
            List<Node> path = new List<Node>();
            SimGame copy = new SimGame(simRoot.DeepCopy());
            List<Transition> allTransitions;
            List<Transition> transitionNoStats;
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(1234567890);

            path.Clear();
            path.Add(_tree[simRoot.Hash]);

            //List<Transition> LegalTransitions = new List<Transition>();

            while (!copy.IsGameOver())
            {

                if (_moves.Count == 0)
                {
                    break;
                }

                transitionNoStats = new List<Transition>();
                foreach (Transition t in _moves)
                {
                    if (!_tree.ContainsKey(t.Hash))
                        transitionNoStats.Add(t);

                    //Debu_root.Log(Coordinate.coordToString(t.Builder) + Coordinate.coordToString(t.Move) + Coordinate.coordToString(t.Build));
                }

                // SELECTION
                if (transitionNoStats.Count == 0)
                {
                    //Debu_root.Log("Count ==0 ");
                    double bestScore = float.MinValue;
                    int parentPlays = path[path.Count - 1].plays;
                    double ucb1Score;
                    int indexOfBestTransition = 0;
                    for (int j = 0; j < _moves.Count; j++)
                    {
                        ucb1Score = _tree[_moves[j].Hash].ParentUCBScore(parentPlays);
                        if (ucb1Score > bestScore)
                        {
                            bestScore = ucb1Score;
                            indexOfBestTransition = j;
                        }
                    }
                    Transition bestTransition = _moves[indexOfBestTransition];
                    copy.Transition(bestTransition);
                    path.Add(_tree[bestTransition.Hash]);
                }

                // EXPANSION
                else
                {
                    //Debu_root.Log("Expansion");
                    Transition t = transitionNoStats.RandomItem(rng);
                    copy.Transition(t);
                    Node node = new Node(copy.CurrentPlayer);

                    path.Add(node);
                    _tree.TryAdd(t.Hash, node);

                    break;
                }
            }

            // ROLLOUT
            copy.Rollout(_root);
            //while (!copy.simulationRunning) is needed?

            // BACKPROP
            foreach (Node node in path)
            {
                node.plays++;
                if (copy.IsWinner(node.player))
                    node.wins++;
            }

            NativeArray<long> ret = new NativeArray<long>(_tree.Count * 4, Allocator.Temp); // for each node in the tree, save 4 pieces of info into this array.
            int counter = 0;
            foreach (var n in _tree)
            {
                ret[counter] = n.Key;
                counter++;
                ret[counter] = n.Value.wins;
                counter++;
                ret[counter] = n.Value.plays;
                counter++;
                ret[counter] = n.Value.player.ID;
                counter++;
            }

            return ret;
        }
        
    }
}
