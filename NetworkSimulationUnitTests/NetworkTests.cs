using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkSimulation;

namespace NetworkSimulationUnitTests
{
    [TestClass]
    public class NetworkTests
    {
        [TestMethod]
        public void updateStatus1()
        {
            int n = 100;
            Network network = new Network(CommonGraphs.Path(n));

            bool[] live = new bool[n];
            for (int i = 0; i < n; i++)
                live[i] = true;

            network.updateStatus(live);

            int[,] current = network.CurrentGraph;

            Assert.AreEqual(current.GetLength(0), n, "Path should not have changed.");
            Assert.AreEqual(current.GetLength(1), n, "Path should not have changed.");

            int[,] original = CommonGraphs.Path(n);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Assert.AreEqual(original[i, j], current[i, j], "Path connections should not have changed.");
                }
            }
        }


        [TestMethod]
        public void updateStatus2()
        {
            int n = 100;
            Network network = new Network(CommonGraphs.Cycle(n));

            bool[] live = new bool[n];
            for (int i = 0; i < n; i++)
                live[i] = false;

            live[0] = true;
            live[n - 1] = true;

            network.updateStatus(live);

            int[,] current = network.CurrentGraph;

            Assert.AreEqual(current.GetLength(0), 2, "Cycle should only have two live nodes.");
            Assert.AreEqual(current.GetLength(1), 2, "Cycle should only have two live nodes.");
            Assert.AreEqual(current[0, 1], 1, "Cycle nodes should be connected.");
            Assert.AreEqual(current[1, 0], 1, "Cycle nodes should be connected.");
        }


        [TestMethod]
        public void isCurrentNetworkConnectedTest1()
        {
            int n = 100;
            Network network = new Network(CommonGraphs.Path(n));

            Assert.IsTrue(network.isCurrentNetworkConnected(), "Path should be connected.");
        }


        [TestMethod]
        public void isCurrentNetworkConnectedTest2()
        {
            int n = 100;
            Network network = new Network(CommonGraphs.Path(n));

            bool[] live = new bool[n];
            for (int i = 0; i < n; i++)
            {
                if (i % 2 == 0)
                    live[i] = false;
                else
                    live[i] = true;
            }
            network.updateStatus(live);

            Assert.IsFalse(network.isCurrentNetworkConnected(), "Path should be broken.");
        }


        [TestMethod]
        public void isCurrentNetworkConnectedTest3()
        {
            int n = 100;
            Network network = new Network(CommonGraphs.Clique(n));

            bool[] live = new bool[n];
            for (int i = 0; i < n; i++)
            {
                if (i % 2 == 0)
                    live[i] = false;
                else
                    live[i] = true;
            }
            network.updateStatus(live);

            Assert.IsTrue(network.isCurrentNetworkConnected(), "Clique should not be broken.");
        }


        [TestMethod]
        public void isCurrentNetworkConnectedTest4()
        {
            int[,] am = new int[,] {{0, 1, 1, 1, 1, 0, 0, 0, 0, 0 },
                                    {1, 0, 1, 0, 1, 0, 0, 0, 0, 0 },
                                    {1, 1, 0, 1, 0, 0, 0, 0, 0, 0 },
                                    {1, 0, 1, 0, 1, 0, 0, 0, 0, 0 },
                                    {1, 1, 0, 1, 0, 0, 0, 0, 0, 0 },
                                    {0, 0, 0, 0, 0, 0, 0, 1, 0, 1 },
                                    {0, 0, 0, 0, 0, 0, 0, 1, 1, 0 },
                                    {0, 0, 0, 0, 0, 1, 1, 0, 0, 1 },
                                    {0, 0, 0, 0, 0, 1, 1, 0, 0, 1 },
                                    {0, 0, 0, 0, 0, 0, 0, 1, 1, 0 } };

            Network network = new Network(am);

            Assert.IsFalse(network.isCurrentNetworkConnected(), "Given adjacency matrix should not be connected.");
        }


        [TestMethod]
        public void getFullDegreeTest1()
        {
            int n = 100;
            Network network = new Network(CommonGraphs.Path(n));

            Assert.AreEqual(network.getFullDegree(0), 1, "A path end node should have a degree of 1.");
            Assert.AreEqual(network.getFullDegree(50), 2, "A path interior node should have a degree of 2.");
        }


        [TestMethod]
        public void getFullDegreeTest2()
        {
            int n = 20;

            Network network = new Network(CommonGraphs.Clique(n));

            Assert.AreEqual(network.getFullDegree(n - 1), n - 1, "Any clique node should have a degree of n - 1.");
        }


        [TestMethod]
        public void getFullDegreeTest3()
        {
            int n = 10;

            Network network = new Network(CommonGraphs.Cycle(n));

            Assert.AreEqual(network.getFullDegree(n / 2), 2, "Any cycle node should have a degree of 2.");
        }


        [TestMethod]
        public void getFullDegreeTest4()
        {
            Network network = new Network(CommonGraphs.PetersonGraph);

            Assert.AreEqual(network.getFullDegree(3), 3, "Any Peterson Graph node should have a degree of 3.");
        }


        [TestMethod]
        public void getFullDegreeTest5()
        {
            int c = 4;
            Network network = new Network(CommonGraphs.GuntherHartnell(c * (c + 1)));

            Assert.AreEqual(network.getFullDegree((c * (c + 1)) / 2), c, "Any node in a full Gunther-Hartnell graph should have a degree same as clique size.");
        }


        [TestMethod]
        public void getCurrentDegreeTest1()
        {
            int n = 100;
            Network network = new Network(CommonGraphs.Path(n));

            bool[] live = new bool[n];
            for (int i = 0; i < n; i++)
            {
                if (i % 2 == 0)
                    live[i] = false;
                else
                    live[i] = true;
            }
            network.updateStatus(live);

            for (int i = 0; i < n; i++)
            {
                if (i % 2 == 0)
                    Assert.AreEqual(network.getCurrentDegree(i), -1, "Node " + i + " is not live.");
                else
                    Assert.AreEqual(network.getCurrentDegree(i), 0, "Node " + i + " is live and has degree 0.");
            }
        }


        [TestMethod]
        public void getCurrentDegreeTest2()
        {
            int n = 20;

            Network network = new Network(CommonGraphs.Clique(n));

            bool[] live = new bool[n];
            for (int i = 0; i < n; i++)
            {
                if (i % 2 == 0)
                    live[i] = false;
                else
                    live[i] = true;
            }
            network.updateStatus(live);

            for (int i = 0; i < n; i++)
            {
                if (i % 2 == 0)
                    Assert.AreEqual(network.getCurrentDegree(i), -1, "Node " + i + " is not live.");
                else
                    Assert.AreEqual(network.getCurrentDegree(i), (n / 2) - 1, "Node " + i + " is live and has degree " + ((n / 2) - 1) + ".");
            }
        }


        [TestMethod]
        public void getCurrentDegreeTest3()
        {
            int n = 10;

            Network network = new Network(CommonGraphs.Cycle(n));

            bool[] live = new bool[n];
            for (int i = 0; i < n; i++)
            {
                if (i % 2 == 0)
                    live[i] = false;
                else
                    live[i] = true;
            }
            network.updateStatus(live);

            for (int i = 0; i < n; i++)
            {
                if (i % 2 == 0)
                    Assert.AreEqual(network.getCurrentDegree(i), -1, "Node " + i + " is not live.");
                else
                    Assert.AreEqual(network.getCurrentDegree(i), 0, "Node " + i + " is live and has degree 0.");
            }
        }


        [TestMethod]
        public void getDistroFullTest1()
        {
            int n = 100;
            Network network = new Network(CommonGraphs.Path(n));

            int[] distro = network.getDistroFull();

            Assert.AreEqual(distro[1], 2, "A path end node should have 2 nodes if degree 1.");
            Assert.AreEqual(distro[2], n - 2, "A path end node should have n - 2 nodes if degree 2.");
        }


        [TestMethod]
        public void getDistroFullTest2()
        {
            int n = 20;
            Network network = new Network(CommonGraphs.Clique(n));

            int[] distro = network.getDistroFull();

            Assert.AreEqual(distro[n - 1], n, "All clique nodes should have degree of n - 1.");
        }


        [TestMethod]
        public void getDistroFullTest3()
        {
            int n = 10;
            Network network = new Network(CommonGraphs.Cycle(n));

            int[] distro = network.getDistroFull();

            Assert.AreEqual(distro[2], n, "All cycle nodes should have a degree of 2.");
        }


        [TestMethod]
        public void getDistroFullTest4()
        {
            Network network = new Network(CommonGraphs.PetersonGraph);

            int[] distro = network.getDistroFull();

            Assert.AreEqual(distro[3], 10, "All Peterson Graph nodes should have a degree of 3.");
        }


        [TestMethod]
        public void getDistroFullTest5()
        {
            int c = 4;
            Network network = new Network(CommonGraphs.GuntherHartnell(c * (c + 1)));

            int[] distro = network.getDistroFull();

            Assert.AreEqual(distro[c], c * (c + 1), "All full Gunther-Hartnell graph nodes should have a degree same as clique size.");
        }


        [TestMethod]
        public void degreeDistroTest6()
        {
            int[,] am = new int[,] {{0, 1, 1, 1, 1, 0, 0, 0, 0, 0 },
                                    {1, 0, 1, 0, 1, 0, 0, 0, 0, 0 },
                                    {1, 1, 0, 1, 0, 0, 0, 0, 0, 0 },
                                    {1, 0, 1, 0, 1, 0, 0, 0, 0, 0 },
                                    {1, 1, 0, 1, 0, 0, 0, 0, 0, 0 },
                                    {0, 0, 0, 0, 0, 0, 0, 1, 0, 1 },
                                    {0, 0, 0, 0, 0, 0, 0, 1, 1, 0 },
                                    {0, 0, 0, 0, 0, 1, 1, 0, 0, 1 },
                                    {0, 0, 0, 0, 0, 1, 1, 0, 0, 1 },
                                    {0, 0, 0, 0, 0, 0, 0, 1, 1, 0 } };

            Network network = new Network(am);

            int[] distro = network.getDistroFull();

            Assert.AreEqual(distro[0], 0, "Example graph has 0 nodes of degree 0.");
            Assert.AreEqual(distro[1], 0, "Example graph has 0 nodes of degree 1.");
            Assert.AreEqual(distro[2], 3, "Example graph has 3 nodes of degree 2.");
            Assert.AreEqual(distro[3], 6, "Example graph has 6 nodes of degree 3.");
            Assert.AreEqual(distro[4], 1, "Example graph has 1 nodes of degree 4.");
        }


        [TestMethod]
        public void getDistroCurrentTest1()
        {
            int n = 100;
            Network network = new Network(CommonGraphs.Path(n));

            bool[] live = new bool[n];
            for (int i = 0; i < n; i++)
            {
                if (i % 2 == 0)
                    live[i] = false;
                else
                    live[i] = true;
            }
            network.updateStatus(live);
            int[] distro = network.getDistroCurrent();            

            Assert.AreEqual(n / 2, distro[0], "All nodes should have degree 0.");
        }


        [TestMethod]
        public void getDistroCurrentTest2()
        {
            int n = 20;
            Network network = new Network(CommonGraphs.Clique(n));

            bool[] live = new bool[n];
            for (int i = 0; i < n; i++)
            {
                if (i % 2 == 0)
                    live[i] = false;
                else
                    live[i] = true;
            }
            network.updateStatus(live);
            int[] distro = network.getDistroCurrent();

            Assert.AreEqual(n / 2, distro[distro.Length - 1], "All nodes should have degree " + (distro.Length - 1) + ".");
        }


        [TestMethod]
        public void getDistroCurrentTest3()
        {
            int n = 10;
            Network network = new Network(CommonGraphs.Cycle(n));

            bool[] live = new bool[n];
            for (int i = 0; i < n; i++)
            {
                if (i % 2 == 0)
                    live[i] = false;
                else
                    live[i] = true;
            }
            network.updateStatus(live);
            int[] distro = network.getDistroCurrent();

            Assert.AreEqual(n / 2, distro[0], "All nodes should have degree 0.");
        }


        [TestMethod]
        public void getDistanceDistroFullTest1()
        {
            int n = 10;
            int startVertex = 0;
            Network network = new Network(CommonGraphs.Path(n));

            int[] distances = network.getDistanceDistroFull(startVertex);

            for (int i = 0; i < n - 1; i++)
            {
                Assert.AreEqual(distances[i], 1, "In a path, every node should be a unique distance from the first node.");
            }
        }


        [TestMethod]
        public void getDistanceDistroFullTest2()
        {
            int n = 11;
            int startVertex = n / 2;
            Network network = new Network(CommonGraphs.Path(n));

            int[] distances = network.getDistanceDistroFull(startVertex);

            Assert.AreEqual(distances[0], 1, "In any simple graph, a node is 0 distance from itself.");

            for (int i = 1; i <= startVertex; i++)
            {
                Assert.AreEqual(distances[i], 2, "In a path, two nodes should be a unique distance from the center node.");
            }

            for (int i = startVertex + 1; i < n; i++)
            {
                Assert.AreEqual(distances[i], 0, "In a path, no node should be farther away than n/2 from the center node.");
            }
        }


        [TestMethod]
        public void getDistanceDistroTFullest3()
        {
            int n = 11;
            int startVertex = 0;
            Network network = new Network(CommonGraphs.Cycle(n));

            int[] distances = network.getDistanceDistroFull(startVertex);

            Assert.AreEqual(distances[0], 1, "In any simple graph, a node is 0 distance from itself.");

            for (int i = 1; i <= n / 2; i++)
            {
                Assert.AreEqual(distances[i], 2, "In a cycle with odd number of nodes, two nodes should be a unique distance from the center node.");
            }

            for (int i = n / 2 + 1; i < n; i++)
            {
                Assert.AreEqual(distances[i], 0, "In a cycle with odd number of nodes, no node should be farther away than n/2 from the center node.");
            }
        }


        [TestMethod]
        public void getDistanceDistroFullTest4()
        {
            int n = 10;
            int startVertex = 0;
            Network network = new Network(CommonGraphs.Cycle(n));

            int[] distances = network.getDistanceDistroFull(startVertex);

            Assert.AreEqual(distances[0], 1, "In any simple graph, a node is 0 distance from itself.");

            for (int i = 1; i < n / 2 - 1; i++)
            {
                Assert.AreEqual(distances[i], 2, "In a cycle with even number of nodes, two nodes for each distance up to n/2.");
            }

            Assert.AreEqual(distances[n / 2], 1, "In a cycle with even number of nodes, one node for distance n/2.");

            for (int i = n / 2 + 1; i < n; i++)
            {
                Assert.AreEqual(distances[i], 0, "In a cycle with even number of nodes, no node should be farther away than n/2 from the center node.");
            }
        }


        [TestMethod]
        public void getShortestPathFullTest1()
        {
            int n = 100;
           Network graph = new Network(CommonGraphs.Path(n));

            int[] path = graph.getShortestPathFull(0, 99);

            for (int i = 0; i < path.Length; i++)
            {
                Assert.AreEqual(path[i], 99 - i, "A path must through every node to get from the first node to the last.");
            }
        }


        [TestMethod]
        public void getShortestPathTest2()
        {
            int n = 100;
            Network graph = new Network(CommonGraphs.Path(n));

            int[] path = graph.getShortestPathFull(99, 0);

            for (int i = 0; i < path.Length; i++)
            {
                Assert.AreEqual(path[i], i, "A path must through every node to get from the first node to the last.");
            }
        }


        [TestMethod]
        public void getShortestPathTest3()
        {
            int n = 100;
            int start = 25;
            int end = 75;
            Network graph = new Network(CommonGraphs.Clique(n));

            int[] path = graph.getShortestPathFull(start, end);

            Assert.AreEqual(path[0], end, "In a clique, the source is directly connected to the destination.");
            Assert.AreEqual(path[1], start, "In a clique, the source is directly connected to the destination.");
        }


        [TestMethod]
        public void getShortestPathTest4()
        {
            int n = 100;
            Network graph = new Network(CommonGraphs.Cycle(n));

            int[] path = graph.getShortestPathFull(5, 95);

            for (int i = 0; i < path.Length; i++)
            {
                Assert.AreEqual(path[i], (i + 95) % n, "In a cycle, the shortest path may cross the Node 0 to Node 99.");
            }
        }
    }
}
