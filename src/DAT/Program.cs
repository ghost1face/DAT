using DAT.AppCommand;
using DAT.CommandParser;
using DAT.Extensions;
using DAT.Logging;
using DAT.Providers.Sql;
using DAT.Results;
using DAT.Results.DataCompare;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DAT
{
    class Program
    {
        private static readonly CancellationTokenSource tokenSource = new CancellationTokenSource();

        static async Task Main(string[] args)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;

            var command = new DATCommand();

            try
            {
                Parser.ParseArguments(command, args);

                using (var logger = new SimpleLogger(command.LoggingLevel, command.LogPath))
                {
                    await RunTest(command, logger, cancellationToken: tokenSource.Token);
                }
            }
            catch (CommandParserException commandExc)
            {
                Console.WriteLine(commandExc.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(command.PrintUsage());
            }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            tokenSource.CancelAfter(2000);
        }

        private static async Task RunTest(DATCommand command, ILogger logger, CancellationToken cancellationToken)
        {
            // TODO: Simplify & Remove Repetitive Code

            logger.Log(LogLevel.Minimal, $"Beginning test with {command.TestRunConfig.ThreadCount} threads, {command.TestRunConfig.Iterations} iterations.");

            var dataCompare = command.DataCompare;
            var performanceProfile = command.PerformanceProfile;

            IEnumerable<DATCommandResult>[] test1Results;
            // Test Run #1
            {
                var testRunParams = new DATTestParameters
                {
                    ConnectionString = command.TestRunConfig.Test1ConnectionString,
                    Iterations = command.TestRunConfig.Iterations,
                    SqlQuery = ResolveQuery(command.TestRunConfig.Test1SQL)
                };

                test1Results = await RunTestGroup(testRunParams, ResolveQuery(command.TestRunConfig.PreTest1SQL), command.TestRunConfig.ThreadCount, logger, cancellationToken: cancellationToken);
            }

            IEnumerable<DATCommandResult>[] test2Results;
            // Test Run #2
            {
                var testRunParams = new DATTestParameters
                {
                    ConnectionString = command.TestRunConfig.Test2ConnectionString,
                    Iterations = command.TestRunConfig.Iterations,
                    SqlQuery = ResolveQuery(command.TestRunConfig.Test2SQL)
                };

                test2Results = await RunTestGroup(testRunParams, ResolveQuery(command.TestRunConfig.PreTest2SQL), command.TestRunConfig.ThreadCount, logger, cancellationToken: cancellationToken);
            }

            
            if (dataCompare)
            {
                var dataComparisonResult = (new List<IEnumerable<DATCommandResult>[]>() { test1Results, test2Results }).CompareTestResults();
                var bleh = false;
            }
            // now we can compare performance or data
            //
            // 
            // TODO: Compare results and determine output
            if (performanceProfile)
            {
                var percentiles = new List<int> { 50, 75, 99 };
                var test1AggregatedResults = test1Results.SelectMany(t => t)
                    .SelectMany(t => t.QueryStatistics)
                    .Cast<QueryStats>()
                    .Select(r => new QueryStatTotals
                    {
                        CPUCompileTime = r.CompileTimes.Sum(i => i.CPU),
                        ElapsedCompileTime = r.CompileTimes.Sum(i => i.Elapsed),
                        CPUExecutionTime = r.ExecutionTimes.Sum(i => i.CPU),
                        ElapsedExecutionTime = r.ExecutionTimes.Sum(i => i.Elapsed),
                        Scan = r.IOStatistics.Sum(i => i.Scan),
                        Physical = r.IOStatistics.Sum(i => i.Physical),
                        Logical = r.IOStatistics.Sum(i => i.Logical),
                        LobLogical = r.IOStatistics.Sum(i => i.LobLogical),
                        LobPhysical = r.IOStatistics.Sum(i => i.LobPhysical),
                        LobReadAhead = r.IOStatistics.Sum(i => i.LobReadAhead),
                        PercentRead = r.IOStatistics.Sum(i => i.PercentRead),
                        ReadAhead = r.IOStatistics.Sum(i => i.ReadAhead)
                    });

                var test1Totals = new QueryStatTotals
                {
                    CPUCompileTime = test1AggregatedResults.Sum(i => i.CPUCompileTime),
                    ElapsedCompileTime = test1AggregatedResults.Sum(i => i.ElapsedCompileTime),
                    CPUExecutionTime = test1AggregatedResults.Sum(i => i.CPUExecutionTime),
                    ElapsedExecutionTime = test1AggregatedResults.Sum(i => i.ElapsedExecutionTime),
                    Scan = test1AggregatedResults.Sum(i => i.Scan),
                    Physical = test1AggregatedResults.Sum(i => i.Physical),
                    Logical = test1AggregatedResults.Sum(i => i.Logical),
                    LobLogical = test1AggregatedResults.Sum(i => i.LobLogical),
                    LobPhysical = test1AggregatedResults.Sum(i => i.LobPhysical),
                    LobReadAhead = test1AggregatedResults.Sum(i => i.LobReadAhead),
                    PercentRead = test1AggregatedResults.Sum(i => i.PercentRead),
                    ReadAhead = test1AggregatedResults.Sum(i => i.ReadAhead)
                };

                var test1Averages = new QueryStatTotals
                {
                    CPUCompileTime = test1AggregatedResults.Average(i => i.CPUCompileTime),
                    ElapsedCompileTime = test1AggregatedResults.Average(i => i.ElapsedCompileTime),
                    CPUExecutionTime = test1AggregatedResults.Average(i => i.CPUExecutionTime),
                    ElapsedExecutionTime = test1AggregatedResults.Average(i => i.ElapsedExecutionTime),
                    Scan = test1AggregatedResults.Average(i => i.Scan),
                    Physical = test1AggregatedResults.Average(i => i.Physical),
                    Logical = test1AggregatedResults.Average(i => i.Logical),
                    LobLogical = test1AggregatedResults.Average(i => i.LobLogical),
                    LobPhysical = test1AggregatedResults.Average(i => i.LobPhysical),
                    LobReadAhead = test1AggregatedResults.Average(i => i.LobReadAhead),
                    PercentRead = test1AggregatedResults.Average(i => i.PercentRead),
                    ReadAhead = test1AggregatedResults.Average(i => i.ReadAhead)
                };

                var test1Percentiles = new Dictionary<int, QueryStatTotals>();
                foreach (var percentile in percentiles)
                {
                    int skipCount = (int)Math.Ceiling(((double)test1AggregatedResults.Count()) * ((double)percentile) / 100D);
                    skipCount = skipCount >= test1AggregatedResults.Count() ? test1AggregatedResults.Count() - 1 : skipCount;

                    test1Percentiles.Add(
                        percentile,
                        new QueryStatTotals
                        {
                            CPUCompileTime = test1AggregatedResults.OrderBy(i => i.CPUCompileTime).Skip(skipCount).FirstOrDefault().CPUCompileTime,
                            ElapsedCompileTime = test1AggregatedResults.OrderBy(i => i.ElapsedCompileTime).Skip(skipCount).FirstOrDefault().ElapsedCompileTime,
                            CPUExecutionTime = test1AggregatedResults.OrderBy(i => i.CPUExecutionTime).Skip(skipCount).FirstOrDefault().CPUExecutionTime,
                            ElapsedExecutionTime = test1AggregatedResults.OrderBy(i => i.ElapsedExecutionTime).Skip(skipCount).FirstOrDefault().ElapsedExecutionTime,
                            Scan = test1AggregatedResults.OrderBy(i => i.Scan).Skip(skipCount).FirstOrDefault().Scan,
                            Physical = test1AggregatedResults.OrderBy(i => i.Physical).Skip(skipCount).FirstOrDefault().Physical,
                            Logical = test1AggregatedResults.OrderBy(i => i.Logical).Skip(skipCount).FirstOrDefault().Logical,
                            LobLogical = test1AggregatedResults.OrderBy(i => i.LobLogical).Skip(skipCount).FirstOrDefault().LobLogical,
                            LobPhysical = test1AggregatedResults.OrderBy(i => i.LobPhysical).Skip(skipCount).FirstOrDefault().LobPhysical,
                            LobReadAhead = test1AggregatedResults.OrderBy(i => i.LobReadAhead).Skip(skipCount).FirstOrDefault().LobReadAhead,
                            PercentRead = test1AggregatedResults.OrderBy(i => i.PercentRead).Skip(skipCount).FirstOrDefault().PercentRead,
                            ReadAhead = test1AggregatedResults.OrderBy(i => i.ReadAhead).Skip(skipCount).FirstOrDefault().ReadAhead
                        });
                }


                // test 2
                var test2AggregatedResults = test2Results.SelectMany(t => t)
                    .SelectMany(t => t.QueryStatistics)
                    .Cast<QueryStats>()
                    .Select(r => new QueryStatTotals
                    {
                        CPUCompileTime = r.CompileTimes.Sum(i => i.CPU),
                        ElapsedCompileTime = r.CompileTimes.Sum(i => i.Elapsed),
                        CPUExecutionTime = r.ExecutionTimes.Sum(i => i.CPU),
                        ElapsedExecutionTime = r.ExecutionTimes.Sum(i => i.Elapsed),
                        Scan = r.IOStatistics.Sum(i => i.Scan),
                        Physical = r.IOStatistics.Sum(i => i.Physical),
                        Logical = r.IOStatistics.Sum(i => i.Logical),
                        LobLogical = r.IOStatistics.Sum(i => i.LobLogical),
                        LobPhysical = r.IOStatistics.Sum(i => i.LobPhysical),
                        LobReadAhead = r.IOStatistics.Sum(i => i.LobReadAhead),
                        PercentRead = r.IOStatistics.Sum(i => i.PercentRead),
                        ReadAhead = r.IOStatistics.Sum(i => i.ReadAhead)
                    });

                var test2Totals = new QueryStatTotals
                {
                    CPUCompileTime = test2AggregatedResults.Sum(i => i.CPUCompileTime),
                    ElapsedCompileTime = test2AggregatedResults.Sum(i => i.ElapsedCompileTime),
                    CPUExecutionTime = test2AggregatedResults.Sum(i => i.CPUExecutionTime),
                    ElapsedExecutionTime = test2AggregatedResults.Sum(i => i.ElapsedExecutionTime),
                    Scan = test2AggregatedResults.Sum(i => i.Scan),
                    Physical = test2AggregatedResults.Sum(i => i.Physical),
                    Logical = test2AggregatedResults.Sum(i => i.Logical),
                    LobLogical = test2AggregatedResults.Sum(i => i.LobLogical),
                    LobPhysical = test2AggregatedResults.Sum(i => i.LobPhysical),
                    LobReadAhead = test2AggregatedResults.Sum(i => i.LobReadAhead),
                    PercentRead = test2AggregatedResults.Sum(i => i.PercentRead),
                    ReadAhead = test2AggregatedResults.Sum(i => i.ReadAhead)
                };

                var test2Averages = new QueryStatTotals
                {
                    CPUCompileTime = test2AggregatedResults.Average(i => i.CPUCompileTime),
                    ElapsedCompileTime = test2AggregatedResults.Average(i => i.ElapsedCompileTime),
                    CPUExecutionTime = test2AggregatedResults.Average(i => i.CPUExecutionTime),
                    ElapsedExecutionTime = test2AggregatedResults.Average(i => i.ElapsedExecutionTime),
                    Scan = test2AggregatedResults.Average(i => i.Scan),
                    Physical = test2AggregatedResults.Average(i => i.Physical),
                    Logical = test2AggregatedResults.Average(i => i.Logical),
                    LobLogical = test2AggregatedResults.Average(i => i.LobLogical),
                    LobPhysical = test2AggregatedResults.Average(i => i.LobPhysical),
                    LobReadAhead = test2AggregatedResults.Average(i => i.LobReadAhead),
                    PercentRead = test2AggregatedResults.Average(i => i.PercentRead),
                    ReadAhead = test2AggregatedResults.Average(i => i.ReadAhead)
                };

                var test2Percentiles = new Dictionary<int, QueryStatTotals>();
                foreach (var percentile in percentiles)
                {
                    int skipCount = (int)Math.Ceiling(((double)test1AggregatedResults.Count()) * ((double)percentile) / 100D);
                    skipCount = skipCount >= test1AggregatedResults.Count() ? test1AggregatedResults.Count() - 1 : skipCount;

                    test2Percentiles.Add(
                        percentile,
                        new QueryStatTotals
                        {
                            CPUCompileTime = test2AggregatedResults.OrderBy(i => i.CPUCompileTime).Skip(skipCount).FirstOrDefault().CPUCompileTime,
                            ElapsedCompileTime = test2AggregatedResults.OrderBy(i => i.ElapsedCompileTime).Skip(skipCount).FirstOrDefault().ElapsedCompileTime,
                            CPUExecutionTime = test2AggregatedResults.OrderBy(i => i.CPUExecutionTime).Skip(skipCount).FirstOrDefault().CPUExecutionTime,
                            ElapsedExecutionTime = test2AggregatedResults.OrderBy(i => i.ElapsedExecutionTime).Skip(skipCount).FirstOrDefault().ElapsedExecutionTime,
                            Scan = test2AggregatedResults.OrderBy(i => i.Scan).Skip(skipCount).FirstOrDefault().Scan,
                            Physical = test2AggregatedResults.OrderBy(i => i.Physical).Skip(skipCount).FirstOrDefault().Physical,
                            Logical = test2AggregatedResults.OrderBy(i => i.Logical).Skip(skipCount).FirstOrDefault().Logical,
                            LobLogical = test2AggregatedResults.OrderBy(i => i.LobLogical).Skip(skipCount).FirstOrDefault().LobLogical,
                            LobPhysical = test2AggregatedResults.OrderBy(i => i.LobPhysical).Skip(skipCount).FirstOrDefault().LobPhysical,
                            LobReadAhead = test2AggregatedResults.OrderBy(i => i.LobReadAhead).Skip(skipCount).FirstOrDefault().LobReadAhead,
                            PercentRead = test2AggregatedResults.OrderBy(i => i.PercentRead).Skip(skipCount).FirstOrDefault().PercentRead,
                            ReadAhead = test2AggregatedResults.OrderBy(i => i.ReadAhead).Skip(skipCount).FirstOrDefault().ReadAhead
                        });
                }

                test1Totals.TestRunIdentifier = "Test 1";
                test2Totals.TestRunIdentifier = "Test 2";

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(test1Totals.ToString());
                Console.ResetColor();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.Write(test2Totals.ToString());
                Console.ResetColor();
            }
        }

        private static async Task<IEnumerable<DATCommandResult>[]> RunTestGroup(DATTestParameters testRunParams, string preTestSql, int threadCount, ILogger logger, CancellationToken cancellationToken)
        {
            var tasks = new List<Task<IEnumerable<DATCommandResult>>>();

            // Execute Pre-Test Script/Command
            logger.Log(LogLevel.Minimal, "Executing pre-test script command...");
            if (!string.IsNullOrWhiteSpace(preTestSql))
                await RunSqlQuery(preTestSql, testRunParams.ConnectionString, cancellationToken: cancellationToken);

            for (int i = 0; i < threadCount; i++)
                tasks.Add(RunThreadTest(testRunParams, logger, i, cancellationToken: cancellationToken));

            return await Task.WhenAll(tasks);
        }

        private static async Task<IEnumerable<DATCommandResult>> RunThreadTest(DATTestParameters parameters, ILogger logger, int threadNumber, CancellationToken cancellationToken)
        {
            int iterations = parameters.Iterations;

            var tasks = new List<Task<DATCommandResult>>();
            for (int i = 0; i < iterations; i++)
            {
                logger.Log(LogLevel.Detailed, $"Running query {i} for thread {threadNumber}");

                tasks.Add(RunSqlQuery(parameters.SqlQuery, parameters.ConnectionString, cancellationToken: cancellationToken));
            }

            var queryResults = await Task.WhenAll(tasks);

            return queryResults.ToList();
        }

        private static async Task<DATCommandResult> RunSqlQuery(string query, string connectionString, CancellationToken cancellationToken)
        {
            var result = new DATCommandResult
            {
                QueryStatistics = new List<object>(),
                ResultSets = new List<List<ExpandoObject>>()
            };

            using (var dbConnectionWrapper = new SqlConnectionWrapper(connectionString))
            using (var dbCommandWrapper = dbConnectionWrapper.CreateCommand())
            {
                dbCommandWrapper.CommandType = CommandType.Text;
                dbCommandWrapper.CommandText = query;

                await dbConnectionWrapper.OpenAsync(cancellationToken: cancellationToken);

                using (DbDataReader reader = await dbCommandWrapper.ExecuteReaderAsync(cancellationToken: cancellationToken))
                {
                    do
                    {


                        var resultSet = new List<ExpandoObject>();
                        while (await reader.ReadAsync(cancellationToken: cancellationToken))
                        {
                            var record = new Dictionary<string, object>();
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                var fieldName = reader.GetName(i);
                                var fieldValue = reader.GetValue(i);
                                record.Add(fieldName, fieldValue);
                            }

                            resultSet.Add(record.ToExpando());
                        }
                        result.ResultSets.Add(resultSet);
                    }
                    while (await reader.NextResultAsync(cancellationToken: cancellationToken));
                }

                result.QueryStatistics = dbCommandWrapper.RetrieveStats().ToList();
            }

            return result;
        }

        private static string ResolveQuery(string pathOrQuery)
        {
            if (File.Exists(pathOrQuery))
                return File.ReadAllText(pathOrQuery);
            return pathOrQuery;
        }
    }
}
