using DAT.Providers.Sql;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAT.Results
{
    public class PerformanceResult
    {
        public string TestIdentifier { get; set; }

        public QueryStatTotals Totals { get; set; }
        public QueryStatTotals Averages { get; set; }
        public Dictionary<int, QueryStatTotals> Percentiles { get; set; }
    }
}
