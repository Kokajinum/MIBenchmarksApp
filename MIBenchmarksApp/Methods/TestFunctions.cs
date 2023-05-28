using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIBenchmarksApp.Methods
{
    public static class TestFunctions
    {
        public static double FirstFunction(double[] x)
        {
            double sum = 0.0;

            for (int i = 0; i < x.Length; i++)
            {
                sum += Math.Pow(x[i], 2);
            }

            return sum;
        }

        public static double SecondFunction(double[] x)
        {
            double sum = 0.0;

            for (int i = 0; i < x.Length - 1; i++)
            {
                double diff1 = Math.Pow(x[i], 2) - x[i + 1];
                double diff2 = 1 - x[i];

                sum += 100 * (Math.Pow(diff1, 2) + Math.Pow(diff2, 2));
            }

            return sum;
        }

        public static double Schweffel(double[] x)
        {
            double sum = 0.0;
            for (int i = 0; i < x.Length; i++)
            {
                sum += x[i] * Math.Sin(Math.Sqrt(Math.Abs(x[i])));
            }
            return 418.983 * x.Length - sum;
        }

        public static double ObjectiveFunctionKP(double[] solution, List<Item> items, int maxWeight)
        {
            double totalValue = 0;
            double totalWeight = 0;
            for (int i = 0; i < solution.Length; i++)
            {
                if (solution[i] == 1)
                {
                    totalValue += items[i].Value;
                    totalWeight += items[i].Weight;
                }
            }

            if (totalWeight > maxWeight)
            {
                return 0;
            }

            return totalValue;
        }
    }
}
