using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telemeter_v2
{
    public class Data
    {
        public DateTime startPeriod { get; set; }
        public DateTime endPeriod { get; set; }
        public int day { get; set; }
        public double usagePiek { get; set; }
        public double limitPiek { get; set; }
        public double predictionPiek { get; set; }
        public double usageDal { get; set; }
        public double usageTotal { get; set; }
        public string status { get; set; }
        public string abbo { get; set; }
        public DateTime updated { get; set; }
    }
}
