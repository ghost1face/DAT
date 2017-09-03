using DAT.AppCommand;
using DAT.CommandParser;
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

                RunTest(command);
            }
            catch (CommandParserException commandExc)
            {
                Console.WriteLine(commandExc.Message);
            }
            catch (Exception ex)
            {

            }
        }

        static void RunTest(DATCommand command)
        {

        }
    }
}
