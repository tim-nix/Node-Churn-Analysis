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


        public static int[,] Path(int numNodes)
        {
            int[,] c = new int[numNodes, numNodes];

            for (int i = 0; i < numNodes; i++)
            {
                for (int j = i; j < numNodes; j++)
                {
                    if (i == (j - 1))
                    {
                        c[i, j] = 1;
                        c[j, i] = 1;
                    }
                }
            }

            return c;
        }


        public static int[,] Clique(int numNodes)
        {
            int[,] c = new int[numNodes, numNodes];

            for (int i = 0; i < numNodes; i++)
            {
                for (int j = i; j < numNodes; j++)
                {
                    if (i != j)
                    {
                        c[i, j] = 1;
                        c[j, i] = 1;
                    }
                }
            }

            return c;
        }

        public static int[,] GuntherHartnell(int numNodes)
        {
            int[,] c = new int[numNodes, numNodes];
            int k = Convert.ToInt32(Math.Ceiling((Math.Sqrt(1.0 + (4.0 * Convert.ToDouble(numNodes))) - 1.0) / 2.0));
            int numCliques = 0;
            int numFull = 0;

            if (numNodes > (k * k))
            {
                numCliques = k + 1;
                numFull = k;
            }

            if (numNodes == (k * (k + 1)))
            {
                numCliques = k + 1;
                numFull = k + 1;
            }

            if (numNodes <= (k * k))
            {
                numCliques = k;
                numFull = numNodes - (k * (k - 1));
            }

            int b = 0;
            int numAdd = 0;
            int x = 0;
            int y = 0;
            for (int i = 0; i < numCliques; i++)
            {
                if (i < numFull)
                    numAdd = k;
                else
                    numAdd = k - 1;

                for (int s = 2; s <= numAdd; s++)
                {
                    for (int t = 1; t < s; t++)
                    {
                        x = b + s;
                        y = b + t;

                        if ((x <= numNodes) && (y <= numNodes))
                        {
                            c[x - 1, y - 1] = 1;
                            c[y - 1, x - 1] = 1;
                        }
                    }
                }

                b += numAdd;
            }

            for (int i = 0; i < numCliques; i++)
            {
                for (int j = i + 1; j < numCliques; j++)
                {
                    if (i >= numFull)
                        x = (numFull * k) + ((i - numFull) * (k - 1)) + j;
                    else
                        x = (i * k) + j;

                    if (j >= numFull)
                        y = (numFull * k) + ((j - numFull) * (k - 1)) + (i + 1);
                    else
                        y = (j * k) + (i + 1);

                    if ((x <= numNodes) && (y <= numNodes))
                    {
                        c[x - 1, y - 1] = 1;
                        c[y - 1, x - 1] = 1;
                    }
                }
            }

            return c;
        }
    }
}
