using System;

namespace NetworkSimulation
{
    /// <summary>
    /// Derives stable per-trial random seeds from an experiment seed.
    /// </summary>
    public static class RandomSeed
    {
        public static uint Derive(
            int experimentSeed,
            int configurationA,
            int configurationB,
            int trialIndex,
            int attempt,
            int stream)
        {
            unchecked
            {
                uint hash = 2166136261;
                hash = Mix(hash, experimentSeed);
                hash = Mix(hash, configurationA);
                hash = Mix(hash, configurationB);
                hash = Mix(hash, trialIndex);
                hash = Mix(hash, attempt);
                hash = Mix(hash, stream);
                return hash;
            }
        }

        private static uint Mix(uint hash, int value)
        {
            unchecked
            {
                hash ^= (uint)value;
                return hash * 16777619;
            }
        }
    }
}
