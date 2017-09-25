namespace DAT.Providers.Sql
{
    public enum StatisticsRowType
    {
        None = 0,
        IO = 1,
        ExecutionTime = 2,
        CompileTime = 3,
        RowsAffected = 4,
        Error = 5
    }
}
