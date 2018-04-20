using System;
using System.Collections.Generic;

namespace NetworkSimulation
{
    /// <summary>
    /// The purpose of this class is to provide functionality for various graph
    /// algorithms on an adjacency matrix representation.
    /// </summary>
    public class AdjacencyMatrix
    {
        private int[,] graph;


        /// <summary>
        /// The purpose of this 'getter' is to return a copy of the class
        /// member graph.
        /// </summary>
        public int[,] Graph
        {
            get
            {
                return (int[,])graph.Clone();
            }
        }

        public int Order
        {
            get
            {
                return graph.GetLength(0);
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
        /// <param name="numVertices">The number of vertices.</param>
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
            else
                throw new ArgumentException("Error: Positive number of vertices are required!");
        }


        /// <summary>
        /// The purpose of this method is to add an edge between two
        /// existing vertices.  This method does not confirm/deny 
        /// whether there is already an existing edge between the
        /// two vertices.  Given that multiedges are not allowed, if
        /// the edge already exists then no change is made.  If the
        /// edge does not exist, then it is added to the adjacency
        /// matrix.
        /// </summary>
        /// <param name="vertex1">one end of the edge</param>
        /// <param name="vertex2">the other end of the edge</param>
        public void addEdge(int vertex1, int vertex2)
        {
            if ((vertex1 < 0) || (vertex1 > graph.GetLength(0)))
                throw new ArgumentException("Error: Vertex 1 does not exist!");

            if ((vertex2 < 0) || (vertex2 > graph.GetLength(0)))
                throw new ArgumentException("Error: Vertex 2 does not exist!");

            graph[vertex1, vertex2] = 1;
            graph[vertex2, vertex1] = 1;
        }


        /// <summary>
        /// The purpose of this method is to remove an edge between
        /// two existing vertices.  This method does not confirm/deny 
        /// whether there is already an existing edge between the
        /// two vertices.  If the edge already does not exist, then 
        /// no change is made.  If the edge exists, then it is removed
        /// from the adjacency matrix.
        /// </summary>
        /// <param name="vertex1">one end of the edge</param>
        /// <param name="vertex2">the other end of the edge</param>
        public void removeEdge(int vertex1, int vertex2)
        {
            if ((vertex1 < 0) || (vertex1 > graph.GetLength(0)))
                throw new ArgumentException("Error: Vertex 1 does not exist!");

            if ((vertex2 < 0) || (vertex2 > graph.GetLength(0)))
                throw new ArgumentException("Error: Vertex 2 does not exist!");

            graph[vertex1, vertex2] = 0;
            graph[vertex2, vertex1] = 0;
        }

        /// <summary>
        /// The purpose of this method is to determine the shortest cycle 
        /// starting from a given vertex that returns to the same vertex 
        /// without traversing the any edge more than once.  The
        /// approach is to use a breadth-first search with the starting 
        /// vertex as the root of the search tree.
        /// </summary>
        /// <param name="startVertex">The starting vertex in the path.</param>
        /// <returns>The shortest cycle length from the given vertex.</returns>
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
        /// The purpose of this method is to determine the shortest path
        /// length starting from a given vertex and traversing to the
        /// destination vertex.  The approach is to use a breadth-first  
        /// search with the starting vertex as the root of the search tree.  
        /// The method returns -1 if no path exists between the two vertices.
        /// </summary>
        /// <param name="startVertex">the starting vertex</param>
        /// <param name="endVertex">the destination vertex</param>
        /// <returns>The path length between the starting vertex and destination vertex.</returns>
        public int getDistance(int startVertex, int endVertex)
        {
            if ((startVertex < 0) || (startVertex > graph.GetLength(0)))
                throw new ArgumentException("Error: Starting vertex does not exist!");

            if ((endVertex < 0) || (endVertex > graph.GetLength(0)))
                throw new ArgumentException("Error: Ending vertex does not exist!");

            //Console.WriteLine("Starting BFS.  Starting node is {0}", startVertex);
            int depth = 0;
            Queue<int> toVisitNow = new Queue<int>();
            Queue<int> toVisitNext = new Queue<int>();
            HashSet<int> visited = new HashSet<int>();

            toVisitNow.Enqueue(startVertex);

            while ((toVisitNow.Count > 0) || (toVisitNext.Count > 0))
            {
                int currentNode = toVisitNow.Dequeue();
                if (currentNode == endVertex)
                    return depth;

                visited.Add(currentNode);
                for (int i = 0; i < graph.GetLength(0); i++)
                {
                    if ((graph[currentNode, i] == 1) && !visited.Contains(i))
                    {
                        toVisitNext.Enqueue(i);
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

            //Console.WriteLine("No cycle found!");
            return -1;
        }


        /// <summary>
        /// The purpose of this method is to determine the shortest path
        /// from the startVertex to the endVertex.
        /// </summary>
        /// <param name="startVertex">the starting vertex</param>
        /// <param name="endVertex">the destination vertex</param>
        /// <returns>An array of the nodes within the path.</returns>
        public int[] getShortestPath(int startVertex, int endVertex)
        {
            if ((startVertex < 0) || (startVertex > graph.GetLength(0)))
                throw new ArgumentException("Error: Starting vertex does not exist!");

            if ((endVertex < 0) || (endVertex > graph.GetLength(0)))
                throw new ArgumentException("Error: Ending vertex does not exist!");

            //Console.WriteLine("Starting BFS.  Starting node is {0}", startVertex);
            int depth = 0;
            Queue<int> toVisitNow = new Queue<int>();
            Queue<int> toVisitNext = new Queue<int>();
            HashSet<int> visited = new HashSet<int>();
            int[] parents = new int[Order];
            for (int i = 0; i < Order; i++)
            {
                parents[i] = -1;
            }

            toVisitNow.Enqueue(startVertex);

            while ((toVisitNow.Count > 0) || (toVisitNext.Count > 0))
            {
                int currentNode = toVisitNow.Dequeue();
                if (currentNode == endVertex)
                {
                    int[] path = new int[depth + 1];
                    path[0] = currentNode;
                    int i = 1;
                    while (currentNode != startVertex)
                    {
                        currentNode = parents[currentNode];
                        path[i] = currentNode;
                        i++;
                    }

                    return path;
                }

                visited.Add(currentNode);
                for (int i = 0; i < Order; i++)
                {
                    if ((1 == graph[currentNode, i]) && !visited.Contains(i))
                    {
                        toVisitNext.Enqueue(i);
                        if (-1 == parents[i])
                        {
                            parents[i] = currentNode;
                        } 
                    }
                }

                if (0 == toVisitNow.Count)
                {
                    //Console.WriteLine("Depth {0} complete!", depth);
                    toVisitNow = toVisitNext;
                    toVisitNext = new Queue<int>();
                    depth++;
                }
            }


            //Console.WriteLine("No cycle found!");
            return new int[] { -1 };
        }


        /// <summary>
        /// The purpose of this method is to determine the girth of a graph; that
        /// is, the length of a shortest cycle contained in the graph.  If the 
        /// graph does not contain any cycles, if girth is mathematically defined 
        /// as infinity.  However, this method returns the value -1.
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
        /// <returns>True/False - whether the graph is connected.</returns>
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
        /// The purpose of this method is to determine if a path exists between the 
        /// starting vertex and the ending vertex.
        /// </summary>
        /// <param name="startVertex">the starting vertex</param>
        /// <param name="endVertex">the destination vertex</param>
        /// <returns>True/False - whether a path exists between the specified vertices.</returns>
        public bool isPath(int startVertex, int endVertex)
        {

            if (numEdges() == 0)
                return false;

            if ((startVertex < 0) || (startVertex > graph.GetLength(0)))
                throw new ArgumentException("Error: Starting vertex does not exist!");

            if ((endVertex < 0) || (endVertex > graph.GetLength(0)))
                throw new ArgumentException("Error: Ending vertex does not exist!");

            Queue<int> toVisit = new Queue<int>();
            HashSet<int> visited = new HashSet<int>();

            toVisit.Enqueue(startVertex);

            while (toVisit.Count > 0)
            {
                int currentNode = toVisit.Dequeue();
                if (currentNode == endVertex)
                    return true;

                visited.Add(currentNode);
                for (int i = 0; i < graph.GetLength(0); i++)
                {
                    if ((graph[currentNode, i] == 1) && !visited.Contains(i))
                    {
                        toVisit.Enqueue(i);
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// The purpose of this method is to return the value of the degree of 
        /// the given node within the class member graph; that is the number 
        /// of neighbors of the given vertex.
        /// </summary>
        /// <param name="node">The given vertex</param>
        /// <returns>The degree of the given vertex.</returns>
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
        /// The purpose of this method is to return the value of the smallest 
        /// degree within the class member graph; that is the fewest number 
        /// of neighbors had by any vertex.
        /// </summary>
        /// <returns>The minimum degree of the class member graph.</returns>
        public int minDegree()
        {
            int degree = graph.GetLength(0);
            int thisDegree = 0;

            for (int i = 0; i < graph.GetLength(0); i++)
            {
                thisDegree = getDegree(i);

                if (thisDegree < degree)
                    degree = thisDegree;
            }

            return degree;
        }


        /// <summary>
        /// The purpose of this method is to return the value of the largest 
        /// degree within the class member graph; that is the most number of 
        /// neighbors had by any vertex.
        /// </summary>
        /// <returns>The maximum degree of the class member graph.</returns>
        public int maxDegree()
        {
            int degree = 0;
            int thisDegree = 0;

            for (int i = 0; i < graph.GetLength(0); i++)
            {

                thisDegree = getDegree(i);

                if (thisDegree > degree)
                    degree = thisDegree;
            }

            return degree;
        }


        /// <summary>
        /// The purpose of this method is to determine is the class member graph
        /// is regular; that is, if each vertex has the same number of neighbors.
        /// </summary>
        /// <returns>True/False - whether the graph is  regular.</returns>
        public bool isRegular()
        {
            int degree = getDegree(0);

            //Console.WriteLine("Node 0 had degree {0}.", degree);

            for (int i = 1; i < graph.GetLength(0); i++)
            {
                int thisDegree = getDegree(i);

                //Console.WriteLine("Node {0} had degree {1}.", i, thisDegree);
                if (degree != thisDegree)
                    return false;
            }

            return true;
        }


        /// <summary>
        /// Returns the degree distribution of the adjacency matrix.  
        /// The index of the returned array corresponds to the possible
        /// node degree.  Each value in the array corresponds to the 
        /// number of nodes that have that degree.
        /// </summary>
        /// <returns>The degree distribution of the adjacency matrix.</returns>
        public int[] degreeDistro()
        {
            int[] distro = new int[maxDegree() + 1];

            for (int i = 0; i < graph.GetLength(0); i++)
            {
                distro[getDegree(i)]++;
            }

            return distro;
        }



        /// <summary>
        /// The purpose of this method is to modify the class member graph by
        /// removing the specified row and column from the adjecency matrix.
        /// </summary>
        /// <param name="rowToRemove">The row to remove.</param>
        /// <param name="columnToRemove">The column to remove.</param>
        public void trimArray(int rowToRemove, int columnToRemove)
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
        /// graph; that is, the number of edges within the graph.  This method 
        /// assumes that the graph is undirected and unweighted.  Thus, the 
        /// existence of an edge will be denoted by the value 1 and that both 
        /// graph[i, j] == 1 and graph[j, i] == 1.  Thus, the edge count is 
        /// divided by 2 prior to being returned.
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
        /// The purpose of this method is to convert the values contained in 
        /// class member graph to a string.  It presents the array as a
        /// simple square matrix and does not assume anything about the size
        /// of the array.
        /// </summary>
        public override String ToString()
        {
            String s = "";

            for (int i = 0; i < graph.GetLength(0); i++)
            {
                for (int j = 0; j < graph.GetLength(1); j++)
                {
                    s += graph[i, j];
                    s += " ";
                }
                s += "\n";
            }

            return s;
        }
    }
}
