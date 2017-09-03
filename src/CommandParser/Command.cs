using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAT.CommandParser
{
    public abstract class Command
    {
        protected readonly IList<Option> options;
        protected readonly IList<Rule> rules;

        protected Command()
        {
            options = new List<Option>();
            rules = new List<Rule>();
        }

        public IList<Option> Options
        {
            get { return options; }
        }

        public IList<Rule> Rules
        {
            get { return rules; }
        }

        public string ApplicationName { get; set; }

        protected void HasOption(string parameter, Action<string> handler, string description, bool required = false)
        {
            var option = new Option
            {
                Parameter = parameter,
                Description = description,
                Handler = handler,
                IsRequired = required
            };

            options.Add(option);
        }

        protected void HasRequiredOption(string parameter, Action<string> handler, string description)
        {
            HasOption(parameter, handler, description, required: true);
        }

        protected void HasRule(Func<bool> validatorFunc, string errorMessage)
        {
            var rule = new Rule
            {
                Validator = validatorFunc,
                ValidationError = errorMessage
            };

            rules.Add(rule);
        }

        public virtual string PrintUsage()
        {
            return new StringBuilder()
                .AppendLine($"Usage: {ApplicationName} {GetOptionParametersForShortUsage()}")
                .AppendLine()
                .AppendLine()
                .AppendLine("Options:")
                .AppendLine(GetOptionParameterLongDescriptions())
                .ToString();
        }

        private string GetOptionParametersForShortUsage()
        {
            return new StringBuilder()
                .AppendJoin(" ", options.Select(opt => $"[{opt.Parameter}]"))
                .ToString();
        }

        private string GetOptionParameterLongDescriptions()
        {
            return new StringBuilder()
                .AppendJoin(Environment.NewLine, options.Select(GetOptionParameterDescription))
                .ToString();
        }

        private string GetOptionParameterDescription(Option opt)
        {
            return new StringBuilder()
                .Append($"\t{string.Join(" ", opt.Parameter.Split("|", StringSplitOptions.RemoveEmptyEntries))}")
                .Append("\t\t")
                .Append(opt.Description)
                .ToString();

        }
    }
}