using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    /// <summary>
    /// The purpose of this class is to maintain two versions of the same
    /// network: one version is the full network when all nodes are live;
    /// the second version is based on only a portion of the nodes being
    /// live.
    /// </summary>
    class Network
    {
        AdjacencyMatrix fullNetwork;
        AdjacencyMatrix currentStatus;


        /// <summary>
        /// A constructor for the class, the full network and the current
        /// network are both set as if all nodes were active.
        /// </summary>
        /// <param name="g">A two-dimensional array representing the adjacency matrix of the graph.</param>
        public Network(int[,] g)
        {
            fullNetwork = new AdjacencyMatrix(g);
            currentStatus = new AdjacencyMatrix(g);
        }


        /// <summary>
        /// The purpose of this method is to modify the current network adjacency matrix
        /// based on which nodes are live and which nodes are not.  The nodes that are 
        /// not live are removed from the adjacency matrix, currentStatus.
        /// </summary>
        /// <param name="nodeStatus">A boolean array specifying is each node is live.</param>
        public void updateStatus(bool[] nodeStatus)
        {
            currentStatus = new AdjacencyMatrix(fullNetwork.Graph);
            int trimmed = 0;

            for (int i = 0; i < nodeStatus.Length; i++)
            {
                if (!nodeStatus[i])
                {
                    currentStatus.TrimArray(i - trimmed, i - trimmed);
                    trimmed++;
                }
            }
        }


        /// <summary>
        /// The purpose of this method is to serve as a wrapper for the isConnected()
        /// method in the AdjacencyMatrix class called on the currentStatus class member. 
        /// </summary>
        /// <returns>The boolean value as to whether the currentStatus network is connected or not.</returns>
        public bool isCurrentNetworkConnected()
        {
            return currentStatus.isConnected();
        }


        /// <summary>
        /// The purpose of this method is to extend the displayGraph() method in the
        /// AdjacencyMatrix class by displaying both the fullNetwork and the currentStatus.
        /// This method is primarily for testing.
        /// </summary>
        public void displayNetwork()
        {
            Console.WriteLine("The full network is:");
            fullNetwork.displayGraph();
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("The current network state is:");
            currentStatus.displayGraph();
            Console.WriteLine(Environment.NewLine);
        }
    }
}
