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

    public override void loadNEATPlayer(string path)
    {
        this.turns = new List<Turn>();
        // NEAT SETUP 
        _experiment = new SantoriniCoevolutionExperiment();
        XmlDocument xmlConfig = new XmlDocument();
        xmlConfig.Load("santorini.config.xml");
        _experiment.Initialize("Santorini", xmlConfig.DocumentElement);

        NeatGenome genome = null;
        NeatGenomeFactory fac = new NeatGenomeFactory(47, 20);

        XmlReader xr = XmlReader.Create(path);
        genome = NeatGenomeXmlIO.ReadCompleteGenomeList(xr, false, fac)[0];

        IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder;

        genomeDecoder = _experiment.CreateGenomeDecoder();

        Brain = genomeDecoder.Decode(genome);
        
    }

    public override IEnumerator PlaceBuilder(int builder, int player, Game g)
    {
        Coordinate builder1 = new Coordinate(0, 0);
        Coordinate builder2 = new Coordinate(1, 1);
        if (builder == 1) { moveBuidler(builder, builder1, g); } else { moveBuidler(builder, builder2, g); }
        yield return true;
    }

    public override IEnumerator SelectBuilder()
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

    private void setInputSignalArrayBuilder(ISignalArray inputSignalArray, Game g)
    {
        Brain.ResetState();
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
        Brain.ResetState();
        //this func sets up first 29 inputs
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
        Brain.ResetState();
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

    public override IEnumerator chooseMove(Game g)
    {
        // this function has a chance to return {-1,-1} (shouldn't be possible?)   
        var possibleMoves = g.getAllPossibleMoves(currentTurn.BuilderLocation);
        if (possibleMoves.Count == 0) yield return null;
        // outputs 2-... are the confidence value for each of the 25 squares.
        // find most confident move
        Coordinate move = new Coordinate();
        double max = double.MinValue;
        var possim = g.getAllPossibleMoves(currentTurn.BuilderLocation);
        for (int i = 0; i < possim.Count; i++)
        {
            if (Brain.OutputSignalArray[i+2] > max)
            {
                move = Coordinate.stringToCoord(possim[i]);
                max = Brain.OutputSignalArray[i+2];
            }
        }
        // return highest scoring move to go to next.
        currentTurn.MoveLocation = move;
        yield return move;
    }

    public override IEnumerator chooseBuild(Game g)
    {
        // pass in move for valid
        var possibleBuilds = g.getAllPossibleBuilds(currentTurn.MoveLocation);
        Coordinate build = new Coordinate();
        double max = double.MinValue;

        for (int i = 0; i < possibleBuilds.Count; i++)
        {
            Coordinate evalBuild;
            evalBuild = Coordinate.stringToCoord(possibleBuilds[i]);

            if (Brain.OutputSignalArray[i+11] > max && evalBuild.x != currentTurn.MoveLocation.x && evalBuild.y != currentTurn.MoveLocation.y)
            {
                build.x = evalBuild.x;
                build.y = evalBuild.y;
                max = Brain.OutputSignalArray[i+2];
            }
        }

        currentTurn.BuildLocation = build;
        yield return build;
    }

    public override IEnumerator beginTurn(Game g)
    {
        //Debug.Log("NEAT turn Begin");
        currentTurn = new Turn();

        setInputSignalArrayBuilder(Brain.InputSignalArray, g);

        Brain.Activate();

        // after activation choose a builder
        yield return StartCoroutine(SelectBuilder());

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
            }
        }

        // after choosing a builder, find the best square you can move to from it.
        setInputSignalArrayMove(Brain.InputSignalArray, possim, g);
        Brain.Activate();

        yield return StartCoroutine(chooseMove(g));
        

        if (g.isWin(currentTurn.MoveLocation))
        {
            currentTurn.isWin = true;

            turns.Add(currentTurn);
            yield return null;
        }

        List<string> possib = g.getAllPossibleBuilds(currentTurn.MoveLocation);

        setInputSignalArrayBuildLoc(Brain.InputSignalArray, possib, g);
        Brain.Activate();

        yield return StartCoroutine(chooseBuild(g));

        turns.Add(currentTurn);

        yield return null;
    }
}
