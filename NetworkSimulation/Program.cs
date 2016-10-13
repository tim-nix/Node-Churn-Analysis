using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    class Program
    {
        static void Main(string[] args)
        {
            double baseTime = 200.0;
            double endTime  = 210.0;
            double delta    = 1.0;
            int numNodes    = 5;
            int numSessions = 100;
            Network network = new Network(CommonGraphs.PetersonGraph);

            NodeTimeline[] nodeSessions = new NodeTimeline[numNodes];

            for (int i = 0; i < numNodes; i++)
            {
                nodeSessions[i] = new NodeTimeline(numSessions, baseTime);
                nodeSessions[i].generateTimeline();
            }

            bool[] status = new bool[numNodes];

            double time             = baseTime;
            double connectionCount  = 0.0;
            double iterations       = 0.0;
            while (time < endTime)
            {
                for (int i = 0; i < numNodes; i++)
                    status[i] = nodeSessions[i].timeIsLive(time);

                network.updateStatus(status);
                if (network.isCurrentNetworkConnected())
                    connectionCount += 1.0;

                iterations += 1.0;
                time += delta;
            }

            Console.WriteLine("Network is connected {0}% of the time.", (connectionCount / iterations) * 100);
        }
    }
}
