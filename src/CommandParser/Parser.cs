using System;
using System.Linq;

namespace DAT.CommandParser
{
    public static class Parser
    {
        public static void ParseArguments(Command command, string[] args)
        {
            try
            {
                if (command == null)
                    throw new ArgumentNullException(nameof(command));

                foreach (var arg in args)
                {
                    var optionParameters = arg.Split("=", StringSplitOptions.RemoveEmptyEntries);

                    var option = FindValidOption(command, optionParameters[0]);
                    if (option == null)
                        continue;

                    var optionValue = optionParameters.Length == 2 ? optionParameters[1] : null;
                    option.Handler.Invoke(optionValue);
                }

                if (command.Rules.Any())
                {
                    foreach (var rule in command.Rules)
                    {
                        if (!rule.Validator())
                            throw new CommandParserException(command, rule.ValidationError);
                    }
                }
            }
            catch (CommandParserException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new CommandParserException(command);
            }
        }

        private static Option FindValidOption(Command command, string option)
        {
            return command.Options.FirstOrDefault(opt => SplitParameter(opt.Parameter).Any(p => p.Equals(option)));
        }

        private static string[] SplitParameter(string parameter)
        {
            return parameter.Split("|", StringSplitOptions.RemoveEmptyEntries);
        }
    }
}