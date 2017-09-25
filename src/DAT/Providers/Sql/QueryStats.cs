using System.Collections.Generic;

namespace DAT.Providers.Sql
{
    public class QueryStats
    {
        public IList<IOStats> IOStatistics { get; set; }
        public IList<TimeStats> ExecutionTimes { get; set; }
        public IList<TimeStats> CompileTimes { get; set; }
        public long RowsAffected { get; set; }
        public string Error { get; set; }
    }
}
