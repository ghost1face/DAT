using DAT.AppCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAT.Results.DataCompare
{
    public static class DATCommandResultExtensions
    {
        public static DataCompareResults CompareTestResults(this List<IEnumerable<DATCommandResult>[]> testResults)
        {
            // do some stuff
            var returnResult = new DataCompareResults()
            {
                DataSets = new List<DataSetCompareResult>()
            };
            int testIteration = 0;

            foreach (var testResult in testResults)
            {
                returnResult = testResult.CompareTestResults(returnResult, testIteration, testResults.Count);
                testIteration++;
            }

            return returnResult;
        }

        public static DataCompareResults CompareTestResults(this IEnumerable<DATCommandResult>[] testResults, DataCompareResults existingResults, int testIteration, int testCount)
        {
            int dataSetCounter = 0;

            foreach (var dataSet in testResults.FirstOrDefault().FirstOrDefault().ResultSets)
            {
                int dataRowCounter = 0;

                if (existingResults.DataSets.Count <= dataSetCounter)
                {
                    existingResults.DataSets.Add(new DataSetCompareResult() { DataRows = new List<DataRowCompareResult>() });
                }

                foreach (var dataRow in dataSet)
                {
                    DataRowCompareResult dataRowResult = null;

                    if (existingResults.DataSets[dataSetCounter].DataRows.Count <= dataRowCounter)
                    {
                        existingResults.DataSets[dataSetCounter].DataRows.Add(new DataRowCompareResult()
                        {
                            DataPoints = new List<DataPointCompareResult>()
                        });
                    }

                    dataRowResult = existingResults.DataSets[dataSetCounter].DataRows[dataRowCounter];

                    // now the fields/columns
                    foreach (var columnKvp in (IDictionary<string, object>)dataRow)
                    {
                        var existingColumn = dataRowResult.DataPoints.FirstOrDefault(i => i.Name == columnKvp.Key);

                        if (existingColumn == null)
                        {
                            dataRowResult.DataPoints.Add(new DataPointCompareResult
                            {
                                Name = columnKvp.Key,
                                TestValues = new object[testCount]
                            });

                            existingColumn = dataRowResult.DataPoints.FirstOrDefault(i => i.Name == columnKvp.Key);
                        }

                        // add the value of this dude
                        existingColumn.TestValues[testIteration] = columnKvp.Value;
                        existingColumn.TestValueCount++;
                        existingColumn.ValueEquality = existingColumn.TestValueCount == testCount && existingColumn.TestValues.All(i => string.Equals(existingColumn.TestValues.First().ToString(), i.ToString(), StringComparison.InvariantCultureIgnoreCase));
                    }
                    
                    dataRowCounter++;
                }

                dataSetCounter++;
            }

            return existingResults;
        }
    }
}
