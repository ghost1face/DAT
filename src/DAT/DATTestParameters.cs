namespace DAT
{
    public class DATTestParameters
    {
        public string SqlQuery { get; set; }
        public string ConnectionString { get; set; }
        public int Iterations { get; set; }
        public bool DataCompare { get; set; }
        public bool PerformanceProfile { get; set; }
    }
}
