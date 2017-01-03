using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkSimulation;

namespace NetworkSimulationUnitTests
{
    [TestClass]
    public class SessionTests
    {
        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void ConstructorTest1()
        {
            double s = 4.5;
            double e = 4.4;

            Session session = new Session(s, e);
        }


        [TestMethod]
        public void isLiveTest1()
        {
            double s = 4.4;
            double e = 4.5;

            Session session = new Session(s, e);

            Assert.IsTrue(session.isLive(4.45), "Given session should be live.");
        }


        [TestMethod]
        public void isLiveTest2()
        {
            double s = 4.4;
            double e = 4.5;

            Session session = new Session(s, e);

            Assert.IsFalse(session.isLive(4.6), "Given session should not be live.");
        }


        [TestMethod]
        public void getDurationLiveTest1()
        {
            double s = 4.4;
            double e = 4.5;

            Session session = new Session(s, e);

            Assert.AreEqual(e - s, session.getDurationLive(), "Given session period should be " + (e - s) + ".");
        }
    }
}
