using SharpNeat.Phenomes;
using SharpNeat.Core;

namespace AI_SpaceRace
{
    class SantoriniCoevolutionEvaluator : ICoevolutionPhenomeEvaluator<IBlackBox>
    {
        private ulong _evalCount;

        public ulong EvaluationCount
        {
            get { return _evalCount; }
        }

        public bool StopConditionSatisfied
        {
            get { return false; }
        }

        public void Evaluate(IBlackBox agent1, IBlackBox agent2, out FitnessInfo fitness1, out FitnessInfo fitness2)
        {
            //NeatPlayer player1 = new NeatPlayer(agent1);
            //NeatPlayer player2 = new NeatPlayer(agent2);

            
            //// Play the two agents against each other
            //var winner = Game.TrainNets(player1, player2);

            //if (winner == player1)
            //{
            //    fitness1 = new FitnessInfo(10, 10);
            //    fitness2 = new FitnessInfo(0, 0);
            //}
            //else if (winner == player2)
            //{
            //    fitness2 = new FitnessInfo(10, 10);
            //    fitness1 = new FitnessInfo(0, 0);
            //}
            //else
            //{
            //    // either a tie or an error.
            //    fitness1 = new FitnessInfo(1, 1);
            //    fitness2 = new FitnessInfo(1, 1);
            //}
            ////// Score agent1
            ///// IF WE CHOOSE TO SCORE BY MORE THAN JUST WINS
            ////double score1 = getScore(winner);
            fitness1 = new FitnessInfo(0, 0);

            ////// Score agent2
            ////double score2 = getScore(winner);
            fitness2 = new FitnessInfo(0, 0);

            //// Update the evaluation counter
            //_evalCount++;
        }

        public void Reset()
        {
        }
    }
}
