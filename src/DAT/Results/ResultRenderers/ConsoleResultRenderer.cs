using System;
using System.Collections.Generic;
using System.Text;
using DAT.AppCommand;
using ConsoleTableExt;
using System.Linq;

namespace DAT.Results.ResultRenderers
{
    public class ConsoleResultRenderer : IResultRenderer
    {
        public void Render(DATCommand command, DataCompareResults dataComparisonResult, List<PerformanceResult> performanceResults)
        {
            // data comparison output not supported


            // performance output
            var totalResults = new List<object[]>();
            var avgResults = new List<object[]>();
            var percentileResults = new List<object[]>();

            foreach (var performanceResult in performanceResults)
            {
                totalResults.Add(
                    new object[]
                    {
                        performanceResult.TestIdentifier,
                        "n/a",
                        performanceResult.Totals.CPUCompileTime,
                        performanceResult.Totals.ElapsedCompileTime,
                        performanceResult.Totals.CPUExecutionTime,
                        performanceResult.Totals.ElapsedExecutionTime,
                        performanceResult.Totals.Scan,
                        performanceResult.Totals.Logical,
                        performanceResult.Totals.Physical,
                        performanceResult.Totals.ReadAhead,
                        performanceResult.Totals.LobLogical,
                        performanceResult.Totals.LobPhysical,
                        performanceResult.Totals.LobReadAhead,
                        performanceResult.Totals.PercentRead
                    });

                avgResults.Add(
                    new object[]
                    {
                        performanceResult.TestIdentifier,
                        "n/a",
                        performanceResult.Totals.CPUCompileTime,
                        performanceResult.Totals.ElapsedCompileTime,
                        performanceResult.Totals.CPUExecutionTime,
                        performanceResult.Totals.ElapsedExecutionTime,
                        performanceResult.Totals.Scan,
                        performanceResult.Totals.Logical,
                        performanceResult.Totals.Physical,
                        performanceResult.Totals.ReadAhead,
                        performanceResult.Totals.LobLogical,
                        performanceResult.Totals.LobPhysical,
                        performanceResult.Totals.LobReadAhead,
                        performanceResult.Totals.PercentRead
                    });

                foreach (var percentileKVP in performanceResult.Percentiles)
                {
                    percentileResults.Add(
                        new object[]
                        {
                            performanceResult.TestIdentifier,
                            percentileKVP.Key / 100D,
                            performanceResult.Totals.CPUCompileTime,
                            performanceResult.Totals.ElapsedCompileTime,
                            performanceResult.Totals.CPUExecutionTime,
                            performanceResult.Totals.ElapsedExecutionTime,
                            performanceResult.Totals.Scan,
                            performanceResult.Totals.Logical,
                            performanceResult.Totals.Physical,
                            performanceResult.Totals.ReadAhead,
                            performanceResult.Totals.LobLogical,
                            performanceResult.Totals.LobPhysical,
                            performanceResult.Totals.LobReadAhead,
                            performanceResult.Totals.PercentRead
                        });
                }
            }

            Console.WriteLine();
            Console.WriteLine("Totals");

            ConsoleTableBuilder
                .From(totalResults)
                .WithOptions(new ConsoleTableBuilderOption
                {
                    MetaRowPosition = MetaRowPosition.Bottom,
                    MetaRowParams = new object[]
                    {
                            "test value 1",
                            2,
                            AppConstants.MetaRow.COLUMN_COUNT,
                            AppConstants.MetaRow.ROW_COUNT
                    },
                    TrimColumn = true
                })
                .WithFormat(ConsoleTableBuilderFormat.Minimal)
                .WithColumn(new List<string> {
                        "TestRunIdentifier", "Percentile", "CPUCompileTime", "ElapsedCompileTime", "CPUExecutionTime", "ElapsedExecutionTime", "Scan", "Logical", "Physical",
                        "ReadAhead", "LobLogical", "LobPhysical", "LobReadAhead", "PercentRead"
                })
                .ExportAndWriteLine();

            Console.WriteLine();
            Console.WriteLine("Averages");

            ConsoleTableBuilder
                .From(avgResults)
                .WithOptions(new ConsoleTableBuilderOption
                {
                    MetaRowPosition = MetaRowPosition.Bottom,
                    MetaRowParams = new object[]
                    {
                            "test value 1",
                            2,
                            AppConstants.MetaRow.COLUMN_COUNT,
                            AppConstants.MetaRow.ROW_COUNT
                    },
                    TrimColumn = true
                })
                .WithFormat(ConsoleTableBuilderFormat.Minimal)
                .WithColumn(new List<string> {
                        "TestRunIdentifier", "Percentile", "CPUCompileTime", "ElapsedCompileTime", "CPUExecutionTime", "ElapsedExecutionTime", "Scan", "Logical", "Physical",
                        "ReadAhead", "LobLogical", "LobPhysical", "LobReadAhead", "PercentRead"
                })
                .ExportAndWriteLine();

            Console.WriteLine();
            Console.WriteLine("Percentiles");
            
            ConsoleTableBuilder
                .From(percentileResults.OrderBy(i => (double)i[1]).ToList())
                .WithOptions(new ConsoleTableBuilderOption
                {
                    MetaRowPosition = MetaRowPosition.Bottom,
                    MetaRowParams = new object[]
                    {
                            "test value 1",
                            2,
                            AppConstants.MetaRow.COLUMN_COUNT,
                            AppConstants.MetaRow.ROW_COUNT
                    },
                    TrimColumn = true
                })
                .WithFormat(ConsoleTableBuilderFormat.Minimal)
                .WithColumn(new List<string> {
                        "TestRunIdentifier", "Percentile", "CPUCompileTime", "ElapsedCompileTime", "CPUExecutionTime", "ElapsedExecutionTime", "Scan", "Logical", "Physical",
                        "ReadAhead", "LobLogical", "LobPhysical", "LobReadAhead", "PercentRead"
                })
                .ExportAndWriteLine();
        }
    }
}
