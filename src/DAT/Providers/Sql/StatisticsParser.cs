using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

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

        public IEnumerable<QueryStats> ParseStatistics(string txt)
        {
            var lines = txt.Split(Environment.NewLine);
            var isExecution = false;
            var inTable = false;
            var isCompile = false;
            var isError = false;
            //var tableCount = 0;
            var queryStatistics = new List<QueryStats>();
            //var tableIOResult = new List<IOStats>();
            //var executionTimes = new List<TimeStats>();
            //var compileTimes = new List<TimeStats>();

            var rowType = StatisticsRowType.None;
            var queryStats = NewStats();
            foreach (var line in lines)
            {
                if (!isExecution && !isCompile && !isError)
                {
                    rowType = DetermineRowType(line);
                }

                switch (rowType)
                {
                    case StatisticsRowType.IO:
                        if (inTable)
                        {
                            ProcessIOTableRow(line, queryStats.IOStatistics);
                        }
                        else
                        {
                            //tableCount++;
                            inTable = true;
                            ProcessIOTableRow(line, queryStats.IOStatistics);
                        }
                        break;
                    case StatisticsRowType.ExecutionTime:
                        if (isExecution)
                        {
                            var et = ProcessTime(line);
                            if (queryStatistics.Count == 0)
                                queryStats.ExecutionTimes.Add(et);
                            else
                                queryStatistics[queryStatistics.Count - 1].ExecutionTimes.Add(et);
                        }

                        isExecution = !isExecution;
                        break;
                    case StatisticsRowType.CompileTime:
                        if (isCompile)
                        {
                            var ct = ProcessTime(line);
                            queryStats.CompileTimes.Add(ct);
                        }

                        isCompile = !isCompile;
                        break;
                    case StatisticsRowType.RowsAffected:
                        var re = new Regex("\\d+");
                        var matches = re.Matches(line);
                        if (matches.Count > 0)
                        {
                            long.TryParse(matches[0].Value, out long rowsAffected);
                            queryStats.RowsAffected = rowsAffected;
                        }

                        break;
                    case StatisticsRowType.Error:
                        if (inTable)
                            inTable = false;

                        if (!IsEmptyQueryStats(queryStats))
                        {
                            queryStatistics.Add(queryStats);
                            queryStats = NewStats();
                        }

                        isError = (!isError ? true : false);
                        if (queryStats.Error == null)
                            queryStats.Error = line;
                        else
                            queryStats.Error += $"{Environment.NewLine}{line}";
                        break;

                    case StatisticsRowType.None:
                    default:
                        if (inTable)
                        {
                            inTable = false;
                            queryStatistics.Add(queryStats);
                            queryStats = NewStats();
                        }
                        break;
                }
            }

            if (!queryStatistics.Contains(queryStats))
                queryStatistics.Add(queryStats);

            return queryStatistics;
        }

        private QueryStats NewStats()
        {
            return new QueryStats
            {
                CompileTimes = new List<TimeStats>(),
                ExecutionTimes = new List<TimeStats>(),
                IOStatistics = new List<IOStats>()
            };
        }

        private void ProcessIOTableRow(string line, IList<IOStats> tableResult)
        {
            var section = line.Split(".");
            var tableName = GetSubstring(section[0], "'");
            var tableData = section[1];

            if (tableData != null)
            {
                if (tableData == string.Empty)
                {
                    var statLineInfo = new IOStats(tableResult.Count, line)
                    {
                        NoStats = true
                    };

                    tableResult.Add(statLineInfo);
                }

                var stat = Regex.Split(tableData, "[,]+");
                var statInfo = new IOStats(
                    rowNumber: tableResult.Count + 1,
                    tableName: tableName,
                    scan: InfoReplace(stat[0], language.Scan, string.Empty),
                    logical: InfoReplace(stat[1], language.Logical, string.Empty),
                    physical: InfoReplace(stat[2], language.Physical, string.Empty),
                    readAhead: InfoReplace(stat[3], language.ReadAhead, string.Empty),
                    lobLogical: InfoReplace(stat[4], language.LOBLogical, string.Empty),
                    lobPhysical: InfoReplace(stat[5], language.LOBPhysical, string.Empty),
                    lobReadAhead: InfoReplace(stat[6], language.LOBReadAhead, string.Empty)
                );

                tableResult.Add(statInfo);
            }
            else
            {
                if (line.Length > 0)
                {
                    var statLineInfo = new IOStats(tableResult.Count + 1, line)
                    {
                        NoStats = true
                    };

                    tableResult.Add(statLineInfo);
                }
            }
        }

        private bool IsEmptyQueryStats(QueryStats stats)
        {
            return stats == null ||
                (
                    stats.CompileTimes.Count == 0
                    && stats.ExecutionTimes.Count == 0
                    && stats.IOStatistics.Count == 0
                    && stats.RowsAffected == 0
                );
        }

        private TimeStats ProcessTime(string line)
        {
            var section = line.Split(",");

            var re = ProcessTimeRegEx(language.CPUTime, language.Milliseconds);
            var re2 = ProcessTimeRegEx(language.ElapsedTime, language.Milliseconds);

            return new TimeStats(
                 cpu: int.Parse(re.Replace(section[0], (m) => m.Groups[2].Value)),
                 elapsed: int.Parse(re2.Replace(section[1], (m) => m.Groups[2].Value))
            );
        }

        private Regex ProcessTimeRegEx(string preText, string postText)
        {
            return new Regex("(.*" + preText + "+)(.*?)(\\s+" + postText + ".*)");
        }

        private StatisticsRowType DetermineRowType(string line)
        {
            var rowType = StatisticsRowType.None;
            if (string.IsNullOrEmpty(line))
            {
                rowType = StatisticsRowType.None;
            }
            else if (line.Substring(0, 7) == language.Table)
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

        private int InfoReplace(string input, string searchValue, string newValue)
        {
            var returnValue = 0;
            if (input != null)
            {
                int.TryParse(input.Replace(searchValue, newValue), out returnValue);
            }

            return returnValue;
        }

    }
}
