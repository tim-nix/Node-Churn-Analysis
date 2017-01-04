using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkSimulation;


namespace NetworkSimulationUnitTests
{
    [TestClass]
    public class NodeTimelineTests
    {
        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void ConstructorTest1()
        {
            NodeTimeline nodeTL = new NodeTimeline(0, 100.0);
        }


        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void ConstructorTest2()
        {
            NodeTimeline nodeTL = new NodeTimeline(10, -0.1);
        }


        [TestMethod]
        public void generateUUTimelineTest1()
        {
            NodeTimeline nodeTL = new NodeTimeline(1000, 200);
            nodeTL.generateUUTimeline();

            Assert.IsTrue(nodeTL.getFirstTime() >= nodeTL.BaseTime, "First session start time should be at or after basetime.");
        }


        [TestMethod]
        public void generateUUTimelineTest2()
        {
            NodeTimeline nodeTL = new NodeTimeline(1000, 200);
            nodeTL.generateUUTimeline();

            double avgUp = nodeTL.averageUpTime();
            Assert.IsTrue((avgUp > 0.48) && (avgUp < 0.52), "Expected time of (uniform) up time is 0.5");

            double avgDown = nodeTL.averageDownTime();
            Assert.IsTrue((avgUp > 0.48) && (avgUp < 0.52), "Expected time of (uniform) down time is 0.5");
        }


        [TestMethod]
        public void generatePETimelineTest1()
        {
            NodeTimeline nodeTL = new NodeTimeline(1000, 200);
            nodeTL.generatePETimeline();

            Assert.IsTrue(nodeTL.getFirstTime() >= nodeTL.BaseTime, "First session start time should be at or after basetime.");
        }


        [TestMethod]
        public void generatePETimelineTest2()
        {
            NodeTimeline nodeTL = new NodeTimeline(10000, 200);
            double alpha = 3.0;
            double beta = 1.0;
            double lambda = 2.0;
            nodeTL.generatePETimeline(alpha, beta, lambda);

            double delta = 0.02;
            double expP = beta / (alpha - 1);
            double expE = 1.0 / lambda;
            double avgUp = nodeTL.averageUpTime();
            Assert.IsTrue((avgUp > (expP - delta)) && (avgUp < (expP + delta)), "Expected time of (Paretto) up time is 0.5");

            double avgDown = nodeTL.averageDownTime();
            Assert.IsTrue((avgUp > (expE - delta)) && (avgUp < (expE + delta)), "Expected time of (exponential) down time is 0.5");
        }


        [TestMethod]
        public void generatePPTimelineTest1()
        {
            NodeTimeline nodeTL = new NodeTimeline(1000, 200);
            nodeTL.generatePPTimeline();

            Assert.IsTrue(nodeTL.getFirstTime() >= nodeTL.BaseTime, "First session start time should be at or after basetime.");
        }


        [TestMethod]
        public void generatePPTimelineTest2()
        {
            NodeTimeline nodeTL = new NodeTimeline(10000, 200);
            double alpha = 3.0;
            double beta = 1.0;
            nodeTL.generatePPTimeline(alpha, beta);

            double delta = 0.02;
            double expP = beta / (alpha - 1);
            double avgUp = nodeTL.averageUpTime();
            Assert.IsTrue((avgUp > (expP - delta)) && (avgUp < (expP + delta)), "Expected time of (Paretto) up time is 0.5");

            double avgDown = nodeTL.averageDownTime();
            Assert.IsTrue((avgUp > (expP - delta)) && (avgUp < (expP + delta)), "Expected time of (Paretto) down time is 0.5");
        }


        [TestMethod]
        public void generateEETimelineTest1()
        {
            NodeTimeline nodeTL = new NodeTimeline(1000, 200);
            nodeTL.generateEETimeline();

            Assert.IsTrue(nodeTL.getFirstTime() >= nodeTL.BaseTime, "First session start time should be at or after basetime.");
        }


        [TestMethod]
        public void generateEETimelineTest2()
        {
            NodeTimeline nodeTL = new NodeTimeline(10000, 200);
            double lambda = 2.0;
            nodeTL.generateEETimeline(lambda);

            double delta = 0.02;
            double expE = 1.0 / lambda;
            double avgUp = nodeTL.averageUpTime();
            Assert.IsTrue((avgUp > (expE - delta)) && (avgUp < (expE + delta)), "Expected time of (exponential) up time is 0.5");

            double avgDown = nodeTL.averageDownTime();
            Assert.IsTrue((avgUp > (expE - delta)) && (avgUp < (expE + delta)), "Expected time of (exponential) down time is 0.5");
        }


        [TestMethod]
        [ExpectedException(typeof(System.NullReferenceException))]
        public void getFirstTimeTest1()
        {
            NodeTimeline nodeTL = new NodeTimeline(1000, 200);
            nodeTL.getFirstTime();
        }


        [TestMethod]
        public void getFirstTimeTest2()
        {
            NodeTimeline nodeTL = new NodeTimeline(1000, 200);
            nodeTL.generateUUTimeline();
            Session[] tl = nodeTL.TimeLine;

            Assert.AreEqual(tl[0].StartTime, nodeTL.getFirstTime(), "getFirstTime should return the earliest time in the timeline.");
        }


        [TestMethod]
        [ExpectedException(typeof(System.NullReferenceException))]
        public void getFinalTimeTest1()
        {
            NodeTimeline nodeTL = new NodeTimeline(1000, 200);
            nodeTL.getFinalTime();
        }


        [TestMethod]
        public void getFinalTimeTest2()
        {
            NodeTimeline nodeTL = new NodeTimeline(1000, 200);
            nodeTL.generateUUTimeline();
            Session[] tl = nodeTL.TimeLine;

            Assert.AreEqual(tl[tl.Length - 1].EndTime, nodeTL.getFinalTime(), "getFinalTime should return the last stop time in the timeline.");
        }


        [TestMethod]
        [ExpectedException(typeof(System.NullReferenceException))]
        public void timeIsLiveTest1()
        {
            NodeTimeline nodeTL = new NodeTimeline(1000, 200);
            nodeTL.timeIsLive(205.0);
        }


        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void timeIsLiveTest2()
        {
            NodeTimeline nodeTL = new NodeTimeline(0, 100.0);
        }


        [TestMethod]
        public void timeIsLiveTest3()
        {
            NodeTimeline nodeTL = new NodeTimeline(100, 100.0);
            nodeTL.generateUUTimeline();
            Session[] tl = nodeTL.TimeLine;

            double time_t = tl[50].StartTime + (tl[50].getDurationLive() / 2.0);

            Assert.IsTrue(nodeTL.timeIsLive(time_t), "timeIsLive is inaccurately reporting a live time as down");
        }


        [TestMethod]
        public void timeIsLiveTest4()
        {
            NodeTimeline nodeTL = new NodeTimeline(100, 100.0);
            nodeTL.generateUUTimeline();
            Session[] tl = nodeTL.TimeLine;

            double time_t = tl[20].EndTime + ((tl[21].StartTime - tl[20].EndTime) / 2.0);

            Assert.IsFalse(nodeTL.timeIsLive(time_t), "timeIsLive is inaccurately reporting a down time as live");
        }


        [TestMethod]
        [ExpectedException(typeof(System.NullReferenceException))]
        public void averageUpTimeTest1()
        {
            NodeTimeline nodeTL = new NodeTimeline(1000, 200);
            nodeTL.averageUpTime();
        }


        [TestMethod]
        public void averageUpTimeTest2()
        {
            NodeTimeline nodeTL = new NodeTimeline(1000, 200);
            nodeTL.generateUUTimeline();
            Session[] tl = nodeTL.TimeLine;

            double avg = 0.0;

            for (int i = 0; i < tl.Length; i++)
                avg += tl[i].getDurationLive();

            avg = avg / tl.Length;

            Assert.AreEqual(avg, nodeTL.averageUpTime(), "averageUpTime incorrectly calculated.");
        }


        [TestMethod]
        [ExpectedException(typeof(System.NullReferenceException))]
        public void averageDownTimeTest1()
        {
            NodeTimeline nodeTL = new NodeTimeline(1000, 200);
            nodeTL.averageDownTime();
        }


        [TestMethod]
        public void averageDownTimeTest2()
        {
            NodeTimeline nodeTL = new NodeTimeline(1000, 200);
            nodeTL.generateUUTimeline();
            Session[] tl = nodeTL.TimeLine;

            double avg = 0;

            for (int i = 1; i < tl.Length; i++)
                avg += (tl[i].StartTime - tl[i - 1].EndTime);

            avg = avg / (tl.Length - 1);

            Assert.AreEqual(avg, nodeTL.averageDownTime(), "averageDownTime incorrectly calculated.");
        }
    }
}
