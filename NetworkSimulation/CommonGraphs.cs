using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    public class CommonGraphs
    {
        /// <summary>
        /// Creates an adjacency matrix for an undirected cycle graph with the specified number of nodes.
        /// </summary>
        /// <remarks>The resulting matrix is symmetric and has 1s for each pair of adjacent nodes (i and
        /// i±1 modulo numNodes). Negative values for numNodes will cause a runtime exception when allocating the
        /// array.</remarks>
        /// <param name="numNodes">The number of nodes in the cycle. Must be non-negative; if zero, returns an empty 0-by-0 matrix.</param>
        /// <returns>An int[,] whose element [i,j] is 1 if nodes i and j are adjacent in the cycle (consecutive indices or first
        /// and last), otherwise 0.</returns>
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


        /// <summary>
        /// Creates an adjacency matrix for an undirected path graph with the specified number of nodes.
        /// </summary>
        /// <remarks>The matrix is symmetric; edges exist only between node i and i+1 for 0 <= i <
        /// numNodes - 1.</remarks>
        /// <param name="numNodes">Number of nodes in the path.</param>
        /// <returns>An int[,] adjacency matrix of size numNodes-by-numNodes where 1 indicates an edge between consecutive nodes
        /// and 0 otherwise.</returns>
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

        /// <summary>
        /// Creates an adjacency matrix representing internally vertex-disjoint
        /// linear paths with shared source and destination nodes.
        /// </summary>
        /// <remarks>
        /// The source and destination are shared by every path. Each path
        /// therefore contributes pathLength - 2 internal nodes.
        /// </remarks>
        /// <param name="pathLength">
        /// Number of nodes in each path, including the shared source and
        /// destination nodes.
        /// </param>
        /// <param name="pathCount">
        /// Number of internally disjoint paths; must be at least two.
        /// </param>
        /// <returns>
        /// An adjacency matrix with 2 + pathCount * (pathLength - 2) nodes.
        /// </returns>
        public static int[,] MultiPath(int pathLength, int pathCount)
        {
            if (pathLength < 3)
            {
                throw new ArgumentException(
                    "Path length must be at least 3 nodes.");
            }

            if (pathCount < 2)
            {
                throw new ArgumentException("Path count must be at least 2.");
            }

            /*
             * Node layout:
             *
             * 0 = source
             * 1 = destination
             *
             * Each path contributes (pathLength - 2)
             * internal nodes.
             *
             * Total nodes:
             *
             * 2 + pathCount * (pathLength - 2)
             */

            int totalNodes = 2 + pathCount * (pathLength - 2);

            int[,] graph = new int[totalNodes, totalNodes];

            int nextNode = 2;

            for (int p = 0; p < pathCount; p++)
            {
                int previous = 0;

                /* Add internal nodes for this path. */
                for (int i = 0; i < pathLength - 2; i++)
                {
                    int current = nextNode++;

                    graph[previous, current] = 1;
                    graph[current, previous] = 1;

                    previous = current;
                }

                /* Connect final node in the path to destination node 1. */
                graph[previous, 1] = 1;
                graph[1, previous] = 1;
            }

            return graph;
        }


        /// <summary>
        /// Creates an adjacency matrix for a complete undirected graph (clique) with the specified number of nodes.
        /// </summary>
        /// <remarks>The matrix is symmetric and represents an undirected graph; diagonal entries remain
        /// zero.</remarks>
        /// <param name="numNodes">The number of nodes in the clique. Must be non-negative.</param>
        /// <returns>An int[,] adjacency matrix of size numNodes × numNodes where entries are 1 for each distinct connected node
        /// pair and 0 on the diagonal (no self-loops).</returns>
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


        /// <summary>
        /// Adjacency matrix for the Petersen graph.
        /// </summary>
        /// <remarks>The matrix is 10-by-10 and symmetric; rows and columns correspond to vertices indexed
        /// 0 through 9. Entries are 1 for an edge and 0 for no edge; diagonal entries are 0 (no self-loops).</remarks>
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



        /// <summary>
        /// Creates an undirected random graph in the G(n, p) model and returns its adjacency matrix.
        /// </summary>
        /// <remarks>Randomness is generated with a Mersenne Twister. Each unordered pair (i, j) with i <
        /// j is sampled independently; no self-loops are added.</remarks>
        /// <param name="n">Number of vertices in the graph.</param>
        /// <param name="p">Probability in [0, 1] that each unordered pair of vertices is connected by an edge.</param>
        /// <returns>An n-by-n adjacency matrix where 1 denotes an edge and 0 denotes no edge; the matrix is symmetric with zeros
        /// on the diagonal.</returns>
        public static int[,] Gnp(int n, double p)
        {
            int[,] c = new int[n, n];
            MersenneTwister randomNum = new MersenneTwister();

            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (randomNum.genrand_real1() <= p)
                    {
                        c[i, j] = 1;
                        c[j, i] = 1;
                    }
                }
            }

            return c;
        }


        /// <summary>
        /// Generates an adjacency matrix for the Gunther–Hartnell graph on the specified number of nodes.
        /// </summary>
        /// <remarks>Computes k = ceil((sqrt(1+4*numNodes)-1)/2) to determine clique sizes, partitions
        /// vertices into numCliques cliques of size k or k-1, fully connects vertices within each clique, and adds
        /// designated inter-clique edges according to the Gunther–Hartnell construction.</remarks>
        /// <param name="numNodes">The number of vertices in the graph.</param>
        /// <returns>An adjacency matrix of size numNodes-by-numNodes (int[,]) with 1 indicating an edge and 0 indicating no
        /// edge.</returns>
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


        /// <summary>
        /// The purpose of this method is to generate a Barabasi-Albert
        /// random scale-free network topology.  The construction uses
        /// preferential attachment; that is, as new edges are added to
        /// the topololy, nodes with high degree are more likely to gain
        /// an edge than nodes with low degree.
        /// </summary>
        /// <param name="numNodes">The number of nodes in the topology.</param>
        /// <param name="mo">The number of nodes in the initial clique.</param>
        /// <param name="m">The number of initial edges each additional node.</param>
        /// <returns>The adjacency matrix of the constructed graph.</returns>
        public static int[,] BarabasiAlbert(int numNodes, int mo, int m)
        {
            int[,] c = new int[numNodes, numNodes];
            ArrayList pSet = new ArrayList();
            MersenneTwister randomNum = new MersenneTwister();

            if (numNodes <= mo)
                throw new Exception("Error: Barabasi-Albert requires more than " + m + " nodes.");

            if (mo <= m)
                throw new Exception("Error: Barabasi-Albert requires m < mo.");
                        
            // Build a clique of mo nodes.
            for (int i = 0; i < mo; i++)
            {
                for (int j = 0; j < mo; j++)
                {
                    if (i != j)
                    {
                        c[i, j] = 1;
                        c[j, i] = 1;
                        pSet.Add(i);
                        pSet.Add(j);
                    }
                }
            }

            int index = 0;
            int node2 = 0;
            for (int i = mo; i < numNodes; i++)
            {
                int j = 0;

                while (j < m)
                {
                    index = Convert.ToInt32(randomNum.genrand_int(pSet.Count));
                    node2 = Convert.ToInt32(pSet[index]);
                    if ((0 == c[i, node2]) && (i != node2))
                    {
                        c[i, node2] = 1;
                        c[node2, i] = 1;
                        pSet.Add(i);
                        pSet.Add(node2);
                        j++;
                    }
                }
            }
                        
            return c;
        }
    }
}
