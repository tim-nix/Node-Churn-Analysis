using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    static class TestClass
    {
        public static void testConnectivity()
        {
            AdjacencyMatrix g1 = new AdjacencyMatrix(CommonGraphs.Path(100));

            if (g1.isConnected())
                Console.WriteLine("Test 1 passed");

            Network n1 = new Network(CommonGraphs.Path(100));

            if (n1.isCurrentNetworkConnected())
                Console.WriteLine("Test 2 passed");

            int[,] am2 = new int[,] {{0, 1, 1, 1, 1, 0, 0, 0, 0, 0},
                                    {1, 0, 1, 0, 1, 0, 0, 0, 0, 0},
                                    {1, 1, 0, 1, 0, 0, 0, 0, 0, 0 },
                                    {1, 0, 1, 0, 1, 0, 0, 0, 0, 0 },
                                    {1, 1, 0, 1, 0, 0, 0, 0, 0, 0 },
                                    {0, 0, 0, 0, 0, 0, 0, 1, 0, 1 },
                                    {0, 0, 0, 0, 0, 0, 0, 1, 1, 0 },
                                    {0, 0, 0, 0, 0, 1, 1, 0, 0, 1 },
                                    {0, 0, 0, 0, 0, 1, 1, 0, 0, 1 },
                                    {0, 0, 0, 0, 0, 0, 0, 1, 1, 0 } };

            AdjacencyMatrix g2 = new AdjacencyMatrix(am2);

            if (!g2.isConnected())
                Console.WriteLine("Test 3 passed");

            Network n2 = new Network(am2);

            if (!n2.isCurrentNetworkConnected())
                Console.WriteLine("Test 4 passed");
        }
    }
}
