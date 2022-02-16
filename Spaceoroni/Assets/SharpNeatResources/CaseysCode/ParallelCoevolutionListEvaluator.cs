using System.Collections.Generic;
using SharpNeat.Core;
using System.Threading.Tasks;
using System.Collections;

namespace AI_SpaceRace
{
    public class ParallelCoevolutionListEvaluator<TGenome, TPhenome> : IGenomeListEvaluator<TGenome>
        where TGenome : class, IGenome<TGenome>
        where TPhenome : class
    {
        readonly IGenomeDecoder<TGenome, TPhenome> _genomeDecoder;
        readonly ICoevolutionPhenomeEvaluator<TPhenome> _phenomeEvaluator;
        readonly ParallelOptions _parallelOptions;

        #region Constructors
        public ParallelCoevolutionListEvaluator(IGenomeDecoder<TGenome,TPhenome> genomeDecoder, ICoevolutionPhenomeEvaluator<TPhenome> phenomeEvaluator)
        {
            _genomeDecoder = genomeDecoder;
            _phenomeEvaluator = phenomeEvaluator;
            _parallelOptions = new ParallelOptions();
        }
        public ParallelCoevolutionListEvaluator(IGenomeDecoder<TGenome, TPhenome> genomeDecoder, ICoevolutionPhenomeEvaluator<TPhenome> phenomeEvaluator, ParallelOptions options)
        {
            _genomeDecoder = genomeDecoder;
            _phenomeEvaluator = phenomeEvaluator;
            _parallelOptions = options;
        }
        #endregion

        #region IGenomeListEvaluator<TGenome> Members

        public ulong EvaluationCount
        {
            get { return _phenomeEvaluator.EvaluationCount; }
        }

        public bool StopConditionSatisfied
        {
            get { return _phenomeEvaluator.StopConditionSatisfied; }
        }

        public void Reset()
        {
            _phenomeEvaluator.Reset();
        }

        public void Evaluate(IList<TGenome> genomeList)
        {
            // Create a temporary list of fitness values
            FitnessInfo[] results = new FitnessInfo[genomeList.Count];
            for (int i = 0; i < results.Length; i++) results[i] = FitnessInfo.Zero;

            // Exhaustively compete individuals against each other.
            Parallel.For(0, genomeList.Count, delegate (int i)
            {
                for(int j = 0; j < genomeList.Count; j++)
                {
                    // Don't bother evaluating individuals against themselves
                    if (i == j) continue;

                    // Decode the first genome.
                    TPhenome phenome1 = _genomeDecoder.Decode(genomeList[i]);

                    // Check that the first genome is valid.
                    if (phenome1 == null) continue;

                    // Decode the second genome
                    TPhenome phenome2 = _genomeDecoder.Decode(genomeList[j]);

                    // Check that the second genome is valid.
                    if (phenome2 == null) continue;

                    // Compete the two individuals against each other and get the results.
                    FitnessInfo fitness1, fitness2;
                    _phenomeEvaluator.Evaluate(phenome1, phenome2, out fitness1, out fitness2);

                    // Add the results to each genome's overall fitness.
                    // Note the mutex because parallelism
                    lock (results)
                    {
                        results[i]._fitness += fitness1._fitness;
                        results[j]._fitness += fitness2._fitness;
                    }
                }
            });

            // Update every genome in the population with its new fitness score.
            for (int i = 0; i < results.Length; i++)
            {
                genomeList[i].EvaluationInfo.SetFitness(results[i]._fitness);
            }
        }

        IEnumerator IGenomeListEvaluator<TGenome>.Evaluate(IList<TGenome> genomeList)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
