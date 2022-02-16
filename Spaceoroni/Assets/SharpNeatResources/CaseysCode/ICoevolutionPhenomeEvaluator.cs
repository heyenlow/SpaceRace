using SharpNeat.Core;

namespace AI_SpaceRace
{
    public interface ICoevolutionPhenomeEvaluator<TPhenome>
    {
        /// <summary>
        /// Gets the total number of individual genome evaluations that have been performed by this evaluator.
        /// </summary>
        ulong EvaluationCount { get; }

        /// <summary>
        /// Gets a value indicating whether some goal fitness has been achieved and that
        /// the the evolutionary algorithm search should stop. This property's value can remain false
        /// to allow the algorithm to run indefinitely.
        /// </summary>
        bool StopConditionSatisfied { get; }

        /// <summary>
        /// Evaluate the provided phenomes and return their fitness scores.
        /// </summary>
        void Evaluate(TPhenome phenome1, TPhenome phenome2,
                      out FitnessInfo fitness1, out FitnessInfo fitness2);

        /// <summary>
        /// Reset the internal state of the evaluation scheme if any exists.
        /// </summary>
        void Reset();
    }
}
