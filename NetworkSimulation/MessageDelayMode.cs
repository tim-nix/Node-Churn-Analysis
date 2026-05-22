

namespace NetworkSimulation
{
    public enum MessageDelayMode
    {
        // Delay from source node (0) to node n-1
        PathEndpoint,

        // Delay from source node (0) to node floor(n/2)
        // (approximates graph diameter for cycle graphs)
        CycleDiameter,

   
        // Message delay is measured between the shared source
        // node (0) and shared destination node (1) in a
        // topology containing multiple internally disjoint
        // equal-length paths.
        MultiPathEndpoint
    }
}