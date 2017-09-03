namespace DAT
{
    public class DATTestModel
    {
        public DATTestModel()
        {
            ThreadCount = 1;
            Iterations = 3;
        }

        public string Test1SQL { get; set; }

        public string Test2SQL { get; set; }

        public int Iterations { get; set; }

        public int ThreadCount { get; set; }

        public string Test1ConnectionString { get; set; }

        public string Test2ConnectionString { get; set; }
    }
}
