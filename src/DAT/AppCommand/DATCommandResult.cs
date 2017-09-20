using System.Collections.Generic;

namespace DAT.AppCommand
{
    public class DATCommandResult
    {
        public List<List<Dictionary<string, object>>> ResultSets { get; set; }
        public Dictionary<string, object> PerformanceResults { get; set; }
    }
}
