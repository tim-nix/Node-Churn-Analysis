using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    class CommonGraphs
    {
        public static int[,] PetersonGraph = new int[,] {{0, 1, 0, 0, 1, 1, 0, 0, 0, 0},
                                                         {1, 0, 1, 0, 0, 0, 1, 0, 0, 0},
                                                         {0, 1, 0, 1, 0, 0, 0, 1, 0, 0 },
                                                         {0, 0, 1, 0, 1, 0, 0, 0, 1, 0 },
                                                         {1, 0, 0, 1, 0, 0, 0, 0, 0, 1 },
                                                         {1, 0, 0, 0, 0, 0, 0, 1, 1, 0 },
                                                         {0, 1, 0, 0, 0, 0, 0, 0, 1, 1 },
                                                         {0, 0, 1, 0, 0, 1, 0, 0, 0, 1 },
                                                         {0, 0, 0, 1, 0, 1, 1, 0, 0, 0 },
                                                         {0, 0, 0, 0, 1, 0, 1, 1, 0, 0 } };

        public static int[,] Cycle(int numNodes)
        {
            int[,] c = new int[numNodes, numNodes];

            for (int i = 0; i < numNodes; i++)
            {
                for (int j = 0; j < numNodes; j++)
                {
                    if (((i - 1) == j) || ((i + 1) == j))
                        c[i, j] = 1;
                    else if ((i == 0) && (j == (numNodes - 1)))
                        c[i, j] = 1;
                    else if ((i == (numNodes - 1)) && (j == 0))
                        c[i, j] = 1;
                    else
                        c[i, j] = 0;

                }
            }

            return c;
        }
    }
}
