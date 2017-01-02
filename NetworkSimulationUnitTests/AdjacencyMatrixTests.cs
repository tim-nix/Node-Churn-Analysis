using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkSimulation;

namespace NetworkSimulationUnitTests
{
    [TestClass]
    public class AdjacencyMatrixTests
    {
        [TestMethod]
        public void smallestCycleTest1()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.Path(100));

            Assert.AreEqual(graph.smallestCycle(0), -1, "A path should not have a cycle.");
        }


        [TestMethod]
        public void smallestCycleTest2()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.Clique(20));

            Assert.AreEqual(graph.smallestCycle(0), 3, "A clique should have a smallest cycle of 3.");
        }


        [TestMethod]
        public void smallestCycleTest3()
        {
            int n = 100;

            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.Cycle(n));

            Assert.AreEqual(graph.smallestCycle(n - 1), n, "A cycle should have a smallest cycle of n.");
        }


        [TestMethod]
        public void smallestCycleTest4()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.PetersonGraph);

            Assert.AreEqual(graph.smallestCycle(5), 5, "A Peterson Graph should have a minimum cycle size of 5.");
        }


        [TestMethod]
        public void smallestCycleTest5()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.GuntherHartnell(20));

            Assert.AreEqual(graph.smallestCycle(5), 3, "A Gunther-Hartnell graph will usually have a cycle size of 3.");
        }


        [TestMethod]
        public void girthTest1()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.Path(100));

            Assert.AreEqual(graph.girth(), -1, "A path should have a girth of infinity.");
        }


        [TestMethod]
        public void girthTest2()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.Clique(20));

            Assert.AreEqual(graph.girth(), 3, "A clique should have a girth of 3.");
        }


        [TestMethod]
        public void girthTest3()
        {
            int n = 100;

            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.Cycle(n));

            Assert.AreEqual(graph.girth(), n, "A cycle should have a girth of n.");
        }


        [TestMethod]
        public void girthTest4()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.PetersonGraph);

            Assert.AreEqual(graph.girth(), 5, "A Peterson Graph should have a girth of 5.");
        }


        [TestMethod]
        public void girthTest5()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.GuntherHartnell(20));

            Assert.AreEqual(graph.girth(), 3, "A Gunther-Hartnell graph with more than 6 nodes will have a girth of 3.");
        }


        [TestMethod]
        public void isConnectedTest1()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.Path(100));

            Assert.IsTrue(graph.isConnected(), "Path should be connected.");
        }


        [TestMethod]
        public void isConnectedTest2()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.Cycle(20));

            Assert.IsTrue(graph.isConnected(), "Cycle should be connected.");
        }


        [TestMethod]
        public void isConnectedTest3()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.Clique(20));

            Assert.IsTrue(graph.isConnected(), "Clique should be connected.");
        }


        [TestMethod]
        public void isConnectedTest4()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.PetersonGraph);

            Assert.IsTrue(graph.isConnected(), "Peterson graph should be connected.");
        }


        [TestMethod]
        public void isConnectedTest5()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.GuntherHartnell(20));

            Assert.IsTrue(graph.isConnected(), "Gunther-Hartnell graph should be connected.");
        }


        [TestMethod]
        public void isConnectedTest6()
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

            AdjacencyMatrix graph = new AdjacencyMatrix(am);

            Assert.IsFalse(graph.isConnected(), "Given adjacency matrix should not be connected.");
        }


        [TestMethod]
        public void isRegularTest1()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.Path(100));

            Assert.IsFalse(graph.isRegular(), "Path should not be regular.");
        }


        [TestMethod]
        public void isRegularTest2()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.Cycle(20));

            Assert.IsTrue(graph.isRegular(), "Cycle should be regular.");
        }


        [TestMethod]
        public void isRegularTest3()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.Clique(20));

            Assert.IsTrue(graph.isRegular(), "Clique should be regular.");
        }


        [TestMethod]
        public void isRegularTest4()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.PetersonGraph);

            Assert.IsTrue(graph.isRegular(), "Peterson graph should be regular.");
        }


        [TestMethod]
        public void isRegularTest5()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.GuntherHartnell(20));

            Assert.IsTrue(graph.isRegular(), "Full Gunther-Hartnell graph should be regular.");
        }


        [TestMethod]
        public void isRegularTest6()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.GuntherHartnell(19));

            Assert.IsFalse(graph.isRegular(), "Irregular Gunther-Hartnell graph should npt be regular.");
        }


        [TestMethod]
        public void isRegularTest7()
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

            AdjacencyMatrix graph = new AdjacencyMatrix(am);

            Assert.IsFalse(graph.isRegular(), "Given adjacency matrix should not be regular.");
        }


        [TestMethod]
        public void getDegreeTest1()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.Path(100));

            Assert.AreEqual(graph.getDegree(0), 1, "A path end node should have a degree of 1.");
            Assert.AreEqual(graph.getDegree(50), 2, "A path interior node should have a degree of 2.");
        }


        [TestMethod]
        public void getDegreeTest2()
        {
            int n = 20;

            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.Clique(n));

            Assert.AreEqual(graph.getDegree(10), n - 1, "Any clique node should have a degree of n - 1.");
        }


        [TestMethod]
        public void getDegreeTest3()
        {
            int n = 10;

            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.Cycle(n));

            Assert.AreEqual(graph.getDegree(n - 1), 2, "Any cycle node should have a degree of 2.");
        }


        [TestMethod]
        public void getDegreeTest4()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.PetersonGraph);

            Assert.AreEqual(graph.getDegree(5), 3, "Any Peterson Graph node should have a degree of 3.");
        }


        [TestMethod]
        public void getDegreeTest5()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.GuntherHartnell(20));

            Assert.AreEqual(graph.getDegree(9), 4, "Any node in a full Gunther-Hartnell graph should have a degree same as clique size.");
        }


        [TestMethod]
        public void minDegreeTest1()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.Path(100));

            Assert.AreEqual(graph.minDegree(), 1, "A path minimum degree is 1.");
        }


        [TestMethod]
        public void minDegreeTest2()
        {
            int n = 20;

            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.Clique(n));

            Assert.AreEqual(graph.minDegree(), n - 1, "A clique should have a minimum degree of n - 1.");
        }


        [TestMethod]
        public void minDegreeTest3()
        {
            int n = 10;

            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.Cycle(n));

            Assert.AreEqual(graph.minDegree(), 2, "A cycle should have a minimum degree of 2.");
        }


        [TestMethod]
        public void minDegreeTest4()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.PetersonGraph);

            Assert.AreEqual(graph.minDegree(), 3, "A Peterson Graph should have a minimum degree of 3.");
        }


        [TestMethod]
        public void minDegreeTest5()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.GuntherHartnell(20));

            Assert.AreEqual(graph.minDegree(), 4, "A full Gunther-Hartnell graph should have a minimum degree same as clique size.");
        }


        [TestMethod]
        public void maxDegreeTest1()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.Path(100));

            Assert.AreEqual(graph.maxDegree(), 2, "A path maximum degree is 2.");
        }


        [TestMethod]
        public void maxDegreeTest2()
        {
            int n = 20;

            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.Clique(n));

            Assert.AreEqual(graph.maxDegree(), n - 1, "A clique should have a maximum degree of n - 1.");
        }


        [TestMethod]
        public void maxDegreeTest3()
        {
            int n = 10;

            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.Cycle(n));

            Assert.AreEqual(graph.maxDegree(), 2, "A cycle should have a maximum degree of 2.");
        }


        [TestMethod]
        public void maxDegreeTest4()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.PetersonGraph);

            Assert.AreEqual(graph.maxDegree(), 3, "A Peterson Graph should have a maximum degree of 3.");
        }


        [TestMethod]
        public void maxDegreeTest5()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.GuntherHartnell(20));

            Assert.AreEqual(graph.maxDegree(), 4, "A full Gunther-Hartnell graph should have a minimum degree same as clique size.");
        }


        [TestMethod]
        public void trimArrayTest7()
        {
            int[,] am1 = new int[,] {{0, 1, 1, 1, 1, 0, 0, 0, 0, 0 },
                                     {1, 0, 1, 0, 1, 0, 0, 0, 0, 0 },
                                     {1, 1, 0, 1, 0, 0, 0, 0, 0, 0 },
                                     {1, 0, 1, 0, 1, 0, 0, 0, 0, 0 },
                                     {1, 1, 0, 1, 0, 0, 0, 0, 0, 0 },
                                     {0, 0, 0, 0, 0, 0, 0, 1, 0, 1 },
                                     {0, 0, 0, 0, 0, 0, 0, 1, 1, 0 },
                                     {0, 0, 0, 0, 0, 1, 1, 0, 0, 1 },
                                     {0, 0, 0, 0, 0, 1, 1, 0, 0, 1 },
                                     {0, 0, 0, 0, 0, 0, 0, 1, 1, 0 } };

            AdjacencyMatrix graph = new AdjacencyMatrix(am1);

            graph.trimArray(5, 5);

            int[,] am2 = new int[,] {{0, 1, 1, 1, 1, 0, 0, 0, 0 },
                                     {1, 0, 1, 0, 1, 0, 0, 0, 0 },
                                     {1, 1, 0, 1, 0, 0, 0, 0, 0 },
                                     {1, 0, 1, 0, 1, 0, 0, 0, 0 },
                                     {1, 1, 0, 1, 0, 0, 0, 0, 0 },
                                     {0, 0, 0, 0, 0, 0, 1, 1, 0 },
                                     {0, 0, 0, 0, 0, 1, 0, 0, 1 },
                                     {0, 0, 0, 0, 0, 1, 0, 0, 1 },
                                     {0, 0, 0, 0, 0, 0, 1, 1, 0 } };

            int[,] trimmedArray = graph.Graph;

            for (int i = 0; i < trimmedArray.GetLength(0); i++)
            {
                for (int j = 0; j < trimmedArray.GetLength(1); j++)
                {
                    Assert.AreEqual(am2[i, j], trimmedArray[i, j], "Two adjacency matrices should be equal.");
                }
            }
        }


        [TestMethod]
        public void numEdgesTest1()
        {
            int n = 100;

            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.Path(n));

            Assert.AreEqual(graph.numEdges(), n - 1, "A path should have n - 1 edges.");
        }


        [TestMethod]
        public void numEdgesTest2()
        {
            int n = 20;

            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.Clique(n));

            Assert.AreEqual(graph.numEdges(), (n * (n - 1)) / 2, "A clique should have n * (n - 1) / 2 edges.");
        }


        [TestMethod]
        public void numEdgesTest3()
        {
            int n = 10;

            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.Cycle(n));

            Assert.AreEqual(graph.numEdges(), n, "A cycle should have n edges.");
        }


        [TestMethod]
        public void numEdgesTest4()
        {
            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.PetersonGraph);

            Assert.AreEqual(graph.numEdges(), 15, "A Peterson Graph should have 15 edges.");
        }


        [TestMethod]
        public void numEdgesTest5()
        {
            int s = 4;

            AdjacencyMatrix graph = new AdjacencyMatrix(CommonGraphs.GuntherHartnell(s * (s + 1)));

            Assert.AreEqual(graph.numEdges(), ((s * s) * (s + 1)) / 2, "A full Gunther-Hartnell graph should have s^2 (s + 1) / 2 edges.");
        }
    }
}
