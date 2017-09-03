using System;
using System.Collections.Generic;
using System.IO;


namespace DAT.CommandParser
{
    public static class Parser
    {
        public static void ParseArguments(Command command, string[] args, TextWriter outputWriter)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (outputWriter == null)
                throw new ArgumentNullException(nameof(outputWriter));

            
        }
    }
}