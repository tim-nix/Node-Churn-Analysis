using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    class Network
    {
        AdjacencyMatrix fullNetwork;
        AdjacencyMatrix currentStatus;

        public Network(int[,] g)
        {
            fullNetwork = new AdjacencyMatrix(g);
            currentStatus = new AdjacencyMatrix(g);
        }


        public void updateStatus(int[] nodeStatus)
        {
            int[,] g = fullNetwork.Graph;

            int numRows = g.GetLength(0);

            for (int i = 0; i < nodeStatus.Length; i++)
            {
                if (nodeStatus[i] == 0)
                {
                    for (int j = 0; j < numRows; j++)
                    {
                        g[i, j] = 0;
                        g[j, i] = 0;
                    }
                }
            }
        }


        public void displayNetwork()
        {
            Console.WriteLine("The full network is:");
            fullNetwork.displayGraph();
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("The current network state is:");
            currentStatus.displayGraph();
        }
    }
}
