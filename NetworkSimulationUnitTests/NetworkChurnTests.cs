using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkSimulation;

namespace NetworkSimulationUnitTests
{
    [TestClass]
    public class NetworkChurnTests
    {
        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void ConstructorTest1()
        {
            NetworkChurn netChurn = new NetworkChurn(0);
        }


        [TestMethod]
        public void generateUUChurnTest1()
        {
            ContinuousUniform upD = new ContinuousUniform(0.0, 1.0);
            ContinuousUniform downD = new ContinuousUniform(0.0, 1.0);
            double basetime = 200.0;
            NetworkChurn netChurn = new NetworkChurn(1000);
            netChurn.generateChurn(100, basetime, upD, downD);

            Assert.IsTrue(netChurn.getEarliestFirstTime() >= basetime, "Earliest session start time should be at or after basetime.");
        }


        [TestMethod]
        public void generateUUChurnTest2()
        {
            ContinuousUniform upD = new ContinuousUniform(0.0, 1.0);
            ContinuousUniform downD = new ContinuousUniform(0.0, 1.0);
            double basetime = 200.0;
            NetworkChurn netChurn = new NetworkChurn(1000);
            netChurn.generateChurn(100, basetime, upD, downD);

            double avgUp = netChurn.getAverageUpTime();
            Assert.IsTrue((avgUp > 0.48) && (avgUp < 0.52), "Expected time of (uniform) up time is 0.5");

            double avgDown = netChurn.getAverageDownTime();
            Assert.IsTrue((avgDown > 0.48) && (avgDown < 0.52), "Expected time of (uniform) down time is 0.5");
        }


        [TestMethod]
        public void generatePEChurnTest1()
        {
            double basetime = 200.0;
            double alpha = 3.0;
            double beta = 1.0;
            double lambda = 2.0;
            Paretto upD = new Paretto(alpha, beta);
            Exponential downD = new Exponential(lambda);
            NetworkChurn netChurn = new NetworkChurn(1000);
            netChurn.generateChurn(100, basetime, upD, downD);

            Assert.IsTrue(netChurn.getEarliestFirstTime() >= basetime, "Earliest session start time should be at or after basetime.");
        }


        [TestMethod]
        public void generatePEChurnTest2()
        {
            double basetime = 200.0;
            double alpha = 3.0;
            double beta = 1.0;
            double lambda = 2.0;
            Paretto upD = new Paretto(alpha, beta);
            Exponential downD = new Exponential(lambda);
            NetworkChurn netChurn = new NetworkChurn(1000);
            netChurn.generateChurn(100, basetime, upD, downD);

            double delta = 0.02;
            double expP = beta / (alpha - 1);
            double expE = 1.0 / lambda;
            double avgUp = netChurn.getAverageUpTime();
            Assert.IsTrue((avgUp > (expP - delta)) && (avgUp < (expP + delta)), "Expected time of (Paretto) up time is 0.5");

            double avgDown = netChurn.getAverageDownTime();
            Assert.IsTrue((avgDown > (expE - delta)) && (avgDown < (expE + delta)), "Expected time of (exponential) down time is 0.5");
        }


        [TestMethod]
        public void generatePPChurnTest1()
        {
            double basetime = 200.0;
            double alpha = 3.0;
            double beta = 1.0;
            Paretto upD = new Paretto(alpha, beta);
            Paretto downD = new Paretto(alpha, beta);
            NetworkChurn netChurn = new NetworkChurn(1000);
            netChurn.generateChurn(100, basetime, upD, downD);

            Assert.IsTrue(netChurn.getEarliestFirstTime() >= basetime, "Earliest session start time should be at or after basetime.");
        }


        [TestMethod]
        public void generatePPChurnTest2()
        {
            double basetime = 200.0;
            double alpha = 3.0;
            double beta = 1.0;
            Paretto upD = new Paretto(alpha, beta);
            Paretto downD = new Paretto(alpha, beta);
            NetworkChurn netChurn = new NetworkChurn(1000);
            netChurn.generateChurn(100, basetime, upD, downD);

            double delta = 0.02;
            double expP = beta / (alpha - 1);
            double avgUp = netChurn.getAverageUpTime();
            Assert.IsTrue((avgUp > (expP - delta)) && (avgUp < (expP + delta)), "Expected time of (Paretto) up time is 0.5");

            double avgDown = netChurn.getAverageDownTime();
            Assert.IsTrue((avgDown > (expP - delta)) && (avgDown < (expP + delta)), "Expected time of (Paretto) down time is 0.5");
        }


        [TestMethod]
        public void generateEEChurnTest1()
        {
            double basetime = 200.0;
            double lambda = 2.0;
            Exponential upD = new Exponential(lambda);
            Exponential downD = new Exponential(lambda);
            NetworkChurn netChurn = new NetworkChurn(1000);
            netChurn.generateChurn(100, basetime, upD, downD);

            Assert.IsTrue(netChurn.getEarliestFirstTime() >= basetime, "Earliest session start time should be at or after basetime.");
        }


        [TestMethod]
        public void generateEEChurnTest2()
        {
            double basetime = 200.0;
            double lambda = 2.0;
            Exponential upD = new Exponential(lambda);
            Exponential downD = new Exponential(lambda);
            NetworkChurn netChurn = new NetworkChurn(1000);
            netChurn.generateChurn(100, basetime, upD, downD);

            double delta = 0.02;
            double expE = 1.0 / lambda;
            double avgUp = netChurn.getAverageUpTime();
            Assert.IsTrue((avgUp > (expE - delta)) && (avgUp < (expE + delta)), "Expected time of (exponential) up time is 0.5");

            double avgDown = netChurn.getAverageDownTime();
            Assert.IsTrue((avgDown > (expE - delta)) && (avgDown < (expE + delta)), "Expected time of (exponential) down time is 0.5");
        }


        [TestMethod]
        [ExpectedException(typeof(System.NullReferenceException))]
        public void getStatusAtTimeTest1()
        {
            NetworkChurn netChurn = new NetworkChurn(1000);
            netChurn.getStatusAtTime(205);
        }


        [TestMethod]
        public void getStatusAtTimeTest2()
        {
            double basetime = 200.0;
            double alpha = 3.0;
            double beta = 1.0;
            double lambda = 2.0;
            Paretto upD = new Paretto(alpha, beta);
            Exponential downD = new Exponential(lambda);
            double time_t = 205.5;
            NetworkChurn netChurn = new NetworkChurn(1000);
            netChurn.generateChurn(100, basetime, upD, downD);

            bool[] statusAtT = netChurn.getStatusAtTime(time_t);
            bool actualStatus = statusAtT[500];

            NodeTimeline tl = netChurn[500];

            bool expectedStatus = tl.timeIsLive(time_t);

            Assert.AreEqual(expectedStatus, actualStatus, "Problem with getStatusAtTime.");
        }


        [TestMethod]
        [ExpectedException(typeof(System.NullReferenceException))]
        public void getEarliestFirstTimeTest1()
        {
            NetworkChurn netChurn = new NetworkChurn(1000);
            netChurn.getEarliestFirstTime();
        }


        [TestMethod]
        public void getEarliestFirstTimeTest2()
        {
            double basetime = 200.0;
            double alpha = 3.0;
            double beta = 1.0;
            double lambda = 2.0;
            Paretto upD = new Paretto(alpha, beta);
            Exponential downD = new Exponential(lambda);
            int numSessions = 100;
            int numNodes = 1000;
            NetworkChurn netChurn = new NetworkChurn(numNodes);
            netChurn.generateChurn(numSessions, basetime, upD, downD);

            double actualEarliest = netChurn.getEarliestFirstTime();

            double expectedEarliest = netChurn[0].getFirstTime();

            for (int i = 1; i < numNodes; i++)
            {
                if (netChurn[i].getFirstTime() < expectedEarliest)
                    expectedEarliest = netChurn[i].getFirstTime();
            }

            Assert.AreEqual(expectedEarliest, actualEarliest, "Problem with getEarliestFirstTime.");
        }


        [TestMethod]
        [ExpectedException(typeof(System.NullReferenceException))]
        public void getEarliestFinalTimeTest1()
        {
            NetworkChurn netChurn = new NetworkChurn(1000);
            netChurn.getEarliestFinalTime();
        }


        [TestMethod]
        public void getEarliestFinalTimeTest2()
        {
            double basetime = 200.0;
            double alpha = 3.0;
            double beta = 1.0;
            double lambda = 2.0;
            Paretto upD = new Paretto(alpha, beta);
            Exponential downD = new Exponential(lambda);
            int numSessions = 100;
            int numNodes = 1000;
            NetworkChurn netChurn = new NetworkChurn(numNodes);
            netChurn.generateChurn(numSessions, basetime, upD, downD);

            double actualEarliest = netChurn.getEarliestFinalTime();

            double expectedEarliest = netChurn[0].getFinalTime();

            for (int i = 1; i < numNodes; i++)
            {
                if (netChurn[i].getFinalTime() < expectedEarliest)
                    expectedEarliest = netChurn[i].getFinalTime();
            }

            Assert.AreEqual(expectedEarliest, actualEarliest, "Problem with getEarliestFirstTime.");
        }


        [TestMethod]
        [ExpectedException(typeof(System.NullReferenceException))]
        public void getAverageUpTimeTest1()
        {
            NetworkChurn netChurn = new NetworkChurn(1000);
            netChurn.getAverageUpTime();
        }


        [TestMethod]
        public void getAverageUpTimeTest2()
        {
            double basetime = 200.0;
            double alpha = 3.0;
            double beta = 1.0;
            double lambda = 2.0;
            Paretto upD = new Paretto(alpha, beta);
            Exponential downD = new Exponential(lambda);
            int numSessions = 100;
            int numNodes = 1000;
            NetworkChurn netChurn = new NetworkChurn(numNodes);
            netChurn.generateChurn(numSessions, basetime, upD, downD);

            double delta = 0.02;
            double expP = beta / (alpha - 1);
            double avgUp = netChurn.getAverageUpTime();
            Assert.IsTrue((avgUp > (expP - delta)) && (avgUp < (expP + delta)), "Expected time of (Paretto) up time is 0.5");
        }


        [TestMethod]
        [ExpectedException(typeof(System.NullReferenceException))]
        public void getAverageDownTimeTest1()
        {
            NetworkChurn netChurn = new NetworkChurn(1000);
            netChurn.getAverageDownTime();
        }


        [TestMethod]
        public void getAverageDownTimeTest2()
        {
            double basetime = 200.0;
            double alpha = 3.0;
            double beta = 1.0;
            double lambda = 2.0;
            Paretto upD = new Paretto(alpha, beta);
            Exponential downD = new Exponential(lambda);
            int numSessions = 100;
            int numNodes = 1000;
            NetworkChurn netChurn = new NetworkChurn(numNodes);
            netChurn.generateChurn(numSessions, basetime, upD, downD);

            double delta = 0.02;
            double expE = 1.0 / lambda;
            double avgDown = netChurn.getAverageDownTime();
            Assert.IsTrue((avgDown > (expE - delta)) && (avgDown < (expE + delta)), "Expected time of (exponential) down time is 0.5");
        }
    }
}
