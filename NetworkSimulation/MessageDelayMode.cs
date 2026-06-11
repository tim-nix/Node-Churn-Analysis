

namespace NetworkSimulation
{
    public enum MessageDelayMode
    {
        // Flooding delay from source node 0 to node n - 1 of a path graph.
        FloodingPathEndpoint,

        // Single-copy sequential store-and-forward delay used only for
        // closed-form path-model validation.
        SequentialPathEndpoint,

        // Delay from source node 0 to node floor(n / 2), a diameter node
        // of the cycle graph. Configured comparison cycles are even.
        CycleDiameter,

        // Delay between shared source node 0 and destination node 1 in a
        // topology containing equal-length internally disjoint paths.
        MultiPathEndpoint
    }
}
