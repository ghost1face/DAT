using DAT.AppCommand;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAT.Results.ResultRenderers
{
    public interface IResultRenderer
    {
        void Render(DATCommand command, DataCompareResults dataComparisonResult, List<PerformanceResult> performanceResults);
    }
}
