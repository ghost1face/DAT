using DAT.AppCommand;
using DAT.CommandParser;
using DAT.Logging;
using System;
using System.Collections.Generic;
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

            var tasks = new List<Task>();
            for (int i = 0; i < command.TestRunConfig.ThreadCount; i++)
            {
                tasks.Add(RunThreadTest(command, logger));
            }

            await Task.WhenAll(tasks);
        }

        static async Task RunThreadTest(DATCommand command, ILogger logger)
        {
            bool performanceTest = command.PerformanceProfile;
            bool dataCompare = command.DataCompare;
            int iterations = command.TestRunConfig.Iterations;

            var tasks = new List<Task>();
            for (int i = 0; i < iterations; i++)
            {
                tasks.Add(RunSqlQuery());
            }

            await Task.WhenAll(tasks);
        }

        static Task RunSqlQuery()
        {
            return Task.FromResult(0);
        }
    }
}
