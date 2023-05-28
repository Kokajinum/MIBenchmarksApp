using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIBenchmarksApp.Methods
{
    public static class StatisticFunctions
    {
        public static double Min(List<double> values) => values.Min();
        public static double Max(List<double> values) => values.Max();
        public static double Mean(List<double> values) => values.Average();
        public static double Median(List<double> values)
        {
            var sortedValues = values.OrderBy(x => x).ToList();
            return (sortedValues[(values.Count - 1) / 2] + sortedValues[values.Count / 2]) / 2.0;
        }

        public static double StdDev(List<double> values)
        {
            double mean = Mean(values);
            double squareSum = values.Sum(v => Math.Pow(v - mean, 2));
            return Math.Sqrt(squareSum / (values.Count - 1));
        }

        public static void DrawGraph()
        {

        }
    }
}
