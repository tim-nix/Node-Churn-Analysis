using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    class AdjacencyMatrix
    {
        int[,] graph;

        public AdjacencyMatrix(int[,] g)
        {
            if (g.GetLength(0) == g.GetLength(1))
            {
                graph = g;
            }
            else
                throw new ArgumentException("Error: Square matrix is required!");
        }

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

        public bool isConnected()
        {
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

        public void setGraph(Bits bitArray)
        {
            for (int i = 0; i < graph.GetLength(0); i++)
            {
                for (int j = 0; j < graph.GetLength(1); j++)
                {
                    graph[i, j] = 0;
                }
            }

            int bitIndex = 0;
            for (int i = 0; i < graph.GetLength(0); i++)
            {
                for (int j = i + 1; j < graph.GetLength(0); j++)
                {
                    if (bitArray.BitArray[bitIndex])
                    {
                        graph[i, j] = 1;
                        graph[j, i] = 1;
                    }
                    bitIndex++;
                }
            }
        }

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
