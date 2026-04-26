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
                    ZeroDelayPercent = 0.0,
                    AllNodesLivePercent = 0.0,
                    AverageMessageDelay = 0.0,
                    AverageLivePercent = 0.0,
                    TrialCount = 0
                };
            }

            int zeroDelayCount = successfulResults.Count(result => result.ZeroDelay);
            int allNodesLiveCount = successfulResults.Count(result => result.AllNodesLive);

            return new ResultSummary
            {
                ZeroDelayPercent =
                    (zeroDelayCount / Convert.ToDouble(successfulResults.Count)) * 100.0,

                AllNodesLivePercent =
                    (allNodesLiveCount / Convert.ToDouble(successfulResults.Count)) * 100.0,

                RedundancyGainPercent =
                    ((zeroDelayCount - allNodesLiveCount) / Convert.ToDouble(successfulResults.Count)) * 100.0,

                AverageMessageDelay =
                    successfulResults.Average(result => result.Delay),

                AverageLivePercent =
                    successfulResults.Average(result => result.PercentLive),

                TrialCount = successfulResults.Count
            };
        }
    }
}