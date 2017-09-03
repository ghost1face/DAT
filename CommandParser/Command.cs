using System;
using System.Collections.Generic;


namespace DAT.CommandParser
{
    public abstract class Command
    {
        protected internal IList<Option> options;

        protected Command()
        {
            options = new List<Option>();
        }

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
    }
}