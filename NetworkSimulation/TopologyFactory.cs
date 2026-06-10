using System;

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

        /// <summary>
        /// Returns the cycle order whose diameter route has the same number
        /// of nodes as the specified path.
        /// </summary>
        /// <remarks>
        /// A path containing L nodes has L - 1 edges. An even cycle therefore
        /// requires 2(L - 1) nodes so either route to its diameter node has
        /// L nodes including both endpoints.
        /// </remarks>
        /// <param name="pathLength">
        /// Number of nodes in the path, including source and destination.
        /// </param>
        public static int GetEquivalentCycleOrder(int pathLength)
        {
            if (pathLength < 2)
            {
                throw new ArgumentException(
                    "Path length must be at least 2 nodes.");
            }

            return 2 * (pathLength - 1);
        }

        /// <summary>
        /// Creates equal-length internally disjoint paths between shared
        /// source and destination nodes.
        /// </summary>
        /// <param name="pathLength">
        /// Number of nodes in each path, including the shared endpoints.
        /// </param>
        /// <param name="pathCount">
        /// Number of paths between the endpoints; must be at least two.
        /// </param>
        public static Network CreateMultiPath(int pathLength, int pathCount)
        {
            return new Network(CommonGraphs.MultiPath(pathLength, pathCount));
        }
    }
}
