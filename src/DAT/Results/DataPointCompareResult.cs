using System;
using System.Collections.Generic;
using System.Text;

namespace DAT.Results
{
    public class DataPointCompareResult
    {
        public string Name { get; set; }

        public object[] TestValues { get; set; }

        public int TestValueCount { get; set; }

        public bool ValueEquality { get; set; }
    }
}
