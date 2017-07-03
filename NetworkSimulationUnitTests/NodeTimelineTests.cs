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
            ContinuousUniform upD = new ContinuousUniform(0.0, 1.0);
            ContinuousUniform downD = new ContinuousUniform(0.0, 1.0);
            NodeTimeline nodeTL = new NodeTimeline(1000, 200);
            nodeTL.generateTimeline(upD, downD);

            Assert.IsTrue(nodeTL.getFirstEnd() >= nodeTL.BaseTime, "First session end time should be at or after basetime.");
        }


        [TestMethod]
        public void generateUUTimelineTest2()
        {
            ContinuousUniform upD = new ContinuousUniform(0.0, 1.0);
            ContinuousUniform downD = new ContinuousUniform(0.0, 1.0);
            NodeTimeline nodeTL = new NodeTimeline(1000, 200);
            nodeTL.generateTimeline(upD, downD);

            double avgUp = nodeTL.averageUpTime();
            Assert.IsTrue((avgUp > 0.48) && (avgUp < 0.52), "Actual (Uniform) up time is " + avgUp);

            double avgDown = nodeTL.averageDownTime();
            Assert.IsTrue((avgDown > 0.48) && (avgDown < 0.52), "Actual (Uniform) down time is " + avgDown);
        }


        [TestMethod]
        public void generatePETimelineTest1()
        {
            double alpha = 3.0;
            double beta = 1.0;
            double lambda = 2.0;
            Paretto upD = new Paretto(alpha, beta);
            Exponential downD = new Exponential(lambda);
            NodeTimeline nodeTL = new NodeTimeline(1000, 200.0);
            nodeTL.generateTimeline(upD, downD);

            Assert.IsTrue(nodeTL.getFirstEnd() >= nodeTL.BaseTime, "First session end time should be at or after basetime.");
        }


        [TestMethod]
        public void generatePETimelineTest2()
        {
            double alpha = 3.0;
            double beta = 1.0;
            double lambda = 2.0;
            Paretto upD = new Paretto(alpha, beta);
            Exponential downD = new Exponential(lambda);
            NodeTimeline nodeTL = new NodeTimeline(10000, 200.0);
            nodeTL.generateTimeline(upD, downD);

            double delta = 0.02;
            double expP = beta / (alpha - 1);
            double expE = 1.0 / lambda;
            double avgUp = nodeTL.averageUpTime();
            Assert.IsTrue((avgUp > (expP - delta)) && (avgUp < (expP + delta)), "Actual (Paretto) up time is " + avgUp);

            double avgDown = nodeTL.averageDownTime();
            Assert.IsTrue((avgDown > (expE - delta)) && (avgDown < (expE + delta)), "Actual (Paretto) down time is " + avgDown);
        }


        [TestMethod]
        public void generatePPTimelineTest1()
        {
            double alpha = 3.0;
            double beta = 1.0;
            Paretto upD = new Paretto(alpha, beta);
            Paretto downD = new Paretto(alpha, beta);
            NodeTimeline nodeTL = new NodeTimeline(10000, 200.0);
            nodeTL.generateTimeline(upD, downD);

            Assert.IsTrue(nodeTL.getFirstEnd() >= nodeTL.BaseTime, "First session end time should be at or after basetime.");
        }


        [TestMethod]
        public void generatePPTimelineTest2()
        {
            double alpha = 3.0;
            double beta = 1.0;
            Paretto upD = new Paretto(alpha, beta);
            Paretto downD = new Paretto(alpha, beta);
            NodeTimeline nodeTL = new NodeTimeline(10000, 200.0);
            nodeTL.generateTimeline(upD, downD);

            double delta = 0.02;
            double expP = beta / (alpha - 1);
            double avgUp = nodeTL.averageUpTime();
            Assert.IsTrue((avgUp > (expP - delta)) && (avgUp < (expP + delta)), "Actual (Paretto) up time is " + avgUp);

            double avgDown = nodeTL.averageDownTime();
            Assert.IsTrue((avgDown > (expP - delta)) && (avgDown < (expP + delta)), "Actual (Paretto) down time is " + avgDown);
        }


        [TestMethod]
        public void generateEETimelineTest1()
        {
            double lambda = 2.0;
            Exponential upD = new Exponential(lambda);
            Exponential downD = new Exponential(lambda);
            NodeTimeline nodeTL = new NodeTimeline(10000, 200.0);
            nodeTL.generateTimeline(upD, downD);

            Assert.IsTrue(nodeTL.getFirstEnd() >= nodeTL.BaseTime, "First session end time should be at or after basetime.");
        }


        [TestMethod]
        public void generateEETimelineTest2()
        {
            double lambda = 2.0;
            Exponential upD = new Exponential(lambda);
            Exponential downD = new Exponential(lambda);
            NodeTimeline nodeTL = new NodeTimeline(10000, 200.0);
            nodeTL.generateTimeline(upD, downD);

            double delta = 0.2;
            double expE = 1.0 / lambda;
            double avgUp = nodeTL.averageUpTime();
            Assert.IsTrue((avgUp > (expE - delta)) && (avgUp < (expE + delta)), "Actual (Exponential) up time is " + avgUp);

            double avgDown = nodeTL.averageDownTime();
            Assert.IsTrue((avgDown > (expE - delta)) && (avgDown < (expE + delta)), "Actual (Exponential) down time is " + avgDown);
        }


        [TestMethod]
        [ExpectedException(typeof(System.NullReferenceException))]
        public void getFirstTimeTest1()
        {
            NodeTimeline nodeTL = new NodeTimeline(10000, 200);
            nodeTL.getFirstTime();
        }


        [TestMethod]
        public void getFirstTimeTest2()
        {
            ContinuousUniform upD = new ContinuousUniform(0.0, 1.0);
            ContinuousUniform downD = new ContinuousUniform(0.0, 1.0);
            NodeTimeline nodeTL = new NodeTimeline(10000, 200);
            nodeTL.generateTimeline(upD, downD);
            Session[] tl = nodeTL.TimeLine;

            Assert.AreEqual(tl[0].StartTime, nodeTL.getFirstTime(), "getFirstTime should return the earliest time in the timeline.");
        }


        [TestMethod]
        [ExpectedException(typeof(System.NullReferenceException))]
        public void getFinalTimeTest1()
        {
            NodeTimeline nodeTL = new NodeTimeline(10000, 200);
            nodeTL.getFinalTime();
        }


        [TestMethod]
        public void getFinalTimeTest2()
        {
            ContinuousUniform upD = new ContinuousUniform(0.0, 1.0);
            ContinuousUniform downD = new ContinuousUniform(0.0, 1.0);
            NodeTimeline nodeTL = new NodeTimeline(10000, 200);
            nodeTL.generateTimeline(upD, downD);
            Session[] tl = nodeTL.TimeLine;

            Assert.AreEqual(tl[tl.Length - 1].EndTime, nodeTL.getFinalTime(), "getFinalTime should return the last stop time in the timeline.");
        }


        [TestMethod]
        [ExpectedException(typeof(System.NullReferenceException))]
        public void timeIsLiveTest1()
        {
            NodeTimeline nodeTL = new NodeTimeline(10000, 200);
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
            ContinuousUniform upD = new ContinuousUniform(0.0, 1.0);
            ContinuousUniform downD = new ContinuousUniform(0.0, 1.0);
            NodeTimeline nodeTL = new NodeTimeline(100, 100.0);
            nodeTL.generateTimeline(upD, downD);
            Session[] tl = nodeTL.TimeLine;

            double time_t = tl[50].StartTime + (tl[50].getDurationLive() / 2.0);

            Assert.IsTrue(nodeTL.timeIsLive(time_t), "timeIsLive is inaccurately reporting a live time as down");
        }


        [TestMethod]
        public void timeIsLiveTest4()
        {
            ContinuousUniform upD = new ContinuousUniform(0.0, 1.0);
            ContinuousUniform downD = new ContinuousUniform(0.0, 1.0);
            NodeTimeline nodeTL = new NodeTimeline(100, 100.0);
            nodeTL.generateTimeline(upD, downD);
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
            ContinuousUniform upD = new ContinuousUniform(0.0, 1.0);
            ContinuousUniform downD = new ContinuousUniform(0.0, 1.0);
            NodeTimeline nodeTL = new NodeTimeline(1000, 200);
            nodeTL.generateTimeline(upD, downD);
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
            ContinuousUniform upD = new ContinuousUniform(0.0, 1.0);
            ContinuousUniform downD = new ContinuousUniform(0.0, 1.0);
            NodeTimeline nodeTL = new NodeTimeline(1000, 200);
            nodeTL.generateTimeline(upD, downD);
            Session[] tl = nodeTL.TimeLine;

            double avg = 0;

            for (int i = 1; i < tl.Length; i++)
                avg += (tl[i].StartTime - tl[i - 1].EndTime);

            avg = avg / (tl.Length - 1);

            Assert.AreEqual(avg, nodeTL.averageDownTime(), "averageDownTime incorrectly calculated.");
        }
    }
}
