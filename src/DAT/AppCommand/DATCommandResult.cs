using DAT.Providers.Sql;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace DAT.AppCommand
{
    public class DATCommandResult
    {
        public List<List<ExpandoObject>> ResultSets { get; set; }
        public List<object> QueryStatistics { get; set; }
    }
}
