using System;

namespace DAT.CommandParser
{
    public class Rule
    {
        public Func<bool> Validator { get; set; }

        public string ValidationError { get; set; }
    }
}
