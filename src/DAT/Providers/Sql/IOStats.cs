namespace DAT.Providers.Sql
{
    public class IOStats
    {
        public IOStats(int rowNumber, string tableName)
        {
            this.RowNumber = rowNumber;
            this.Table = tableName;
        }

        public IOStats(int rowNumber, string tableName, int scan, int logical, int physical, int readAhead, int lobLogical, int lobPhysical, int lobReadAhead)
        {
            this.RowNumber = rowNumber;
            this.Table = tableName;
            this.Scan = scan;
            this.Logical = logical;
            this.Physical = physical;
            this.ReadAhead = readAhead;
            this.LobLogical = lobLogical;
            this.LobPhysical = lobPhysical;
            this.LobReadAhead = lobReadAhead;
        }

        public int RowNumber { get; private set; }
        public string Table { get; private set; }
        public bool NoStats { get; set; }
        public int Scan { get; private set; }
        public int Logical { get; private set; }
        public int Physical { get; private set; }
        public int ReadAhead { get; private set; }
        public int LobLogical { get; private set; }
        public int LobPhysical { get; private set; }
        public int LobReadAhead { get; private set; }
        public decimal PercentRead { get; private set; }
    }
}
