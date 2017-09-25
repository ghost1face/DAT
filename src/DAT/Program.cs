using DAT.AppCommand;
using DAT.CommandParser;
using DAT.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
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

            // Test Run #1
            {
                var tasks = new List<Task<List<DATCommandResult>>>();
                var testRunParams = new DATTestParameters
                {
                    ConnectionString = command.TestRunConfig.Test1ConnectionString,
                    DataCompare = command.DataCompare,
                    Iterations = command.TestRunConfig.Iterations,
                    PerformanceProfile = command.PerformanceProfile,
                    SqlQuery = ResolveQuery(command.TestRunConfig.Test1SQL)
                };

                for (int i = 0; i < command.TestRunConfig.ThreadCount; i++)
                {
                    tasks.Add(RunThreadTest(testRunParams, logger, cancellationToken: cancellationToken));
                }

                await Task.WhenAll(tasks);
            }

            // Test Run #2
            {
                var tasks = new List<Task<List<DATCommandResult>>>();
                var testRunParams = new DATTestParameters
                {
                    ConnectionString = command.TestRunConfig.Test2ConnectionString,
                    DataCompare = command.DataCompare,
                    Iterations = command.TestRunConfig.Iterations,
                    PerformanceProfile = command.PerformanceProfile,
                    SqlQuery = ResolveQuery(command.TestRunConfig.Test2SQL)
                };

                for (int i = 0; i < command.TestRunConfig.ThreadCount; i++)
                {
                    tasks.Add(RunThreadTest(testRunParams, logger, cancellationToken: cancellationToken));
                }

                await Task.WhenAll(tasks);
            }

            // now we can compare performance or data
        }

        private static async Task<List<DATCommandResult>> RunThreadTest(DATTestParameters parameters, ILogger logger, CancellationToken cancellationToken)
        {
            bool performanceTest = parameters.PerformanceProfile;
            bool dataCompare = parameters.DataCompare;
            int iterations = parameters.Iterations;

            var tasks = new List<Task<DATCommandResult>>();
            for (int i = 0; i < iterations; i++)
            {
                tasks.Add(RunSqlQuery(parameters.SqlQuery, parameters.ConnectionString, performanceTest, dataCompare, cancellationToken: cancellationToken));
            }

            await Task.WhenAll(tasks);

            return tasks.Select(i => i.Result).ToList();
        }

        private static async Task<DATCommandResult> RunSqlQuery(string query, string connectionString, bool performanceTest, bool dataCompare, CancellationToken cancellationToken)
        {
            var result = new DATCommandResult
            {
                PerformanceResults = new Dictionary<string, object>(),
                ResultSets = new List<List<Dictionary<string, object>>>()
            };

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = query;

                // TODO: might not need this part
                if (performanceTest)
                {
                    connection.StatisticsEnabled = true;

                    connection.InfoMessage += new SqlInfoMessageEventHandler((sender, data) =>
                    {
                        result.PerformanceResults.Add(data.Source, data.Message);
                    });
                }

                await connection.OpenAsync(cancellationToken: cancellationToken);

                using (DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken: cancellationToken))
                {
                    do
                    {
                        var resultSet = new List<Dictionary<string, object>>();
                        while (await reader.ReadAsync(cancellationToken: cancellationToken))
                        {
                            var record = new Dictionary<string, object>();
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                var fieldName = reader.GetName(i);
                                var fieldValue = reader.GetValue(i);
                                record.Add(fieldName, fieldValue);
                            }

                            resultSet.Add(record);
                        }
                        result.ResultSets.Add(resultSet);

                    } while (await reader.NextResultAsync(cancellationToken: cancellationToken));
                }

                // TODO: Need parsing here ?
                if (performanceTest)
                {
                    var stats = connection.RetrieveStatistics();

                    foreach (string statKey in stats.Keys)
                    {
                        result.PerformanceResults.Add(statKey, stats[statKey]);
                    }
                }
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
