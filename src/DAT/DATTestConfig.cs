namespace DAT
{
    public class DATTestConfig
    {
        public DATTestConfig()
        {
            ThreadCount = 1;
            Iterations = 3;
        }

        public string PreTest1SQL { get; set; }

        public string Test1SQL { get; set; }

        public string PreTest2SQL { get; set; }

        public string Test2SQL { get; set; }

        public int Iterations { get; set; }

        public int ThreadCount { get; set; }

        public string Test1ConnectionString { get; set; }

        public string Test2ConnectionString { get; set; }
    }
}
