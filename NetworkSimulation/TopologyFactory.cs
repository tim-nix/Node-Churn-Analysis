namespace NetworkSimulation
{
    /// <summary>
    /// Factory class for constructing Network instances from supported
    /// topology families.
    /// </summary>
    public static class TopologyFactory
    {
        /// <summary>
        /// Creates a path topology with the specified number of nodes.
        /// </summary>
        /// <param name="numNodes">Number of nodes in the path graph.</param>
        /// <returns>
        /// A Network whose full topology is the path graph P_numNodes.
        /// </returns>
        public static Network CreatePath(int numNodes)
        {
            return new Network(CommonGraphs.Path(numNodes));
        }

        /// <summary>
        /// Creates a cycle topology with the specified number of nodes.
        /// </summary>
        /// <param name="numNodes">Number of nodes in the cycle graph.</param>
        /// <returns>
        /// A Network whose full topology is the cycle graph C_numNodes.
        /// </returns>
        public static Network CreateCycle(int numNodes)
        {
            return new Network(CommonGraphs.Cycle(numNodes));
        }

        public static Network CreateMultiPath(int pathLength, int pathCount)
        {
            return new Network(CommonGraphs.MultiPath(pathLength, pathCount));
        }
    }
}
