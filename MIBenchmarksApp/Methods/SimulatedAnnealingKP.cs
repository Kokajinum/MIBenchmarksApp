using MIBenchmarksApp.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIBenchmarksApp.Methods
{
    public class SimulatedAnnealingKP
    {
        private int dimension;
        private int maxFES;
        private Random random;
        private Func<double[], double> objectiveFunction;
        private double initialTemperature;
        private double finalTemperature;
        private double coolingRate;
        private double[] initialSolution;
        private int metropolis;

        public SimulatedAnnealingKP(int dimension, int maxFES, Func<double[], double> objectiveFunction, double finalTemperature,
            double initialTemperature, double coolingRate, int metropolis)
        {
            this.dimension = dimension;
            this.maxFES = maxFES;
            this.objectiveFunction = objectiveFunction;
            this.initialTemperature = initialTemperature;
            this.finalTemperature = finalTemperature;
            this.coolingRate = coolingRate;
            this.metropolis = metropolis;
            random = new Random();
            initialSolution = new double[dimension];
        

        public double[] GenerateNeighbour(double[] x0)
        {
            double[] neighbour = new double[x0.Length];

            for (int i = 0; i < x0.Length; i++)
            {
                if (random.NextDouble() < 1.0 / x0.Length)
                {
                    neighbour[i] = x0[i] == 0 ? 1 : 0;
                }
                else
                {
                    neighbour[i] = x0[i];
                }
            }

            return neighbour;
        }

        public double[] GenerateStart(int dimension)
        {
            double[] res = new double[dimension];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = random.Next(2);
            }
            return res;
        }

        public virtual SAResult Compute()
        {
            SAResult result = new();
            double[] x0 = GenerateStart(dimension);
            double[] xBest = x0;

            double temperature = initialTemperature;

            while (temperature > finalTemperature)
            {
                for (int i = 0; i < metropolis; i++)
                {
                    double[] x = GenerateNeighbour(x0);

                    double xCost = objectiveFunction(x);
                    result.AllCosts.Add(xCost);

                    double Df = xCost - objectiveFunction(x0);

                    if (Df < 0)
                    {
                        x0 = x;
                    }
                    if (objectiveFunction(x) < objectiveFunction(xBest))
                    {
                        xBest = x;
                        result.BestCost = xCost;
                        result.AllBestCosts.Add(xCost);
                    }
                    else 
                    {
                        double r = random.NextDouble();
                        if (r < Math.Exp(-Df / temperature))
                        {
                            x0 = x;
                        }
                    }
                }
                temperature *= coolingRate;
            }
            result.BestArgs.AddRange(xBest);
            return result;
        }
    }
}
