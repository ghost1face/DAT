using System.Collections.Generic;

namespace DAT.Providers.Sql
{
    public class AggregatedQueryStatTotals
    {
        public QueryStatTotals Totals { get; set; }
        public QueryStatTotals Averages { get; set; }
        public Dictionary<int, QueryStatTotals> Percentiles { get; set; }
    }
}
