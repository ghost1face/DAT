namespace DAT.Providers.Sql
{
    public class QueryStatTotals
    {
        public string TestRunIdentifier { get; set; }
        public double CPUCompileTime { get; set; }
        public double ElapsedCompileTime { get; set; }
        public double CPUExecutionTime { get; set; }
        public double ElapsedExecutionTime { get; set; }
        public double Scan { get; set; }
        public double Logical { get; set; }
        public double Physical { get; set; }
        public double ReadAhead { get; set; }
        public double LobLogical { get; set; }
        public double LobPhysical { get; set; }
        public double LobReadAhead { get; set; }
        public decimal PercentRead { get; set; }

        public override string ToString()
        {
            return 
$@"{TestRunIdentifier} Query Statistics:

Total Compile Time:
    CPU:        {CPUCompileTime}
    Elapsed:    {ElapsedCompileTime}

Total Execution Time:
    CPU:        {CPUExecutionTime}
    Elapsed:    {ElapsedExecutionTime}

IO Statistics:
    Scan Count:             {Scan}
    Logical Reads:          {Logical}
    Physical Reads:         {Physical}
    Lob Logical Reads:      {LobLogical}
    Lob Physical Reads:     {LobPhysical}
    ReadAhead Reads:        {ReadAhead}
    Lob Read Ahead Reads:   {LobReadAhead}
";
        }
    }
}
