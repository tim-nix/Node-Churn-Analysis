namespace NetworkSimulation
{
    /// <summary>
    /// Supplies deterministic random values to simulation distributions.
    /// </summary>
    public interface IRandomSource
    {
        double NextClosedUnit();

        double NextOpenUnit();
    }
}
