using System;
using System.IO;

namespace DAT.Logging
{
    public class SimpleLogger : ILogger, IDisposable
    {
        private readonly TextWriter textWriter;
        private readonly LogLevel loggerLevel;

        public SimpleLogger(LogLevel logLevel, string logPath)
        {
            loggerLevel = logLevel;
            textWriter = File.CreateText(logPath);
        }

        public void Log(LogLevel level, string message)
        {
            if ((int)loggerLevel >= (int)level)
                textWriter.WriteLine($"{DateTime.Now}\t{level}\t{message}");
        }

        public void Dispose()
        {
            if (textWriter != null)
                textWriter.Dispose();
        }
    }
}
