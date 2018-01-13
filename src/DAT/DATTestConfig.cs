using System.Collections.Generic;

namespace DAT
{
    public class DATTestConfig
    {
        public DATTestConfig()
        {
            ThreadCount = 1;
            Iterations = 3;
        }

        public int Iterations { get; set; }
        public int ThreadCount { get; set; }

        public IEnumerable<DATSQLTestConfig> Tests { get; set; }
    }

    public class DATSQLTestConfig
    {
        public string PreSQL { get; set; }
        public string SQL { get; set; }
        public string ConnectionString { get; set; }
    }
}
