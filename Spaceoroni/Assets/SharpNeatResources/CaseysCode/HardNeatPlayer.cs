using AI_SpaceRace;
using SharpNeat.Core;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class HardNeatPlayer : IPlayer
{
    public class Transition
    {
        public Coordinate Builder;

        public Coordinate Move;

        public Coordinate Build;

        public double Value;

        public long Hash;

        public Transition(Coordinate builder, Coordinate move, Coordinate build, long hash)
        {
            Builder = builder;
            Move = move;
            Build = build;
            Hash = hash;
        }

        public override string ToString() => $"{Coordinate.coordToString(Builder)} + {Coordinate.coordToString(Move)} + {Coordinate.coordToString(Build)}";
    }

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

    SantoriniCoevolutionExperiment _experiment = new SantoriniCoevolutionExperiment();
    private Turn currentTurn;

    public IBlackBox Brain { get; set; }

    public HardNeatPlayer(IBlackBox brain)
    {
        Brain = brain;
    }

    //TODO: Compare with console function in Program.cs
    public override void loadNEATPlayer(string path)
    {
        turns = new List<Turn>();
        // NEAT SETUP 
        _experiment = new SantoriniCoevolutionExperiment();

        TextAsset textAsset = (TextAsset)Resources.Load("santorini.config");
        XmlDocument xmlConfig = new XmlDocument();
        xmlConfig.LoadXml(textAsset.text);
        _experiment.Initialize("Santorini", xmlConfig.DocumentElement);

        NeatGenome genome;
        NeatGenomeFactory fac = new NeatGenomeFactory(32, 1);

        try
        {
            //XmlReader xr = XmlReader.Create(path);
            TextAsset text = (TextAsset)Resources.Load(path);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(text.text);
            XmlNodeReader nodeReader = new XmlNodeReader(xmlDoc);
            XmlReaderSettings settings = new XmlReaderSettings();
            XmlReader reader = XmlReader.Create(nodeReader, settings);


            genome = NeatGenomeXmlIO.ReadCompleteGenomeList(reader, false, fac)[0];

            IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder;

            genomeDecoder = _experiment.CreateGenomeDecoder();
            Debug.Log("Loaded " + path + " AI successfully.");

            Brain = genomeDecoder.Decode(genome);
        }
        catch (Exception e)
        {
            Debug.LogError("Could not load " + path + " AI champ");
            Debug.LogError(e.GetType() + ": " + e.Message);
        }

    }

    public void setInputSignalArray(ISignalArray inputSignalArray, Game g, string transition)
    {
        //Brain.ResetState();
        var tmpBoard = TranslateState(g);
        // map character array representing board state to inputs 0-24
        inputSignalArray[0] = ReferenceEquals(g.CurrentPlayer, g.Player1) ? 0 : 1;    // player 2 is represented by a {1}, and player 1 is represented by a {0}
        int i, x, y;
        for (i = 1, x = 0; x < 5; x++)
        {
            for (y = 0; y < 5; y++, i++)
            {
                inputSignalArray[i] = tmpBoard[x, y];
            }
        }
        for (int j = 0; j < 6; j++, i++)
        {
            inputSignalArray[i] = transition[j];
        }
    }

    public IEnumerator PlaceBuilder(int builder, int player, Game g)
    {
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
        currentTurn = new Turn();

        CoroutineWithData co_data = new CoroutineWithData(g, g.GetLegalTransitions());
        List<Transition> legalMoves = co_data.result;

        if (legalMoves.Count == 0)
        {
            state = States.Loser;
            g.Rival.state = States.Winner;
            yield return null;
        }
        foreach (var t in legalMoves)
        {
            // load in the state (board + move)
            setInputSignalArray(Brain.InputSignalArray, g, t.ToString());
            // perform some magic
            Brain.Activate();
            // save the value.
            t.Value = Brain.OutputSignalArray[0];
        }
        int indexOfBestMoveFound = 0;
        double bestScoreFound = double.MinValue;
        for (int i = 0; i < legalMoves.Count; i++)
        {
            if (legalMoves[i].Value > bestScoreFound)
            {
                bestScoreFound = legalMoves[i].Value;
                indexOfBestMoveFound = i;
            }
        }
        currentTurn = new Turn(legalMoves[indexOfBestMoveFound].ToString());
        while (BuildersAreMoving()) yield return new WaitForEndOfFrame();

        if (g.isWin(currentTurn.MoveLocation))
        {
            currentTurn.isWin = true;

            turns.Add(currentTurn);
            yield return null;
        }
        yield return null;

    }

    /// <summary>
    /// returns the 2D character matrix representing the current game board for both players.
    /// </summary>
    /// <returns></returns>
    public char[,] TranslateState(Game g)
    {
        char[,] ret = new char[5, 5];
        // takes the Game g.state int matrix and converts it to a char matrix with representations for builders on board
        Coordinate tmp = new Coordinate();
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                tmp.x = i; tmp.y = j;
                if (g.locationClearOfAllBuilders(tmp))
                {
                    ret[i, j] = (char)(g.state[i, j] + 48);
                }
                else
                {
                    char piece = '0';
                    switch (g.state[i, j])
                    {
                        case 0:
                            if (Equals(tmp, Coordinate.stringToCoord(g.Player1.getBuilderLocations().Substring(0, 2)))) piece = 'A';
                            else if (Equals(tmp, Coordinate.stringToCoord(g.Player1.getBuilderLocations().Substring(2, 2)))) piece = 'E';
                            else if (Equals(tmp, Coordinate.stringToCoord(g.Player2.getBuilderLocations().Substring(0, 2)))) piece = 'a';
                            else if (Equals(tmp, Coordinate.stringToCoord(g.Player2.getBuilderLocations().Substring(2, 2)))) piece = 'e';
                            break;
                        case 1:
                            if (Equals(tmp, Coordinate.stringToCoord(g.Player1.getBuilderLocations().Substring(0, 2)))) piece = 'B';
                            else if (Equals(tmp, Coordinate.stringToCoord(g.Player1.getBuilderLocations().Substring(2, 2)))) piece = 'F';
                            else if (Equals(tmp, Coordinate.stringToCoord(g.Player2.getBuilderLocations().Substring(0, 2)))) piece = 'b';
                            else if (Equals(tmp, Coordinate.stringToCoord(g.Player2.getBuilderLocations().Substring(2, 2)))) piece = 'f';
                            break;
                        case 2:
                            if (Equals(tmp, Coordinate.stringToCoord(g.Player1.getBuilderLocations().Substring(0, 2)))) piece = 'C';
                            else if (Equals(tmp, Coordinate.stringToCoord(g.Player1.getBuilderLocations().Substring(2, 2)))) piece = 'G';
                            else if (Equals(tmp, Coordinate.stringToCoord(g.Player2.getBuilderLocations().Substring(0, 2)))) piece = 'c';
                            else if (Equals(tmp, Coordinate.stringToCoord(g.Player2.getBuilderLocations().Substring(2, 2)))) piece = 'g';
                            break;
                        case 3:
                            if (Equals(tmp, Coordinate.stringToCoord(g.Player1.getBuilderLocations().Substring(0, 2)))) piece = 'D';
                            else if (Equals(tmp, Coordinate.stringToCoord(g.Player1.getBuilderLocations().Substring(2, 2)))) piece = 'H';
                            else if (Equals(tmp, Coordinate.stringToCoord(g.Player2.getBuilderLocations().Substring(0, 2)))) piece = 'd';
                            else if (Equals(tmp, Coordinate.stringToCoord(g.Player2.getBuilderLocations().Substring(2, 2)))) piece = 'h';
                            break;
                    }
                    ret[i, j] = piece;
                }
            }

        }

        return ret;
    }

}
