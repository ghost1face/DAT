using System;

namespace DAT.CommandParser
{
    public class CommandParserException : Exception
    {
        public CommandParserException(Command command)
            : this(command.PrintUsage())
        {

        }

        public CommandParserException(Command command, string message)
            : base($"{message}{Environment.NewLine}{Environment.NewLine}{command.PrintUsage()}")
        {

        }

        public CommandParserException(string message)
            : base(message)
        {

        }
    }
}
