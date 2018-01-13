using DAT.CommandParser;
using DAT.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace DAT.AppCommand
{
    // TODO: Move strings to resources file
    // TODO: Add DATTestModel here, can either be parsed from an optionsFile directive or from individual directives
    public sealed class DATCommand : Command
    {
        public bool PerformanceProfile { get; private set; }

        public bool DataCompare { get; private set; }

        public LogLevel LoggingLevel { get; private set; }

        public string LogPath { get; private set; }

        public DATTestConfig TestRunConfig { get; private set; }

        public DATCommand()
        {
            SetDefaults();

            ConfigureOptions();
            ConfigureRules();
        }

        public override string PrintUsage()
        {
            var usageBuilder = new StringBuilder("DAT Tool v??")
                .AppendLine()
                .AppendLine(base.PrintUsage());

            return usageBuilder.ToString();
        }

        private void SetDefaults()
        {
            LoggingLevel = LogLevel.Detailed;
            ApplicationName = "dat";
            LogPath = Path.Combine(Environment.CurrentDirectory, $"{ApplicationName}.log");
            TestRunConfig = null;
        }

        private void ConfigureOptions()
        {
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

            HasOption("-l|--logPath", param => LogPath = param, $"The path to log based on the specified verbosity, default is the current directory {ApplicationName}.log");

            HasOption("-d|--datFile", param =>
            {
                var filePath = param;
                if (!File.Exists(filePath))
                    throw new Exception("datfile does not exist.");

                var configFileContents = File.ReadAllText(filePath);

                TestRunConfig = JsonConvert.DeserializeObject<DATTestConfig>(configFileContents);
            }, "Specifies the configuration file to execute");
        }

        private void ConfigureRules()
        {
            HasRule(() => TestRunConfig != null, "Option -d|--datFile must be provided");
            HasRule(() => PerformanceProfile || DataCompare, "Performance profile and/or Data compare must be specified.");
        }
    }
}
