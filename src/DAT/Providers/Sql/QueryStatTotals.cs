namespace DAT.Providers.Sql
{
    public class QueryStatTotals
    {
        public string TestRunIdentifier { get; set; }
        public int CPUCompileTime { get; set; }
        public int ElapsedCompileTime { get; set; }
        public int CPUExecutionTime { get; set; }
        public int ElapsedExecutionTime { get; set; }
        public int Scan { get; set; }
        public int Logical { get; set; }
        public int Physical { get; set; }
        public int ReadAhead { get; set; }
        public int LobLogical { get; set; }
        public int LobPhysical { get; set; }
        public int LobReadAhead { get; set; }
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
