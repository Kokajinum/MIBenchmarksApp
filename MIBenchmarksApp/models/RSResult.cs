using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIBenchmarksApp.models
{
    public class RSResult
    {
        public double BestFitness { get; set; }
        public List<double> AllFitness { get; set; } = new();
        public List<double> BestArgs { get; set; } = new();
        public RSResult()
        {
            
        }
    }
}
