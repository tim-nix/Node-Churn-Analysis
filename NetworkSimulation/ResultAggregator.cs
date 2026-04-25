using System;
using System.Collections.Generic;
using System.Linq;

namespace NetworkSimulation
{
    public static class ResultAggregator
    {
        public static ResultSummary Summarize(List<TrialResult> results)
        {
            List<TrialResult> successfulResults = results
                .Where(result => result.Success)
                .ToList();

            if (successfulResults.Count == 0)
            {
                return new ResultSummary
                {
                    ConnectivityPercent = 0.0,
                    AverageMessageDelay = 0.0,
                    AverageLivePercent = 0.0,
                    TrialCount = 0
                };
            }

            int connectedCount = successfulResults.Count(result => result.Connected);

            return new ResultSummary
            {
                ConnectivityPercent =
                    (connectedCount / Convert.ToDouble(successfulResults.Count)) * 100.0,

                AverageMessageDelay =
                    successfulResults.Average(result => result.Delay),

                AverageLivePercent =
                    successfulResults.Average(result => result.PercentLive),

                TrialCount = successfulResults.Count
            };
        }
    }
}