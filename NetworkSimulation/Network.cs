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

        public bool isCurrentNetworkConnected()
        {
            return currentStatus.isConnected();
        }

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
