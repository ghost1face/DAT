using DAT.Providers.Sql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ParseStats()
        {
            var statsString = @"
SQL Server parse and compile time: 
   CPU time = 108 ms, elapsed time = 108 ms.

(13431682 row(s) affected)
Table 'PostTypes'. Scan count 1, logical reads 2, physical reads 1, read-ahead reads 0, lob logical reads 0, lob physical reads 0, lob read-ahead reads 0.
Table 'Users'. Scan count 5, logical reads 42015, physical reads 1, read-ahead reads 41306, lob logical reads 0, lob physical reads 0, lob read-ahead reads 0.
Table 'Comments'. Scan count 5, logical reads 1089402, physical reads 248, read-ahead reads 1108174, lob logical reads 0, lob physical reads 0, lob read-ahead reads 0.
Table 'PostTags'. Scan count 5, logical reads 77500, physical reads 348, read-ahead reads 82219, lob logical reads 0, lob physical reads 0, lob read-ahead reads 0.
Table 'Posts'. Scan count 5, logical reads 397944, physical reads 9338, read-ahead reads 402977, lob logical reads 0, lob physical reads 0, lob read-ahead reads 0.
Table 'Worktable'. Scan count 999172, logical reads 16247024, physical reads 0, read-ahead reads 0, lob logical reads 0, lob physical reads 0, lob read-ahead reads 0.
Table 'Worktable'. Scan count 0, logical reads 0, physical reads 0, read-ahead reads 0, lob logical reads 0, lob physical reads 0, lob read-ahead reads 0.

 SQL Server Execution Times:
   CPU time = 156527 ms,  elapsed time = 284906 ms.
SQL Server parse and compile time: 
   CPU time = 16 ms, elapsed time = 19 ms.

(233033 row(s) affected)
Table 'Worktable'. Scan count 0, logical reads 0, physical reads 0, read-ahead reads 0, lob logical reads 0, lob physical reads 0, lob read-ahead reads 0.
Table 'Votes'. Scan count 1, logical reads 250128, physical reads 10, read-ahead reads 250104, lob logical reads 0, lob physical reads 0, lob read-ahead reads 0.
Table 'Posts'. Scan count 1, logical reads 165586, physical reads 18, read-ahead reads 49191, lob logical reads 823463, lob physical reads 42854, lob read-ahead reads 3272.
Table 'Users'. Scan count 1, logical reads 41405, physical reads 3, read-ahead reads 41401, lob logical reads 0, lob physical reads 0, lob read-ahead reads 0.

 SQL Server Execution Times:
   CPU time = 17207 ms,  elapsed time = 38163 ms.
Msg 207, Level 16, State 1, Line 1
Invalid column name 'scores'.
SQL Server parse and compile time: 
   CPU time = 0 ms, elapsed time = 0 ms.

 SQL Server Execution Times:
   CPU time = 0 ms,  elapsed time = 0 ms.
";

            var parser = new StatisticsParser(LanguageType.English);

            var results = parser.ParseStatistics(statsString);
        }
    }
}
