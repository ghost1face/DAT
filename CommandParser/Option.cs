using System;

namespace DAT.CommandParser
{
    public class Option
    {
        public string Parameter { get; set; }
        public Action<string> Handler { get; set; }
        public string Description { get; set; }
        public bool IsRequired { get; set; }
    }
}