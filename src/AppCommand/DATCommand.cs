using DAT.CommandParser;
using DAT.Logging;
using System;
using System.IO;
using System.Text;

namespace DAT.AppCommand
{
    public sealed class DATCommand : Command, IDisposable
    {
        private bool disposed;

        public bool PerformanceProfile { get; private set; }

        public bool DataCompare { get; private set; }

        public LogLevel LoggingLevel { get; private set; }

        public TextWriter LogWriter { get; private set; }

        public DATCommand()
        {
            LoggingLevel = LogLevel.Minimal;

            ApplicationName = "dat";

            HasOption("-p|--perf", param => PerformanceProfile = true, "Performance profile queries.");

            HasOption("-c|--compare", param => DataCompare = true, "Compare output from two queries to validate changes made still produce the same output.");

            HasOption("-v|--verbosity", param =>
            {
                if (Enum.TryParse(param, true, out LogLevel result))
                    LoggingLevel = result;
                else
                {
                    if (param.Equals("q", StringComparison.OrdinalIgnoreCase))
                        LoggingLevel = LogLevel.Quiet;
                    else if (param.Equals("m", StringComparison.OrdinalIgnoreCase))
                        LoggingLevel = LogLevel.Minimal;
                    else if (param.Equals("d", StringComparison.OrdinalIgnoreCase))
                        LoggingLevel = LogLevel.Detailed;
                    else if (param.Equals("diag", StringComparison.OrdinalIgnoreCase))
                        LoggingLevel = LogLevel.Diagnostic;
                }

            }, "Specifies the amount of information to display in the log.  You can specify the following verbosity levels: q[uiet], m[inimal], d[etailed], diag[nostic]");

            HasRule(() => PerformanceProfile || DataCompare, "Performance profile and/or Data compare must be specified.");
        }

        public override string PrintUsage()
        {
            var usageBuilder = new StringBuilder("DAT Tool v??")
                .AppendLine()
                .AppendLine(base.PrintUsage());

            return usageBuilder.ToString();
        }

        ~DATCommand()
        {
            Dispose(false);
        }

        public void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                disposed = true;

                if (LogWriter != null)
                    LogWriter.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
