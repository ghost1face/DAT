namespace DAT.Providers.Sql
{
    public class TimeStats
    {
        public TimeStats(int cpu, int elapsed)
        {
            this.CPU = cpu;
            this.Elapsed = elapsed;
        }

        public int CPU { get; private set; }
        public int Elapsed { get; private set; }
    }
}
