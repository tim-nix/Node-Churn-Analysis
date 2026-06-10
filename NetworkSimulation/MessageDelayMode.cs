

namespace NetworkSimulation
{
    public enum MessageDelayMode
    {
        // Delay from source node 0 to node n - 1 of a path graph.
        PathEndpoint,

        // Delay from source node 0 to node floor(n / 2), a diameter node
        // of the cycle graph. Configured comparison cycles are even.
        CycleDiameter,

        // Delay between shared source node 0 and destination node 1 in a
        // topology containing equal-length internally disjoint paths.
        MultiPathEndpoint
    }
}
