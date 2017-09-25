﻿using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace DAT.Providers.Sql
{
    public class Language
    {
        private dynamic deserializedLang;

        public Language(LanguageType languageType)
        {
            this.LangValue = languageType;
            this.deserializedLang = JObject.Parse(LoadLanguageResource(languageType));
        }

        private string LoadLanguageResource(LanguageType type)
        {
            string language = null;
            switch (type)
            {
                case LanguageType.Spanish:
                    language = "es";
                    break;
                case LanguageType.English:
                default:
                    language = "en";
                    break;
            }

            var languageResourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Providers/Sql/Resources", $"{language}.json");
            return File.ReadAllText(languageResourcePath);
        }

        public LanguageType LangValue { get; private set; }
        public string Table => deserializedLang.table;
        public string Scan => deserializedLang.scan;
        public string Logical => deserializedLang.logical;
        public string Physical => deserializedLang.physical;
        public string ReadAhead => deserializedLang.readahead;
        public string LOBLogical => deserializedLang.loblogical;
        public string LOBPhysical => deserializedLang.lobphysical;
        public string LOBReadAhead => deserializedLang.lobreadahead;
        public string ExecutionTime => deserializedLang.executiontime;
        public string CompileTime => deserializedLang.compiletime;
        public string RowsAffected => deserializedLang.rowsaffected;
        public string ErrorMsg => deserializedLang.errormsg;
        public string CPUTime => deserializedLang.cputime;
        public string ElapsedTime => deserializedLang.elapsedtime;
        public string Milliseconds => deserializedLang.milliseconds;
        public string HeaderRowsAffected => deserializedLang.headerrowsaffected;
        //public string HeaderRowNum => deserializedLang.headerrownum;
        //public string HeaderTable => deserializedLang.headertable;
        //public string HeaderScan => deserializedLang.headerscan;
        //public string HeaderLogical => deserializedLang.headerlogical;
        //public 

        //langvalue : "en",
        //langname : "English" ,
        //table : "Table '",
        //scan : "Scan count ",
        //logical : "logical reads ",
        //physical : "physical reads ",
        //readahead : "read-ahead reads ",
        //loblogical : "lob logical reads ",
        //lobphysical : "lob physical reads ",
        //lobreadahead : "lob read-ahead reads ",
        //headerrownum : "Row Num",
        //headertable : "Table",
        //headerscan : "Scan Count",
        //headerlogical : "Logical Reads",
        //headerphysical : "Physical Reads",
        //headerreadahead : "Read-Ahead Reads",
        //headerloblogical : "LOB Logical Reads",
        //headerlobphysical : "LOB Physical Reads",
        //headerlobreadahead : "LOB Read-Ahead Reads",
        //headerperlogicalread : "% Logical Reads of Total Reads",
        //executiontime : "SQL Server Execution Times:",
        //compiletime : "SQL Server parse and compile time:",
        //cputime : "CPU time = ",
        //elapsedtime : "elapsed time = ",
        //elapsedlabel : "Elapsed",
        //cpulabel : "CPU",
        //milliseconds : "ms",
        //rowsaffected : "row(s) affected",
        //headerrowsaffected : " rows affected",
        //headerrowaffected : " row affected",
        //errormsg : "Msg",
        //sampleoutput :  "SQL Server parse and compile time: \r\n   CPU time = 108 ms, elapsed time = 108 ms.\r\n\r\n(13431682 row(s) affected)\r\nTable \u0027PostTypes\u0027. Scan count 1, logical reads 2, physical reads 1, read-ahead reads 0, lob logical reads 0, lob physical reads 0, lob read-ahead reads 0.\r\nTable \u0027Users\u0027. Scan count 5, logical reads 42015, physical reads 1, read-ahead reads 41306, lob logical reads 0, lob physical reads 0, lob read-ahead reads 0.\r\nTable \u0027Comments\u0027. Scan count 5, logical reads 1089402, physical reads 248, read-ahead reads 1108174, lob logical reads 0, lob physical reads 0, lob read-ahead reads 0.\r\nTable \u0027PostTags\u0027. Scan count 5, logical reads 77500, physical reads 348, read-ahead reads 82219, lob logical reads 0, lob physical reads 0, lob read-ahead reads 0.\r\nTable \u0027Posts\u0027. Scan count 5, logical reads 397944, physical reads 9338, read-ahead reads 402977, lob logical reads 0, lob physical reads 0, lob read-ahead reads 0.\r\nTable \u0027Worktable\u0027. Scan count 999172, logical reads 16247024, physical reads 0, read-ahead reads 0, lob logical reads 0, lob physical reads 0, lob read-ahead reads 0.\r\nTable \u0027Worktable\u0027. Scan count 0, logical reads 0, physical reads 0, read-ahead reads 0, lob logical reads 0, lob physical reads 0, lob read-ahead reads 0.\r\n\r\n SQL Server Execution Times:\r\n   CPU time = 156527 ms,  elapsed time = 284906 ms.\r\nSQL Server parse and compile time: \r\n   CPU time = 16 ms, elapsed time = 19 ms.\r\n\r\n(233033 row(s) affected)\r\nTable \u0027Worktable\u0027. Scan count 0, logical reads 0, physical reads 0, read-ahead reads 0, lob logical reads 0, lob physical reads 0, lob read-ahead reads 0.\r\nTable \u0027Votes\u0027. Scan count 1, logical reads 250128, physical reads 10, read-ahead reads 250104, lob logical reads 0, lob physical reads 0, lob read-ahead reads 0.\r\nTable \u0027Posts\u0027. Scan count 1, logical reads 165586, physical reads 18, read-ahead reads 49191, lob logical reads 823463, lob physical reads 42854, lob read-ahead reads 3272.\r\nTable \u0027Users\u0027. Scan count 1, logical reads 41405, physical reads 3, read-ahead reads 41401, lob logical reads 0, lob physical reads 0, lob read-ahead reads 0.\r\n\r\n SQL Server Execution Times:\r\n   CPU time = 17207 ms,  elapsed time = 38163 ms.\r\nMsg 207, Level 16, State 1, Line 1\r\nInvalid column name \u0027scores\u0027.\r\nSQL Server parse and compile time: \r\n   CPU time = 0 ms, elapsed time = 0 ms.\r\n\r\n SQL Server Execution Times:\r\n   CPU time = 0 ms,  elapsed time = 0 ms.\r\n"

    }
}
