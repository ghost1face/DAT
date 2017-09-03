using DAT.AppCommand;
using DAT.CommandParser;
using DAT.Logging;
using System;

namespace DAT
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var command = new DATCommand();

                Parser.ParseArguments(command, args);

                using (var logger = new SimpleLogger(command.LoggingLevel, command.LogPath))
                {
                    RunTest(command, logger);
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

        static void RunTest(DATCommand command, ILogger logger)
        {
            // create threads with delegate
              // each delegate iterate for iterations variable
                // parse performance stats, if compare perform data compare

        }
    }
}
