using System;

namespace NetworkSimulation
{
    /// <summary>
    /// The purpose of this class is to maintain two versions of the same
    /// network: one version is the full network when all nodes are live;
    /// the second version is based on only a portion of the nodes being
    /// live.
    /// </summary>
    public class Network
    {
        private int[] labels;
        private AdjacencyMatrix fullNetwork;
        private AdjacencyMatrix currentStatus;


        /// <summary>
        /// A constructor for the class, the full network and the current
        /// network are both set as if all nodes were active.
        /// </summary>
        /// <param name="g">A two-dimensional array representing the adjacency matrix of the graph.</param>
        public Network(int[,] g)
        {
            fullNetwork = new AdjacencyMatrix(g);
            currentStatus = new AdjacencyMatrix(g);
            labels = new int[g.GetLength(0)];
        }


        /// <summary>
        /// The purpose of this method is to modify the current network adjacency matrix
        /// based on which nodes are live and which nodes are not.  The nodes that are 
        /// not live are removed from the adjacency matrix, currentStatus.
        /// </summary>
        /// <param name="nodeStatus">A boolean array specifying is each node is live.</param>
        public void updateStatus(bool[] nodeStatus)
        {
            int count = 0;
            for (int i = 0; i < nodeStatus.Length; i++)
            {
                if (nodeStatus[i])
                    count++;
            }

            labels = new int[count];
            int j = 0;
            for (int i = 0; i < nodeStatus.Length; i++)
            {
                if (nodeStatus[i])
                {
                    labels[j] = i;
                    j++;
                }
            }

            currentStatus = new AdjacencyMatrix(fullNetwork.Graph);
            int trimmed = 0;

            for (int i = 0; i < nodeStatus.Length; i++)
            {
                if (!nodeStatus[i])
                {
                    currentStatus.trimArray(i - trimmed, i - trimmed);
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
        /// Returns the degree of node n in the full network.
        /// </summary>
        /// <param name="n">The node to examine</param>
        /// <returns>The degree of node n.</returns>
        public int getFullDegree(int n)
        {
            return fullNetwork.getDegree(n);
        }


        /// <summary>
        /// Returns the degree of node n in the current network.  If the current
        /// node is not live, then -1 is returned. If the current node is live but 
        /// isolated, then the function returns 0.
        /// </summary>
        /// <param name="n">The node to examine.</param>
        /// <returns>The degree of node n.</returns>
        public int getCurrentDegree(int n)
        {
            int index = Array.IndexOf(labels, n);

            if (index == -1)
                return -1;
            else
                return currentStatus.getDegree(index);
        }


        /// <summary>
        /// Returns the degree distribution of the full network.  The
        /// index of the returned array corresponds to the possible
        /// node degree.  Each value in the array corresponds to the 
        /// number of nodes that have that degree.
        /// </summary>
        /// <returns>The degree distribution of the full network.</returns>
        public int[] getDistroFull()
        {
            return fullNetwork.degreeDistro();
        }


        /// <summary>
        /// Returns the degree distribution of the current network.  
        /// The index of the returned array corresponds to the possible
        /// node degree.  Each value in the array corresponds to the 
        /// number of nodes that have that degree.
        /// </summary>
        /// <returns>The degree distribution of the current network.</returns>
        public int[] getDistroCurrent()
        {
            return currentStatus.degreeDistro();
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

            Console.Write("Labels: ");
            for (int i = 0; i < labels.Length; i++)
                Console.Write("{0} ", labels[i]);
            Console.WriteLine();
        }
    }
}
