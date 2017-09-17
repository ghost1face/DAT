using DAT.AppCommand;
using DAT.CommandParser;
using DAT.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DAT
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var command = new DATCommand();

                Parser.ParseArguments(command, args);

                using (var logger = new SimpleLogger(command.LoggingLevel, command.LogPath))
                {
                    await RunTest(command, logger);
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

        static async Task RunTest(DATCommand command, ILogger logger)
        {
            // create threads with delegate
            // each delegate iterate for iterations variable
            // parse performance stats, if compare perform data compare

            logger.Log(LogLevel.Detailed, $"Beginning test with {command.TestRunConfig.ThreadCount} threads, {command.TestRunConfig.Iterations} iterations.");

            // Test Run #1
            {
                var tasks = new List<Task>();
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
                    tasks.Add(RunThreadTest(testRunParams, logger));
                }

                await Task.WhenAll(tasks);
            }

            // Test Run #2
            {
                var tasks = new List<Task>();
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
                    tasks.Add(RunThreadTest(testRunParams, logger));
                }

                await Task.WhenAll(tasks);
            }
        }

        static async Task RunThreadTest(DATTestParameters parameters, ILogger logger)
        {
            bool performanceTest = parameters.PerformanceProfile;
            bool dataCompare = parameters.DataCompare;
            int iterations = parameters.Iterations;

            var tasks = new List<Task>();
            for (int i = 0; i < iterations; i++)
            {
                tasks.Add(RunSqlQuery(parameters.SqlQuery, parameters.ConnectionString, performanceTest, dataCompare));
            }

            await Task.WhenAll(tasks);
        }

        static Task RunSqlQuery(string query, string connectionString, bool performanceTest, bool dataCompare)
        {
            return Task.FromResult(0);
        }

        static string ResolveQuery(string pathOrQuery)
        {
            if (File.Exists(pathOrQuery))
                return File.ReadAllText(pathOrQuery);
            return pathOrQuery;
        }
    }
}
