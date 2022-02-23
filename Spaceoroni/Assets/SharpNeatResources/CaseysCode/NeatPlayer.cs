using AI_SpaceRace;
using SharpNeat.Core;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class NeatPlayer : IPlayer
{
    SantoriniCoevolutionExperiment _experiment = new SantoriniCoevolutionExperiment();
    private Turn currentTurn;

    public IBlackBox Brain { get; set; }

    public NeatPlayer(IBlackBox brain)
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
        NeatGenomeFactory fac = new NeatGenomeFactory(47, 20);

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

    private void setInputSignalArrayBuilder(ISignalArray inputSignalArray, Game g)
    {
        // change builder x, y coordinates to an int representing that square (0-24)
        inputSignalArray[0] = (Builder1.getLocation().x * 5) + Builder1.getLocation().y;
        inputSignalArray[1] = (Builder2.getLocation().x * 5) + Builder2.getLocation().y;
        inputSignalArray[2] = (g.curPlayer == g.Player1) ? (g.Player2.getBuilders().Item1.getLocation().x * 5) + g.Player2.getBuilders().Item1.getLocation().y : (g.Player1.getBuilders().Item1.getLocation().x * 5) + g.Player1.getBuilders().Item1.getLocation().y;
        inputSignalArray[3] = (g.curPlayer == g.Player1) ? (g.Player2.getBuilders().Item2.getLocation().x * 5) + g.Player2.getBuilders().Item2.getLocation().y : (g.Player1.getBuilders().Item2.getLocation().x * 5) + g.Player1.getBuilders().Item2.getLocation().y;
        // loop sets 25 inputs to board heights.
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                // add two for buffer between first 4 inputs (builders)
                int index = (i + (5 * j)) + 4;
                inputSignalArray[index] = g.state[i, j]; 
            }
        }
    }

    private void setInputSignalArrayMove(ISignalArray inputSignalArray, List<string> possim, Game g)
    {
        setInputSignalArrayBuilder(inputSignalArray, g);
        Coordinate evalMove;
        // this loop finishes up last possible 9 inputs.
        for (int i = 29; i < possim.Count; i++)
        {
            // need to convert coordinate string (ex. "C2") into 0-24 location on board (C2 = 12);
            evalMove = Coordinate.stringToCoord(possim[i - 29]);
            inputSignalArray[i] = (evalMove.x * 5) + evalMove.y;

        }

    }

    private void setInputSignalArrayBuildLoc(ISignalArray inputSignalArray, List<string> possib, Game g)
    {
        // same 29 inputs for start
        setInputSignalArrayBuilder(inputSignalArray, g);
        Coordinate evalBuild;
        // loop finishes up last 9 possible inputs
        for (int i = 38; i < possib.Count; i++)
        {
            evalBuild = Coordinate.stringToCoord(possib[i - 38]);
            inputSignalArray[i] = (evalBuild.x * 5) + evalBuild.y;
        }
    }

    public override IEnumerator PlaceBuilder(int builder, int player, Game g)
    {
        //Coordinate builder1 = new Coordinate(0, 0);
        //Coordinate builder2 = new Coordinate(1, 1);
        //if (builder == 1) { moveBuidler(builder, builder1, g); } else { moveBuidler(builder, builder2, g); }
        // this player is now temporarily the current player
        g.curPlayer = this;
        // the rival is whichever player this isn't
        g.rival = (ReferenceEquals(g.Player2, this)) ? g.Player1 : g.Player2;
        System.Random rnd = new System.Random();
        Coordinate tmp = new Coordinate();
        int x, y;
        if (g.rival.getBuilders().Item1.coord.x == -1 && g.rival.getBuilders().Item2.coord.y == -1)
        {
            if (builder == 1)
            {
                tmp.x = rnd.Next(0, 4);
                tmp.y = rnd.Next(0, 4);
                moveBuidler(builder, tmp, g);
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
            if (g.rival.getBuilders().Item2.coord.x == -1 && g.rival.getBuilders().Item2.coord.y == -1)
            {
                // place a builder near the opponent's first builder.
                findFreeSpots(g, g.rival.getBuilders().Item1.getLocation().x, g.rival.getBuilders().Item1.getLocation().y, builder);
            }
            else
            {
                x = Math.Abs((g.rival.getBuilders().Item1.coord.x + g.rival.getBuilders().Item2.coord.x) / 2);
                y = Math.Abs((g.rival.getBuilders().Item1.coord.y + g.rival.getBuilders().Item2.coord.y) / 2);
                findFreeSpots(g, x, y, builder); // neat player places second builder close to opponent's builder averaged out
            }
        }
        yield return true;
    }

    private void findFreeSpots(Game g, int x, int y, int builderID)
    {
        Coordinate tmp = new Coordinate {x = x, y = y };
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
                        moveBuidler(builderID, tmp, g);
                        return;
                    }
                }
        }
        else
        {
            // place builder at x y
            moveBuidler(builderID, tmp, g);
            return;
        }
    }
    
    public override IEnumerator SelectBuilder(Game g)
    {
        // output 0 and 1 represent confidence values for builder 1 and 2
        // return whichever builder has the highest confidence value.
        Coordinate builder = (Brain.OutputSignalArray[0] > Brain.OutputSignalArray[1]) ? Builder1.getLocation() : Builder2.getLocation();

        // DON'T ALLOW THE BOT TO CHOOSE A BUILDER THAT CAN'T MOVE ANYWHERE! 
        // IF A MOVE *CAN* BE MADE, IT *MUST* BE MADE.
        // this code won't work because SelectBuilder is inhereted from IPlayer and can't know about the game :/ i guess
        //if (g.getAllPossibleMoves(builder).Count == 0) builder = (builder == Builder1.Location) ? Builder2.Location : Builder1.Location;
        currentTurn.BuilderLocation = builder;
        yield return builder;
    }

    public override IEnumerator chooseMove(Game g)
    {
        // this function has a chance to return {-1,-1} (shouldn't be possible?)   
        var possibleMoves = g.getAllPossibleMoves(currentTurn.BuilderLocation);
        if (possibleMoves.Count == 0)
        {
            currentTurn.canPerformTurn = false;
            yield return null;
        }
        // outputs 2-11 are the 9 possible move locations confidence values
        // find most confident move
        Coordinate move = new Coordinate();
        double max = double.MinValue;
        var possim = g.getAllPossibleMoves(currentTurn.BuilderLocation);
        for (int i = 0; i < possim.Count; i++)
        {
            if (Brain.OutputSignalArray[i+2] >= max)
            {
                move = Coordinate.stringToCoord(possim[i]);
                max = Brain.OutputSignalArray[i+2];
            }
        }
        // return highest scoring move to go to next.
        currentTurn.MoveLocation = move;
        moveBuidler(getBuilderInt(new Coordinate(currentTurn.BuilderLocation.x, currentTurn.BuilderLocation.y)), move, g);
        yield return null;
    }

    public override IEnumerator chooseBuild(Game g)
    {
        // pass in move for valid
        var possibleBuilds = g.getAllPossibleBuilds(currentTurn.MoveLocation);
        double max = double.MinValue;
        Coordinate build = new Coordinate();

        if (possibleBuilds.Count < 1)
        {
            // shouldn't be possible... because you should always be able to at least build on the square you came from...
            Debug.LogError("NeatPlayer returned less than one build possible!\nMOVELOCATION: " + Coordinate.coordToString(currentTurn.MoveLocation));
        }
        else
        {
            build = Coordinate.stringToCoord(possibleBuilds[0]);
        }

        for (int i = 0; i < possibleBuilds.Count; i++)
        {
            Coordinate evalBuild;
            evalBuild = Coordinate.stringToCoord(possibleBuilds[i]);

            if (Brain.OutputSignalArray[i+11] >= max && !Coordinate.Equals(evalBuild, currentTurn.MoveLocation))
            {
                build.x = evalBuild.x;
                build.y = evalBuild.y;
                max = Brain.OutputSignalArray[i+11];
            }
        }
        if (Coordinate.Equals(build, new Coordinate()))
        {
            if (Coordinate.stringToCoord(possibleBuilds[0]).x != -1 && Coordinate.stringToCoord(possibleBuilds[0]).y != -1)
            {
                build = Coordinate.stringToCoord(possibleBuilds[0]);
            }
            else
            {
                // null build error case
                Debug.LogError("NeatPlayer chooseBuild returned null build");
                currentTurn.canPerformTurn = false;
                yield return null;
            }
        }
        if (Coordinate.Equals(currentTurn.MoveLocation, new Coordinate(0, 0)))
        {
            // this was an error in the console logic
            // because in the console logic moveLocation is never null
            // I think because the AI moves as soon as it picks a new location
            //Debug.LogError("Reported no move...");
        }
        currentTurn.BuildLocation = build;
        yield return null;
    }

    public override IEnumerator beginTurn(Game g)
    {
        //Debug.Log("NEAT turn Begin");
        currentTurn = new Turn();
        Brain.ResetState();

        setInputSignalArrayBuilder(Brain.InputSignalArray, g);

        Brain.Activate();

        // after activation choose a builder
        yield return StartCoroutine(SelectBuilder(g));

        //Debug.Log("NEAT player builder selected");

        var possim = g.getAllPossibleMoves(currentTurn.BuilderLocation);
        if (possim.Count < 1)
        {
            // Need to choose other builder instead
            // because this builder has no moves
            currentTurn.BuilderLocation = (Coordinate.Equals(currentTurn.BuilderLocation, Builder1.getLocation())) ? Builder2.getLocation() : Builder1.getLocation();
            if (g.getAllPossibleMoves(currentTurn.BuilderLocation).Count < 1)
            {
                Console.WriteLine("LOSE CONDITION");
                g.playerState = Game.PlayerState.Loser;
                currentTurn.canPerformTurn = false;
            }
        }

        // after choosing a builder, find the best square you can move to from it.
        setInputSignalArrayMove(Brain.InputSignalArray, possim, g);
        Brain.Activate();

        yield return StartCoroutine(chooseMove(g));

        // TODO: NeatPlayer.chooseMove has a chance to return -1,-1 as a move, it currently is not handled...
        if (currentTurn.MoveLocation.x == -1 && currentTurn.MoveLocation.y == -1)
        {
            currentTurn.canPerformTurn = false;
            yield return null;
        }
        else
        {
            if (g.isWin(currentTurn.MoveLocation))
            {
                currentTurn.isWin = true;

                turns.Add(currentTurn);
                yield return null;
            }

            List<string> possib = g.getAllPossibleBuilds(currentTurn.MoveLocation);

            setInputSignalArrayBuildLoc(Brain.InputSignalArray, possib, g);
            Brain.Activate();


            yield return StartCoroutine(chooseBuild(g)); // might return -1,-1? shouldn't though....

            if (currentTurn.MoveLocation == currentTurn.BuildLocation)
            {
                Debug.LogError("This shouldn't be possible!");
            }

            turns.Add(currentTurn);
        }


        yield return null;
    }
}
