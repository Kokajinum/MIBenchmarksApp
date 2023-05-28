using MIBenchmarksApp.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIBenchmarksApp.Methods
{
    public class RandomSearch
    {
        private int _dimension;
        private int _maxFES;
        private Random _random;
        private Func<double[], double> _objectiveFunction;
        private double _lowerBound;
        private double _upperBound;
        private double _initialFitness;
        private List<double> _initialSolution;

        public RandomSearch(int dimension, int maxFES, Func<double[], double> objectiveFunction, double lowerBound, double upperBound)
        {
            _dimension = dimension;
            _maxFES = maxFES;
            _objectiveFunction = objectiveFunction;
            _lowerBound = lowerBound;
            _upperBound = upperBound;
            _random = new Random();
            _initialFitness = double.MaxValue;
            _initialSolution = new();
        }

        public RSResult Compute()
        {
            RSResult result = new();
            List<double> argBest = _initialSolution;
            double fitness0 = _initialFitness;

            for (int i = 0; i < _maxFES; i++)
            {
                List<double> arg = new();
                for (int j = 0; j < _dimension; j++)
                {
                    arg.Add(_random.NextDouble() * (_upperBound - _lowerBound) + _lowerBound);
                }

                double fitness = _objectiveFunction(arg.ToArray());
                if (fitness < fitness0)
                {
                    fitness0 = fitness;
                    argBest = arg;
                }
                result.AllFitness.Add(fitness0);
            }
            result.BestArgs = argBest;
            result.BestFitness = fitness0;

            return result;
        }
    }
}
