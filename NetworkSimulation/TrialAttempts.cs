using System;

namespace NetworkSimulation
{
    /// <summary>
    /// Executes a trial with a bounded number of retryable attempts.
    /// </summary>
    public static class TrialAttempts
    {
        public static TrialResult Run(
            Func<int, TrialResult> runAttempt,
            int maxAttempts)
        {
            if (runAttempt == null)
                throw new ArgumentNullException("runAttempt");
            if (maxAttempts <= 0)
                throw new ArgumentOutOfRangeException("maxAttempts");

            TrialResult lastResult = null;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                lastResult = runAttempt(attempt);

                if (lastResult == null)
                    throw new InvalidOperationException(
                        "A trial attempt returned no result.");

                if (lastResult.Success)
                    return lastResult;
            }

            string reason = lastResult == null
                ? "unknown failure"
                : lastResult.FailureReason;

            throw new InvalidOperationException(
                string.Format(
                    "Simulation trial failed after {0} attempts. Last failure: {1}",
                    maxAttempts,
                    string.IsNullOrWhiteSpace(reason) ? "unspecified" : reason));
        }
    }
}
