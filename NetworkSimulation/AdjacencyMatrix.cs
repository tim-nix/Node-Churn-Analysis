using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    /// <summary>
    /// The purpose of this class is to provide functionality for various graph
    /// algorithms on an adjacency matrix representation.
    /// </summary>
    class AdjacencyMatrix
    {
        int[,] graph;


        /// <summary>
        /// The purpose of this 'getter' is to return a copy of the class
        /// member graph.
        /// </summary>
        public int[,] Graph
        {
            get
            {
                return (int[,])this.graph.Clone();
            }
        }


        /// <summary>
        /// A constructor for the AdjacencyMatrix class.  
        /// </summary>
        /// <param name="g">A square matrix representing a graph</param>
        public AdjacencyMatrix(int[,] g)
        {
            if (g.GetLength(0) == g.GetLength(1))
            {
                graph = g;
            }
            else
                throw new ArgumentException("Error: Square matrix is required!");
        }


        /// <summary>
        /// A constructor for the AdjacencyMatrix class.  Creates a square matrix
        /// numVertices x numVertices in size and initializes all values to zero.
        /// </summary>
        /// <param name="numVertices">The number of vertices in the graph.</param>
        public AdjacencyMatrix(int numVertices)
        {
            if (numVertices > 0)
            {
                graph = new int[numVertices, numVertices];

                for (int i = 0; i < numVertices; i++)
                {
                    for (int j = 0; j < numVertices; j++)
                    {
                        graph[i, j] = 0;
                    }
                }
            }
        }


        /// <summary>
        /// The purpose of this method is to determine the shortest cycle 
        /// starting from a given vertex that returns to the same vertex 
        /// without traversing the any edge more than once.  The
        /// approach is to use a breadth-first search with the starting 
        /// vertex as the root of the search tree.
        /// </summary>
        /// <param name="startVertex">The starting vertex in the path.</param>
        /// <returns>The shortest cycle length starting from the given vertex.</returns>
        public int smallestCycle(int startVertex)
        {
            if ((startVertex < 0) || (startVertex > graph.GetLength(0)))
                throw new ArgumentException("Error: Starting vertex does not exist!");
            else
            {
                //Console.WriteLine("Starting BFS.  Starting node is {0}", startVertex);
                int depth = 1;
                Queue<int> toVisitNow = new Queue<int>();
                Queue<int> toVisitNext = new Queue<int>();
                HashSet<int> visited = new HashSet<int>();

                toVisitNow.Enqueue(startVertex);

                while ((toVisitNow.Count > 0) || (toVisitNext.Count > 0))
                {
                    int currentNode = toVisitNow.Dequeue();

                    visited.Add(currentNode);
                    for (int i = 0; i < graph.GetLength(0); i++)
                    {
                        if ((graph[currentNode, i] == 1) && !visited.Contains(i))
                        {
                            if (toVisitNow.Contains(i))
                            {
                                //Console.WriteLine("Cycle of size {0} found.", (depth * 2) - 1);
                                return (depth * 2) - 1;
                            }
                            else if (toVisitNext.Contains(i))
                            {
                                //Console.WriteLine("Cycle of size {0} found.", depth * 2);
                                return depth * 2;
                            }
                            else
                            {
                                //Console.WriteLine("Adding for vertex {0} at depth {1}", i, depth + 1);
                                toVisitNext.Enqueue(i);
                            }
                        }
                    }

                    if (toVisitNow.Count == 0)
                    {
                        //Console.WriteLine("Depth {0} complete!", depth);
                        toVisitNow = toVisitNext;
                        toVisitNext = new Queue<int>();
                        depth++;
                    }
                }
            }

            //Console.WriteLine("No cycle found!");
            return -1;
        }


        /// <summary>
        /// The purpose of this method is to determine the girth of a graph; that
        /// is, the length of a shortest cycle contained in the graph.  If the graph
        /// does not contain any cycles, if girth is mathematically defined as infinity.
        /// However, this method returns the value -1.
        /// </summary>
        /// <returns>The girth of the graph class member.</returns>
        public int girth()
        {
            int g = -1;

            for (int i = 0; i < graph.GetLength(0); i++)
            {
                int currentCycle = smallestCycle(i);

                if (currentCycle > 0)
                {
                    if (g < 0)
                        g = currentCycle;
                    else if (g > currentCycle)
                        g = currentCycle;
                }
            }

            return g;
        }


        /// <summary>
        /// The purpose of this method is to determine if the graph class member
        /// is connected; that is, a path exists between every pair of vertices.
        /// </summary>
        /// <returns>The boolean answer as to whether the graph is connected.</returns>
        public bool isConnected()
        {

            if (numEdges() == 0)
                return false;

            Queue<int> toVisit = new Queue<int>();
            HashSet<int> visited = new HashSet<int>();

            toVisit.Enqueue(0);

            while (toVisit.Count > 0)
            {
                int currentNode = toVisit.Dequeue();

                visited.Add(currentNode);
                for (int i = 0; i < graph.GetLength(0); i++)
                {
                    if ((graph[currentNode, i] == 1) && !visited.Contains(i))
                    {
                        toVisit.Enqueue(i);
                    }
                }
            }

            return (visited.Count == graph.GetLength(0));
        }


        /// <summary>
        /// The purpose of this method is to determine is the class member graph
        /// is regular; that is, if each vertex has the same number of neighbors.
        /// </summary>
        /// <returns>The boolean answer as to whether the graph is regular.</returns>
        public bool isRegular()
        {
            int degree = 0;

            for (int j = 0; j < graph.GetLength(1); j++)
            {
                if (graph[0, j] == 1)
                    degree++;
            }

            //Console.WriteLine("Node 0 had degree {0}.", degree);

            for (int i = 1; i < graph.GetLength(0); i++)
            {
                int thisDegree = 0;
                for (int j = 0; j < graph.GetLength(1); j++)
                {
                    if (graph[i, j] == 1)
                        thisDegree++;
                }

                //Console.WriteLine("Node {0} had degree {1}.", i, thisDegree);
                if (degree != thisDegree)
                    return false;
            }

            return true;
        }


        /// <summary>
        /// The purpose of this method is to return the value of the smallest degree
        /// within the class member graph; that is the fewest number of neighbors had
        /// by any vertex.
        /// </summary>
        /// <returns>The minimum degree of the class member graph.</returns>
        public int minDegree()
        {
            int degree = graph.GetLength(0);

            for (int i = 0; i < graph.GetLength(0); i++)
            {
                int thisDegree = 0;
                for (int j = 0; j < graph.GetLength(1); j++)
                {
                    if (graph[i, j] == 1)
                        thisDegree++;
                }

                if (thisDegree < degree)
                    degree = thisDegree;
            }

            return degree;
        }


        /// <summary>
        /// The purpose of this method is to return the value of the largest degree
        /// within the class member graph; that is the most number of neighbors had
        /// by any vertex.
        /// </summary>
        /// <returns>The maximum degree of the class member graph.</returns>
        public int maxDegree()
        {
            int degree = 0;

            for (int i = 0; i < graph.GetLength(0); i++)
            {
                int thisDegree = 0;
                for (int j = 0; j < graph.GetLength(1); j++)
                {
                    if (graph[i, j] == 1)
                        thisDegree++;
                }

                if (thisDegree > degree)
                    degree = thisDegree;
            }

            return degree;
        }

        public int getDegree(int node)
        {
            if (node >= graph.GetLength(0))
                throw new System.ArgumentOutOfRangeException("Given node does not exist!");

            int degree = 0;

            for (int j = 0; j < graph.GetLength(0); j++)
            {
                if (graph[node, j] == 1)
                    degree++;
            }

            return degree;
        }


        /// <summary>
        /// The purpose of this method is to modify the class member graph by
        /// removing the specified row and column from the adjecency matrix.
        /// </summary>
        /// <param name="rowToRemove">The row of the matrix to remove.</param>
        /// <param name="columnToRemove">The column of the matrix to remove.</param>
        public void TrimArray(int rowToRemove, int columnToRemove)
        {
            int[,] result = new int[graph.GetLength(0) - 1, graph.GetLength(1) - 1];

            for (int i = 0, j = 0; i < graph.GetLength(0); i++)
            {
                if (i == rowToRemove)
                    continue;

                for (int k = 0, u = 0; k < graph.GetLength(1); k++)
                {
                    if (k == columnToRemove)
                        continue;

                    result[j, u] = graph[i, k];
                    u++;
                }
                j++;
            }

            graph = result;
        }


        /// <summary>
        /// The purpose of this method is to return the size of the class member
        /// graph; that is, the number of edges within the graph.  This method assumes
        /// that the graph is undirected and unweighted.  Thus, the existence of an
        /// edge will be denoted by the value 1 and that both graph[i, j] == 1 and 
        /// graph[j, i] == 1.  Thus, the edge count is divided by 2 prior to being
        /// returned.
        /// </summary>
        /// <returns>The number of edges in the class member graph.</returns>
        public int numEdges()
        {
            int e = 0;

            for (int i = 0; i < graph.GetLength(0); i++)
            {
                for (int j = 0; j < graph.GetLength(1); j++)
                {
                    if (graph[i, j] == 1)
                        e++;
                }
            }

            return e / 2;
        }


        /// <summary>
        /// The purpose of this method is to print the values contained in 
        /// class member graph to the console.  It displays the array as a
        /// simple square matrix and does not assume anything about the size
        /// of the array or the console.  It is primarily used for testing 
        /// purposes.
        /// </summary>
        public void displayGraph()
        {
            for (int i = 0; i < graph.GetLength(0); i++)
            {
                for (int j = 0; j < graph.GetLength(1); j++)
                {
                    Console.Write(" {0} ", graph[i, j]);
                }
                Console.WriteLine();
            }
        }
    }
}
