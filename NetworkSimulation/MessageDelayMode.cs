

namespace NetworkSimulation
{
    public enum MessageDelayMode
    {
        // Delay from source node (0) to node n-1
        PathEndpoint,

        // Delay from source node (0) to node floor(n/2)
        // (approximates graph diameter for cycle graphs)
        CycleDiameter
    }
}