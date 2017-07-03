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
        private static Random r = new Random();


        /// <summary>
        /// The purpose of this 'getter' is to return a copy of the class
        /// member fullNetwork.
        /// </summary>
        public int[,] FullGraph
        {
            get
            {
                return fullNetwork.Graph;
            }
        }


        /// <summary>
        /// The purpose of this 'getter' is to return a copy of the class
        /// member creentStatus.
        /// </summary>
        public int[,] CurrentGraph
        {
            get
            {
                return currentStatus.Graph;
            }
        }

        /// <summary>
        /// A constructor for the class, the full network and the current
        /// network are both set as if all nodes were active.
        /// </summary>
        /// <param name="g">The adjacency matrix of the graph.</param>
        public Network(int[,] g)
        {
            fullNetwork = new AdjacencyMatrix(g);
            currentStatus = new AdjacencyMatrix(g);
            labels = new int[g.GetLength(0)];
        }


        /// <summary>
        /// The purpose of this method is to modify the current network 
        /// adjacency matrix based on which nodes are live and which nodes 
        /// are not.  The currentStatus graph is reset to the full graph 
        /// and then nodes that are not live are trimmed from currentStatus.
        /// </summary>
        /// <param name="nodeStatus">An array specifying the status of each node.</param>
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
        /// The purpose of this method is to serve as a wrapper for the 
        /// isConnected() method in the AdjacencyMatrix class called on 
        /// the currentStatus. 
        /// </summary>
        /// <returns>Whether the currentStatus network is connected.</returns>
        public bool isCurrentNetworkConnected()
        {
            return currentStatus.isConnected();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool isPathinCurrentNetwork()
        {
            //Console.WriteLine("Current graph has " + currentStatus.Order + " nodes.");
            if (currentStatus.Order < 2)
                throw new Exception("Not enough nodes!");
            
            int startVertex = r.Next(0, currentStatus.Order);
            int endVertex = startVertex;

            while (startVertex == endVertex)
                endVertex = r.Next(0, currentStatus.Order);

            //Console.WriteLine("Finding path from " + startVertex + " to " + endVertex);

            return currentStatus.isPath(startVertex, endVertex);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkVertex"></param>
        /// <returns></returns>
        public int getNewNodeLabel(int checkVertex)
        {
            for (int i = 0; i < labels.Length; i++)
                if (labels[i] == checkVertex)
                    return i;

            return -1;
        }


        /// <summary>
        /// The purpose of this method is to serve as a wrapper for the 
        /// getDegree() method in the AdjacencyMatrix class called on 
        /// the fullNetwork.
        /// </summary>
        /// <param name="n">The node to examine</param>
        /// <returns>The degree of node n.</returns>
        public int getFullDegree(int n)
        {
            return fullNetwork.getDegree(n);
        }


        /// <summary>
        /// The purpose of this method is to serve as a wrapper for the 
        /// getDegree() method in the AdjacencyMatrix class called on 
        /// the currentStatus.  If the current node is not live, then 
        /// -1 is returned. If the current node is live but isolated, 
        /// then the function returns 0.
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
        /// The purpose of this method is to serve as a wrapper for the 
        /// degreeDistro() method in the AdjacencyMatrix class called on 
        /// the fullNetwork. The index of the returned array corresponds 
        /// to the possible node degree. Each value in the array 
        /// corresponds to the number of nodes that have that degree.
        /// </summary>
        /// <returns>The degree distribution of the full network.</returns>
        public int[] getDistroFull()
        {
            return fullNetwork.degreeDistro();
        }


        /// <summary>
        /// The purpose of this method is to serve as a wrapper for the 
        /// degreeDistro() method in the AdjacencyMatrix class called on 
        /// the currentStatus. The index of the returned array corresponds 
        /// to the possible node degree.  Each value in the array 
        /// corresponds to the number of nodes that have that degree.
        /// </summary>
        /// <returns>The degree distribution of the current network.</returns>
        public int[] getDistroCurrent()
        {
            return currentStatus.degreeDistro();
        }


        /// <summary>
        /// The purpose of this method is to extend the ToString() method in 
        /// the AdjacencyMatrix class by converting both the fullNetwork, 
        /// the currentStatus, and the labels to a string representation.
        /// </summary>
        public override String ToString()
        {
            String s = "The full network is:\n";
            fullNetwork.ToString();
            s += "\nThe current network state is:\n";
            currentStatus.ToString();

            s += "\nLabels: ";
            for (int i = 0; i < labels.Length; i++)
            {
                s += labels[i];
                s += " ";
            }
            s += "\n";

            return s;
        }
    }
}
