using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIBenchmarksApp.models
{
    public class SAResult
    {
        public double BestCost { get; set; }
        public List<double> AllBestCosts { get; set; } = new();
        public List<double> AllCosts { get; set; } = new();
        public List<double> BestArgs { get; set; } = new();
    }
}
