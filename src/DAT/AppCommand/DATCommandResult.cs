using System;
using System.Collections.Generic;
using System.Text;

namespace DAT.AppCommand
{
    public class DATCommandResult
    {
        public List<Dictionary<string, object>> ResultSet { get; set; }
        public Dictionary<string, object> PerformanceResults { get; set; }
    }
}
