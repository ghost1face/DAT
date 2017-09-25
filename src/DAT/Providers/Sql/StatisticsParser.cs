using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DAT.Providers.Sql
{
    public class StatisticsParser
    {
        private readonly Language language;
        private readonly LanguageType languageType;

        public StatisticsParser()
        {
            languageType = LanguageType.English;
            language = new Language(languageType);
        }

        public StatisticsParser(LanguageType languageType)
        {
            language = new Language(languageType);
            this.languageType = languageType;
        }

        public void ParseStatistics(string txt)
        {
            var lines = txt.Split(Environment.NewLine);
            var isExecution = false;
            var inTable = false;
            var isCompile = false;
            var isError = false;
            var tableCount = 0;
            var tableIOResult = new List<object>();

            foreach (var line in lines)
            {
                var rowType = StatisticsRowType.None;
                if (!isExecution && !isCompile && !isError)
                {
                    rowType = DetermineRowType(line);
                }

                switch (rowType)
                {
                    case StatisticsRowType.IO:
                        if (inTable)
                        {

                        }
                        else
                        {
                            tableCount++;
                            inTable = true;

                        }
                        break;
                    case StatisticsRowType.ExecutionTime:
                        break;
                    case StatisticsRowType.CompileTime:
                        break;
                    case StatisticsRowType.RowsAffected:
                        break;
                    case StatisticsRowType.Error:
                        break;
                    default:
                    case StatisticsRowType.None:
                        break;
                }
            }
        }

        private void ProcessIOTableRow(string line, IList<object> tableResult)
        {
            var section = line.Split(".");
            var tableName = GetSubstring(section[0], "'");
            var tableData = section[1];


        }

        private StatisticsRowType DetermineRowType(string line)
        {
            var rowType = StatisticsRowType.None;
            if (line.Substring(0, 7) == language.Table)
            {
                rowType = StatisticsRowType.IO;
            }
            else if (line.Trim() == language.ExecutionTime)
            {
                rowType = StatisticsRowType.ExecutionTime;
            }
            else if (line.Trim() == language.CompileTime)
            {
                rowType = StatisticsRowType.CompileTime;
            }
            else if (line.IndexOf(language.RowsAffected) > -1)
            {
                rowType = StatisticsRowType.RowsAffected;
            }
            else if (line.Substring(0, 3) == language.ErrorMsg)
            {
                rowType = StatisticsRowType.Error;
            }

            return rowType;
        }

        private string GetSubstring(string str, string delim)
        {
            var a = str.IndexOf(delim);
            if (a == -1)
                return string.Empty;
            var b = str.IndexOf(delim, a + 1);

            if (b == -1)
                return string.Empty;

            return str.Substring(a + 1, b - a - 1);
        }

    }
}
