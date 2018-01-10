using System;
using System.IO;

namespace DAT.Logging
{
    public class SimpleLogger : ILogger, IDisposable
    {
        private readonly TextWriter textWriter;
        private readonly LogLevel loggerLevel;
        private readonly object syncLock;

        public SimpleLogger(LogLevel logLevel, string logPath)
        {
            loggerLevel = logLevel;
            textWriter = File.CreateText(logPath);
            syncLock = new object();
        }

        public void Log(LogLevel level, string message)
        {
            if ((int)loggerLevel >= (int)level)
            {
                lock (syncLock)
                {
                    Console.WriteLine($"{DateTime.Now}\t{level}\t{message}"); // write to console for now as well

                    textWriter.WriteLine($"{DateTime.Now}\t{level}\t{message}");
                    textWriter.Flush();
                }
            }
        }

        public void Dispose()
        {
            if (textWriter != null)
                textWriter.Dispose();
        }
    }
}
