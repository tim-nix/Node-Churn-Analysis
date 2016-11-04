﻿using System;
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
            simGH();
        }

        public static void simClique()
        {
            double baseTime = 200.0;
            double endTime = 300.0;
            double delta = 0.1;
            int numSessions = 100;
            for (int numNodes = 100; numNodes < 1000; numNodes += 100)
            {
                Network network = new Network(CommonGraphs.Clique(numNodes));

                NodeTimeline[] nodeSessions = new NodeTimeline[numNodes];

                for (int i = 0; i < numNodes; i++)
                {
                    nodeSessions[i] = new NodeTimeline(numSessions, baseTime);
                    nodeSessions[i].generateTimeline();
                }

                bool[] status = new bool[numNodes];

                double time = baseTime;
                double connectionCount = 0.0;
                double iterations = 0.0;
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

                Console.WriteLine("Clique with {0} nodes is connected {1}% of the time.", numNodes, (connectionCount / iterations) * 100);
            }
        }

        public static void simGH()
        {
            double baseTime = 200.0;
            double delta = 0.1;
            int numSessions = 200;
            int numNodes = 0;
            for (int numCliques = 3; numCliques < 50; numCliques++)
            {
                numNodes = numCliques * (numCliques - 1);
                Network network = new Network(CommonGraphs.GuntherHartnell(numNodes));

                NetworkChurn netChurn = new NetworkChurn(numNodes);
                netChurn.generateChurn(numSessions, baseTime);

                double time = baseTime;
                double connectionCount = 0.0;
                double iterations = 0.0;
                double endTime = netChurn.getQuickestFinalTime();
                double percentLive = 0;
                while (time < endTime)
                {
                    bool[] status = netChurn.getStatusAtTime(time);

                    double numLive = 0;
                    for (int i = 0; i < status.Length; i++)
                    {
                        if (status[i])
                            numLive += 1.0;
                    }

                    percentLive += numLive / Convert.ToDouble(status.Length);

                    network.updateStatus(status);
                    if (network.isCurrentNetworkConnected())
                        connectionCount += 1.0;

                    iterations += 1.0;
                    time += delta;
                }

                Console.WriteLine("Gunther-Hartnell topology with {0} nodes is connected {1:N2}% of the time.", numNodes, (connectionCount / iterations) * 100);
                Console.WriteLine("Each node is live {0:N2}% of the time", (percentLive / iterations) * 100.0);
            }
        }

        public static void simGnp()
        {
            double p = 0.3;
            double baseTime = 200.0;
            double endTime = 300.0;
            double delta = 0.1;
            int numSessions = 100;
            for (int numNodes = 10; numNodes < 100; numNodes++)
            {
                Network network = new Network(CommonGraphs.Gnp(numNodes, p));

                NodeTimeline[] nodeSessions = new NodeTimeline[numNodes];

                for (int i = 0; i < numNodes; i++)
                {
                    nodeSessions[i] = new NodeTimeline(numSessions, baseTime);
                    nodeSessions[i].generateTimeline();
                }

                bool[] status = new bool[numNodes];

                double time = baseTime;
                double connectionCount = 0.0;
                double iterations = 0.0;
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

                Console.WriteLine("Gnp graph family with {0} nodes and p = {1} is connected {2}% of the time.", numNodes, p, (connectionCount / iterations) * 100);
            }
        }
    }
}
