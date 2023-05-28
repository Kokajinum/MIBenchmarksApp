using MIBenchmarksApp.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIBenchmarksApp.Methods
{
    public class Item
    {
        public int Id { get; set; }
        public int Weight { get; set; }
        public int Value { get; set; }
    }

    public class Knapsack
    {
        public List<Item> Items { get; set; }
        public int Capacity { get; set; }

        public Knapsack(List<Item> items, int capacity)
        {
            this.Items = items;
            this.Capacity = capacity;
        }
        public List<Item> ComputeBruteForce()
        {
            int n = Items.Count;
            List<Item> bestSolution = null;
            int bestSolutionValue = 0;

            // iterujeme přes všechny možné kombinace
            //(1 << n) je bitový posun -> "2 na n-tou"
            for (int i = 0; i < (1 << n); i++)
            {
                List<Item> currentSolution = new();
                int currentSolutionWeight = 0;
                int currentSolutionValue = 0;

                // zkontrolujeme, zda daný předmět patří do současné kombinace
                for (int j = 0; j < n; j++)
                {
                    int shifted = 1 << j; // Posun 1 doleva o j pozic
                    int andResult = i & shifted; // Provedeme bitovou operaci AND
                    bool isInCurrentCombination = andResult != 0; // Zkontrolujeme, zda výsledek není 0
                    if (isInCurrentCombination)
                    {
                        var item = Items[j];
                        currentSolutionWeight += item.Weight;
                        currentSolutionValue += item.Value;
                        currentSolution.Add(item);
                    }
                }

                if (currentSolutionWeight > Capacity)
                    continue;

                if (currentSolutionValue > bestSolutionValue)
                {
                    bestSolutionValue = currentSolutionValue;
                    bestSolution = currentSolution;
                }
            }

            return bestSolution;
        }

        public SAResult ComputeSA()
        {
            int dimension = Items.Count;
            int maxFES = (1 << dimension);
            int maxTemp = 1000;
            double minTemp = 0.01;
            double coolingDecr = 0.99;
            int metropolis = 10;

            SimulatedAnnealingKP saKp = new(dimension, maxFES, ObjectiveFunctionKP, minTemp, maxTemp, coolingDecr, metropolis);
            return saKp.Compute();
        }

        double ObjectiveFunctionKP(double[] solution)
        {
            double totalValue = 0;
            double totalWeight = 0;
            for (int i = 0; i < solution.Length; i++)
            {
                if (solution[i] == 1)
                {
                    totalValue += Items[i].Value;
                    totalWeight += Items[i].Weight;
                }
            }

            if (totalWeight > Capacity)
            {
                return 0; // Řešení je neplatné, pokud je celková hmotnost příliš velká
            }

            return totalValue; // Cílem je maximalizovat celkovou hodnotu
        }
    }

    
}
