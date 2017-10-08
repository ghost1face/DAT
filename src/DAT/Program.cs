using DAT.AppCommand;
using DAT.CommandParser;
using DAT.Extensions;
using DAT.Logging;
using DAT.Providers.Sql;
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

            try
            {
                var command = new DATCommand();

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

            }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            tokenSource.CancelAfter(2000);
        }

        private static async Task RunTest(DATCommand command, ILogger logger, CancellationToken cancellationToken)
        {
            // create threads with delegate
            // each delegate iterate for iterations variable
            // parse performance stats, if compare perform data compare

            logger.Log(LogLevel.Detailed, $"Beginning test with {command.TestRunConfig.ThreadCount} threads, {command.TestRunConfig.Iterations} iterations.");

            var dataCompare = command.DataCompare;
            var performanceProfile = command.PerformanceProfile;

            IEnumerable<DATCommandResult>[] test1Results;
            // Test Run #1
            {
                var tasks = new List<Task<IEnumerable<DATCommandResult>>>();
                var testRunParams = new DATTestParameters
                {
                    ConnectionString = command.TestRunConfig.Test1ConnectionString,
                    Iterations = command.TestRunConfig.Iterations,
                    SqlQuery = ResolveQuery(command.TestRunConfig.Test1SQL)
                };

                for (int i = 0; i < command.TestRunConfig.ThreadCount; i++)
                {
                    tasks.Add(RunThreadTest(testRunParams, logger, cancellationToken: cancellationToken));
                }

                test1Results = await Task.WhenAll(tasks);
            }

            IEnumerable<DATCommandResult>[] test2Results;
            // Test Run #2
            {
                var tasks = new List<Task<IEnumerable<DATCommandResult>>>();
                var testRunParams = new DATTestParameters
                {
                    ConnectionString = command.TestRunConfig.Test2ConnectionString,
                    Iterations = command.TestRunConfig.Iterations,
                    SqlQuery = ResolveQuery(command.TestRunConfig.Test2SQL)
                };

                for (int i = 0; i < command.TestRunConfig.ThreadCount; i++)
                {
                    tasks.Add(RunThreadTest(testRunParams, logger, cancellationToken: cancellationToken));
                }

                test2Results = await Task.WhenAll(tasks);
            }

            // now we can compare performance or data
            //
            // 
            // TODO: Compare results and determine output
            if (performanceProfile)
            {
                var test1Totals = test1Results.SelectMany(t => t)
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
                    })
                    .Aggregate((accumulator, next) =>
                    {
                        accumulator.CPUCompileTime += next.CPUCompileTime;
                        accumulator.ElapsedCompileTime += next.ElapsedCompileTime;
                        accumulator.CPUExecutionTime += next.CPUExecutionTime;
                        accumulator.ElapsedExecutionTime += next.ElapsedExecutionTime;
                        accumulator.Scan += next.Scan;
                        accumulator.Physical += next.Physical;
                        accumulator.Logical += next.Logical;
                        accumulator.LobLogical += next.LobLogical;
                        accumulator.LobPhysical += next.LobPhysical;
                        accumulator.LobReadAhead += next.LobReadAhead;
                        accumulator.PercentRead += next.PercentRead;
                        accumulator.ReadAhead += next.ReadAhead;

                        return accumulator;
                    });

                var test2Totals = test2Results.SelectMany(t => t)
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
                    })
                    .Aggregate((accumulator, next) =>
                    {
                        accumulator.CPUCompileTime += next.CPUCompileTime;
                        accumulator.ElapsedCompileTime += next.ElapsedCompileTime;
                        accumulator.CPUExecutionTime += next.CPUExecutionTime;
                        accumulator.ElapsedExecutionTime += next.ElapsedExecutionTime;
                        accumulator.Scan += next.Scan;
                        accumulator.Physical += next.Physical;
                        accumulator.Logical += next.Logical;
                        accumulator.LobLogical += next.LobLogical;
                        accumulator.LobPhysical += next.LobPhysical;
                        accumulator.LobReadAhead += next.LobReadAhead;
                        accumulator.PercentRead += next.PercentRead;
                        accumulator.ReadAhead += next.ReadAhead;

                        return accumulator;
                    });

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

        private static async Task<IEnumerable<DATCommandResult>> RunThreadTest(DATTestParameters parameters, ILogger logger, CancellationToken cancellationToken)
        {
            int iterations = parameters.Iterations;

            var tasks = new List<Task<DATCommandResult>>();
            for (int i = 0; i < iterations; i++)
            {
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
