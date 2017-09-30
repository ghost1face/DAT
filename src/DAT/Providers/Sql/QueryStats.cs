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

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var item = obj as QueryStats;
            if (item == null)
                return false;

            if (item.IOStatistics.Count == 0 &&
                item.ExecutionTimes.Count == 0 &&
                item.CompileTimes.Count == 0 &&
                item.RowsAffected == 0 &&
                item.Error == null)
                return true;

            return false;
        }

        // TODO: GetHashCode
    }
}
