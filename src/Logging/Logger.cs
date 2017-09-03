using System;
using System.IO;

namespace DAT.Logging
{
    public class Logger
    {
        public void Log(LogLevel level, string message, TextWriter textWriter)
        {
            textWriter.WriteLine($"{DateTime.Now}\t{level}\t{message}");
        }
    }
}
