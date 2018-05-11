using System;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    class Simulations
    {
        private int minOrder = 0;           // smallest graph order to simulate
        private int maxOrder = 0;           // largest graph order to simulate
        private int nodeDelta = 0;          // rate of change in graph order
        private double baseTime = 0.0;      // earliest time to track churn
        private double timeDelta = 0.0;     // time increment
        private int numSessions = 0;        // number of sessions per node to track
        private Distribution upDistro;      // distribution for drawing session up times
        private Distribution downDistro;    // distribution for drawing session down times

        private const int _numlocks = 5;
        private static readonly object[] _lock = new object[_numlocks];  // lock for shared variables across threads


        public Simulations(int minN = 20,
                           int maxN = 200,
                           int nDelta = 100,
                           double startTime = 200.0,
                           double tDelta = 0.1,
                           int sessionsPerNode = 100)
        {
            setNodeRange(minN, maxN, nDelta);
            setTimeRange(startTime, tDelta);
            setNumberOfSessions(sessionsPerNode);
            for (int i = 0; i < _numlocks; i++)
            {
                _lock[i] = new object();
            }
        }

        public void setNodeRange(int minN, int maxN, int nDelta)
        {
            if ((minN <= maxN) && (minN + nDelta <= maxN) && (minN > 0) && (nDelta > 0))
            {
                minOrder = minN;
                maxOrder = maxN;
                nodeDelta = nDelta;
            }
            else
                throw new ArgumentException("Error: Faulty arguments for node range!");
        }


        public void setTimeRange(double startTime, double tDelta)
        {
            if ((startTime >= 0) && (tDelta > 0))
            {
                baseTime = startTime;
                timeDelta = tDelta;
            }
            else
                throw new ArgumentException("Error: Faulty arguments for time range!");
        }


        public void setNumberOfSessions(int sessionsPerNode)
        {
            if (sessionsPerNode > 0)
                numSessions = sessionsPerNode;
            else
                throw new ArgumentException("Error: Faulty arguments for number of sessions!");
        }


        public void setUpDistro(Distribution upD, Distribution downD)
        {
            upDistro = upD;
            downDistro = downD;
        }


        public void simPath()
        {
            if (upDistro == null)
                throw new NullReferenceException("Error: Must set up-time and down-time distributions!");

            int numSims = 10000;

            int[] nValues = new int[maxOrder - minOrder];

            double[] pValues = new double[maxOrder - minOrder];
            double[] connectivity = new double[maxOrder - minOrder];
            double[] liveTime = new double[maxOrder - minOrder];
            double[] avgMsgDelays = new double[maxOrder - minOrder];

            double percentLive = 0.0;

            int index = 0;
            int numNodes = 0;
            int totalSims = 0;

            System.IO.File.WriteAllText("graph_sizes_path.txt", "");
            System.IO.File.WriteAllText("avg_connectivity_path.txt", "");
            System.IO.File.WriteAllText("avg_msg_delays_path.txt", "");
            System.IO.File.WriteAllText("avg_up_time_path.txt", "");

            for (numNodes = minOrder; numNodes < maxOrder; numNodes++)
            {
                nValues[index] = numNodes;

                connectivity[index] = 0.0;
                avgMsgDelays[index] = 0.0;

                Console.WriteLine("Number of nodes: " + numNodes);
                totalSims = 0;
                for (int sim = 0; sim < numSims; sim++)
                {
                    totalSims++;
                    //Console.WriteLine("Simulation " + (sim + 1));
                    Network network = new Network(CommonGraphs.Path(numNodes));

                    NetworkChurn netChurn = new NetworkChurn(numNodes);
                    netChurn.generateChurn(numSessions, baseTime, upDistro, downDistro);


                    double time = baseTime + 25.0;
                    double delay = 0.0;
                    percentLive = 0.0;

                    //Console.WriteLine("time = " + time);
                    bool[] status = netChurn.getStatusAtTime(time);

                    double numLive = 0;
                    for (int i = 0; i < status.Length; i++)
                    {
                        if (status[i])
                            numLive += 1.0;
                    }

                    percentLive += (numLive / Convert.ToDouble(status.Length)) * 100.0;
                    liveTime[index] += percentLive;

                    network.updateStatus(status);
                    if (network.isCurrentNetworkConnected())
                        connectivity[index] += 1.0;

                    Message msg = new Message(network, netChurn, time);
                    try
                    {
                        delay = msg.getPathMessageDelay();
                        avgMsgDelays[index] += delay;
                        System.IO.File.AppendAllText("msg_delays_path_" + numNodes.ToString() + ".txt", delay.ToString() + Environment.NewLine);
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine("Simulation " + sim + ": " + e);
                        sim--;
                    }
                }

                connectivity[index] = (connectivity[index] / Convert.ToDouble(totalSims)) * 100.0;
                avgMsgDelays[index] = avgMsgDelays[index] / Convert.ToDouble(numSims);
                liveTime[index] = liveTime[index] / Convert.ToDouble(totalSims);

                Console.WriteLine("Path graph family with {0} nodes is connected {1:N2}% of the time.", nValues[index], connectivity[index]);
                Console.WriteLine("The average message delay between two end nodes is {0:N4}.", avgMsgDelays[index]);
                Console.WriteLine("On average, {0:N2}% nodes are live at any given time", liveTime[index]);
                Console.WriteLine();

                System.IO.File.AppendAllText("graph_sizes_path.txt", nValues[index].ToString() + Environment.NewLine);
                System.IO.File.AppendAllText("avg_connectivity_path.txt", connectivity[index].ToString() + Environment.NewLine);
                System.IO.File.AppendAllText("avg_msg_delays_path.txt", avgMsgDelays[index].ToString() + Environment.NewLine);
                System.IO.File.AppendAllText("avg_up_time_path.txt", liveTime[index].ToString() + Environment.NewLine);

                index++;
            }
        }


        public void simCycle()
        {
            if (upDistro == null)
                throw new NullReferenceException("Error: Must set up-time and down-time distributions!");

            int numSims = 10000;

            int[] nValues = new int[maxOrder - minOrder];

            double[] pValues = new double[maxOrder - minOrder];
            double[] connectivity = new double[maxOrder - minOrder];
            double[] liveTime = new double[maxOrder - minOrder];
            double[] avgMsgDelays = new double[maxOrder - minOrder];

            double percentLive = 0.0;

            int index = 0;
            int numNodes = 0;
            int totalSims = 0;

            System.IO.File.WriteAllText("graph_sizes_cycle.txt", "");
            System.IO.File.WriteAllText("avg_connectivity_cycle.txt", "");
            System.IO.File.WriteAllText("avg_msg_delays_cycle.txt", "");
            System.IO.File.WriteAllText("avg_up_time_cycle.txt", "");

            for (numNodes = minOrder; numNodes < maxOrder; numNodes++)
            {

                nValues[index] = numNodes;

                connectivity[index] = 0.0;
                avgMsgDelays[index] = 0.0;

                Console.WriteLine("Number of nodes: " + numNodes);
                totalSims = 0;
                for (int sim = 0; sim < numSims; sim++)
                {
                    totalSims++;
                    //Console.WriteLine("Simulation " + (sim + 1));
                    Network network = new Network(CommonGraphs.Cycle(numNodes));

                    NetworkChurn netChurn = new NetworkChurn(numNodes);
                    netChurn.generateChurn(numSessions, baseTime, upDistro, downDistro);


                    double time = baseTime + 25.0;
                    double delay = 0.0;
                    percentLive = 0.0;

                    //Console.WriteLine("time = " + time);
                    bool[] status = netChurn.getStatusAtTime(time);

                    double numLive = 0;
                    for (int i = 0; i < status.Length; i++)
                    {
                        if (status[i])
                            numLive += 1.0;
                    }

                    percentLive += (numLive / Convert.ToDouble(status.Length)) * 100.0;
                    liveTime[index] += percentLive;

                    network.updateStatus(status);
                    if (network.isCurrentNetworkConnected())
                            connectivity[index] += 1.0;

                    Message msg = new Message(network, netChurn, time);
                    try
                    {
                        delay = msg.getCycleMessageDelay();
                        avgMsgDelays[index] += delay;
                        System.IO.File.AppendAllText("msg_delays_cycle_" + numNodes.ToString() + ".txt", delay.ToString() + Environment.NewLine);
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine("Simulation " + sim + ": " + e);
                        sim--;
                    }
                }

                connectivity[index] = (connectivity[index] / Convert.ToDouble(totalSims)) * 100.0;
                avgMsgDelays[index] = avgMsgDelays[index] / Convert.ToDouble(numSims);
                liveTime[index] = liveTime[index] / Convert.ToDouble(totalSims);

                Console.WriteLine("Cycle graph family with {0} nodes is connected {1:N2}% of the time.", nValues[index], connectivity[index]);
                Console.WriteLine("The average message delay between two opposite nodes is {0:N4}.", avgMsgDelays[index]);
                Console.WriteLine("On average, {0:N2}% nodes are live at any given time", liveTime[index]);
                Console.WriteLine();

                System.IO.File.AppendAllText("graph_sizes_cycle.txt", nValues[index].ToString() + Environment.NewLine);
                System.IO.File.AppendAllText("avg_connectivity_cycle.txt", connectivity[index].ToString() + Environment.NewLine);
                System.IO.File.AppendAllText("avg_msg_delays_cycle.txt", avgMsgDelays[index].ToString() + Environment.NewLine);
                System.IO.File.AppendAllText("avg_up_time_cycle.txt", liveTime[index].ToString() + Environment.NewLine);

                index++;
            }
        }


        public void simClique()
        {
            if (upDistro == null)
                throw new NullReferenceException("Error: Must set up-time and down-time distributions!");

            int numSims = 500;

            int cliqueMin = 5;
            while (cliqueMin * (cliqueMin - 1) < minOrder)
                cliqueMin++;


            int cliqueMax = cliqueMin;
            while (cliqueMax * (cliqueMax - 1) < maxOrder)
                cliqueMax++;

            int[] nValues = new int[cliqueMax - cliqueMin];

            double[] pValues = new double[cliqueMax - cliqueMin];
            double[] connectivity = new double[cliqueMax - cliqueMin];
            double[] liveTime = new double[cliqueMax - cliqueMin];
            double[] avgMsgDelays = new double[cliqueMax - cliqueMin];

            double percentLive = 0.0;
            double iterations = 0.0;

            int index = 0;
            int numNodes = 0;

            //Console.WriteLine("The smallest number of cliques: " + cliqueMin);
            //Console.WriteLine("The largest number of cliques: " + cliqueMax);
            for (int numCliques = cliqueMin; numCliques < cliqueMax; numCliques++)
            {
                numNodes = numCliques * (numCliques - 1);
                nValues[index] = numNodes;
                connectivity[index] = 0.0;
                avgMsgDelays[index] = 0.0;

                //Console.WriteLine("Number or nodes: " + numNodes);
                for (int sim = 0; sim < numSims; sim++)
                {
                    //Console.WriteLine("Simulation " + (sim + 1));
                    Network network = new Network(CommonGraphs.Clique(numNodes));

                    NetworkChurn netChurn = new NetworkChurn(numNodes);
                    netChurn.generateChurn(numSessions, baseTime, upDistro, downDistro);


                    double time = baseTime;
                    double connectionCount = 0.0;
                    double avgDelay = 0.0;
                    percentLive = 0.0;
                    iterations = 0;

                    //double endTime = netChurn.getEarliestFinalTime();
                    //double endTime = time + timeDelta;
                    double endTime = baseTime + 50.0;
                    while (time < endTime)
                    {
                        //Console.WriteLine("time = " + time);
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

                        Message msg = new Message(network, netChurn, time);
                        avgDelay += msg.getMessageDelay();

                        iterations += 1.0;
                        time += timeDelta;
                    }

                    connectivity[index] += (connectionCount / iterations) * 100.0;
                    avgMsgDelays[index] += avgDelay / iterations;
                    //Console.WriteLine("On this iteration, a path existed " + (pathCount / iterations) * 100.0 + " percent of the time.");
                }

                connectivity[index] = connectivity[index] / Convert.ToDouble(numSims);
                avgMsgDelays[index] = avgMsgDelays[index] / Convert.ToDouble(numSims);
                liveTime[index] = (percentLive / iterations) * 100.0;
                Console.WriteLine("GH graph family with {0} nodes is connected {1:N2}% of the time.", nValues[index], connectivity[index]);
                Console.WriteLine("The average message delay between two random nodes is {0:N3}.", avgMsgDelays[index]);
                Console.WriteLine("On average, {0:N2}% nodes are live at any given time", liveTime[index]);
                Console.WriteLine();

                index++;
            }

            System.IO.File.WriteAllLines("c:/Temp_For_Grading/graph_sizes_clique.txt", nValues.Select(d => d.ToString()).ToArray());
            System.IO.File.WriteAllLines("c:/Temp_For_Grading/avg_connectivity_clique.txt", connectivity.Select(d => d.ToString()).ToArray());
            System.IO.File.WriteAllLines("c:/Temp_For_Grading/avg_msg_delays_clique.txt", avgMsgDelays.Select(d => d.ToString()).ToArray());
            System.IO.File.WriteAllLines("c:/Temp_For_Grading/avg_up_time_clique.txt", liveTime.Select(d => d.ToString()).ToArray());
        }

        public void simGH()
        {
            if (upDistro == null)
                throw new NullReferenceException("Error: Must set up-time and down-time distributions!");

            int numSims = 10000;

            int cliqueMin = 1;
            while (cliqueMin * (cliqueMin - 1) < minOrder)
                cliqueMin++;


            int cliqueMax = cliqueMin;
            while (cliqueMax * (cliqueMax - 1) < maxOrder)
                cliqueMax++;

            int[] nValues = new int[cliqueMax - cliqueMin];

            double[] pValues = new double[cliqueMax - cliqueMin];
            double[] connectivity = new double[cliqueMax - cliqueMin];
            double[] liveTime = new double[cliqueMax - cliqueMin];
            double[] avgMsgDelays = new double[cliqueMax - cliqueMin];

            double percentLive = 0.0;

            int index = 0;
            int numNodes = 0;
            int totalSims = 0;

            //Console.WriteLine("The smallest number of cliques: " + cliqueMin);
            //Console.WriteLine("The largest number of cliques: " + cliqueMax);

            System.IO.File.WriteAllText("graph_sizes_gh.txt", "");
            System.IO.File.WriteAllText("avg_connectivity_gh.txt", "");
            System.IO.File.WriteAllText("avg_msg_delays_gh.txt", "");
            System.IO.File.WriteAllText("avg_up_time_gh.txt", "");

            for (int numCliques = cliqueMin; numCliques < cliqueMax; numCliques++)
            {
                numNodes = numCliques * (numCliques - 1);

                nValues[index] = numNodes;

                connectivity[index] = 0.0;
                avgMsgDelays[index] = 0.0;

                Console.WriteLine("Number of nodes: " + numNodes);
                totalSims = 0;
                for (int sim = 0; sim < numSims; sim++)
                {
                    totalSims++;
                    //Console.WriteLine("Simulation " + (sim + 1));
                    Network network = new Network(CommonGraphs.GuntherHartnell(numNodes));

                    NetworkChurn netChurn = new NetworkChurn(numNodes);
                    netChurn.generateChurn(numSessions, baseTime, upDistro, downDistro);


                    double time = baseTime + 25.0;
                    double avgDelay = 0.0;
                    percentLive = 0.0;

                    //Console.WriteLine("time = " + time);
                    bool[] status = netChurn.getStatusAtTime(time);

                    double numLive = 0;
                    for (int i = 0; i < status.Length; i++)
                    {
                        if (status[i])
                            numLive += 1.0;
                    }

                    percentLive += (numLive / Convert.ToDouble(status.Length)) * 100.0;
                    liveTime[index] += percentLive;

                    network.updateStatus(status);
                    if (network.isCurrentNetworkConnected())
                        connectivity[index] += 1.0;

                    Message msg = new Message(network, netChurn, time);
                    try
                    {
                        avgDelay += msg.getMessageDelay();
                        avgMsgDelays[index] += avgDelay;
                        System.IO.File.AppendAllText("msg_delays_gh_" + numNodes.ToString() + ".txt", avgDelay.ToString() + Environment.NewLine);
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine(e);
                        sim--;
                    }
                }

                connectivity[index] = (connectivity[index] / Convert.ToDouble(totalSims)) * 100.0;
                avgMsgDelays[index] = avgMsgDelays[index] / Convert.ToDouble(numSims);
                liveTime[index] = liveTime[index] / Convert.ToDouble(totalSims);

                Console.WriteLine("GH graph family with {0} nodes is connected {1:N2}% of the time.", nValues[index], connectivity[index]);
                Console.WriteLine("The average message delay between two random nodes is {0:N4}.", avgMsgDelays[index]);
                Console.WriteLine("On average, {0:N2}% nodes are live at any given time", liveTime[index]);
                Console.WriteLine();

                System.IO.File.AppendAllText("graph_sizes_gh.txt", nValues[index].ToString() + Environment.NewLine);
                System.IO.File.AppendAllText("avg_connectivity_gh.txt", connectivity[index].ToString() + Environment.NewLine);
                System.IO.File.AppendAllText("avg_msg_delays_gh.txt", avgMsgDelays[index].ToString() + Environment.NewLine);
                System.IO.File.AppendAllText("avg_up_time_gh.txt", liveTime[index].ToString() + Environment.NewLine);

                index++;
            }
        }

        public void simGnp(double p)
        {
            if (upDistro == null)
                throw new NullReferenceException("Error: Must set up-time and down-time distributions!");

            int numSims = 500;

            int[] nValues = new int[maxOrder - minOrder];

            double[] pValues = new double[maxOrder - minOrder];
            double[] connectivity = new double[maxOrder - minOrder];
            double[] liveTime = new double[maxOrder - minOrder];
            double[] avgMsgDelays = new double[maxOrder - minOrder];

            double percentLive = 0.0;
            double iterations = 0.0;

            int index = 0;
            for (int numNodes = minOrder; numNodes < maxOrder; numNodes += nodeDelta)
            {
                nValues[index] = numNodes;
                connectivity[index] = 0.0;
                avgMsgDelays[index] = 0.0;

                //Console.WriteLine("Number or nodes: " + numNodes);
                for (int sim = 0; sim < numSims; sim++)
                {
                    //Console.WriteLine("Simulation " + (sim + 1));
                    Network network = new Network(CommonGraphs.Gnp(numNodes, p));

                    // Gnp graphs are not necessarily connected.  We only want
                    // topologies where the full network is connected.
                    while (!network.isFullNetworkConnected())
                        network = new Network(CommonGraphs.Gnp(numNodes, p));

                    NetworkChurn netChurn = new NetworkChurn(numNodes);
                    netChurn.generateChurn(numSessions, baseTime, upDistro, downDistro);


                    double time = baseTime;
                    double connectionCount = 0.0;
                    double avgDelay = 0.0;
                    percentLive = 0.0;
                    iterations = 0;

                    //double endTime = netChurn.getEarliestFinalTime();
                    //double endTime = time + timeDelta;
                    double endTime = baseTime + 50.0;
                    while (time < endTime)
                    {
                        //Console.WriteLine("time = " + time);
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

                        Message msg = new Message(network, netChurn, time);
                        avgDelay += msg.getMessageDelay();

                        iterations += 1.0;
                        time += timeDelta;
                    }

                    connectivity[index] += (connectionCount / iterations) * 100.0;
                    avgMsgDelays[index] += avgDelay / iterations;
                    //Console.WriteLine("On this iteration, a path existed " + (pathCount / iterations) * 100.0 + " percent of the time.");
                }

                connectivity[index] = connectivity[index] / Convert.ToDouble(numSims);
                avgMsgDelays[index] = avgMsgDelays[index] / Convert.ToDouble(numSims);
                liveTime[index] = (percentLive / iterations) * 100.0;
                Console.WriteLine("GH graph family with {0} nodes is connected {1:N2}% of the time.", nValues[index], connectivity[index]);
                Console.WriteLine("The average message delay between two random nodes is {0:N3}.", avgMsgDelays[index]);
                Console.WriteLine("On average, {0:N2}% nodes are live at any given time", liveTime[index]);
                Console.WriteLine();

                System.IO.File.AppendAllLines("c:/Temp_For_Grading/graph_sizes_gnp.txt", nValues.Select(d => d.ToString()).ToArray());
                System.IO.File.WriteAllLines("c:/Temp_For_Grading/avg_connectivity_gnp.txt", connectivity.Select(d => d.ToString()).ToArray());
                System.IO.File.WriteAllLines("c:/Temp_For_Grading/avg_msg_delays_gnp.txt", avgMsgDelays.Select(d => d.ToString()).ToArray());
                System.IO.File.WriteAllLines("c:/Temp_For_Grading/avg_up_time_gnp.txt", liveTime.Select(d => d.ToString()).ToArray());

                index++;
            }
        }

        /// <summary>
        /// This simulation is set up for direct comparison of Gnp to Gunther-Hartnell.
        /// </summary>
        public void simGnp2()
        {
            if (upDistro == null)
                throw new NullReferenceException("Error: Must set up-time and down-time distributions!");

            int numSims = 500;

            int cliqueMin = 5;
            while (cliqueMin * (cliqueMin - 1) < minOrder)
                cliqueMin++;


            int cliqueMax = cliqueMin;
            while (cliqueMax * (cliqueMax - 1) < maxOrder)
                cliqueMax++;

            int[] nValues = new int[cliqueMax - cliqueMin];

            double[] pValues = new double[cliqueMax - cliqueMin];
            double[] connectivity = new double[cliqueMax - cliqueMin];
            double[] liveTime = new double[cliqueMax - cliqueMin];
            double[] avgMsgDelays = new double[cliqueMax - cliqueMin];

            double percentLive = 0.0;
            double iterations = 0.0;

            int index = 0;
            int numNodes = 0;
            int numEdges = 0;
            double p;
            //Console.WriteLine("The smallest number of cliques: " + cliqueMin);
            //Console.WriteLine("The largest number of cliques: " + cliqueMax);

            System.IO.File.WriteAllText("graph_sizes_gnp.txt", "");
            System.IO.File.WriteAllText("avg_connectivity_gnp.txt", "");
            System.IO.File.WriteAllText("avg_msg_delays_gnp.txt", "");
            System.IO.File.WriteAllText("avg_up_time_gnp.txt", "");

            for (int numCliques = cliqueMin; numCliques < cliqueMax; numCliques++)
            {
                numNodes = numCliques * (numCliques - 1);
                numEdges = numNodes * (numCliques - 1) / 2;
                System.IO.File.WriteAllText("connectivity_gnp_" + numNodes.ToString() + ".txt", "");
                System.IO.File.WriteAllText("msg_delays_gnp_" + numNodes.ToString() + ".txt", "");
                p = Convert.ToDouble(numEdges) / (Convert.ToDouble(numNodes * (numNodes - 1)) / 2.0);
                nValues[index] = numNodes;
                connectivity[index] = 0.0;
                avgMsgDelays[index] = 0.0;

                //Console.WriteLine("Number or nodes: " + numNodes);
                Console.WriteLine("p = " + p);

                for (int sim = 0; sim < numSims; sim++)
                {
                    //Console.WriteLine("Simulation " + (sim + 1));
                    Network network = new Network(CommonGraphs.Gnp(numNodes, p));

                    // Gnp graphs are not necessarily connected.  We only want
                    // topologies where the full network is connected.
                    while (!network.isFullNetworkConnected())
                        network = new Network(CommonGraphs.Gnp(numNodes, p));

                    NetworkChurn netChurn = new NetworkChurn(numNodes);
                    netChurn.generateChurn(numSessions, baseTime, upDistro, downDistro);


                    double time = baseTime;
                    double connectionCount = 0.0;
                    double avgDelay = 0.0;
                    percentLive = 0.0;
                    iterations = 0.0;

                    //double endTime = netChurn.getEarliestFinalTime();
                    //double endTime = time + timeDelta;
                    double endTime = baseTime + 50.0;
                    while (time < endTime)
                    {
                        //Console.WriteLine("time = " + time);
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

                        Message msg = new Message(network, netChurn, time);
                        avgDelay += msg.getMessageDelay();

                        iterations += 1.0;
                        time += timeDelta;
                    }

                    connectivity[index] += (connectionCount / iterations) * 100.0;
                    avgMsgDelays[index] += avgDelay / iterations;
                    liveTime[index] += (percentLive / iterations) * 100.0;
                    //Console.WriteLine("On this iteration, a path existed " + (pathCount / iterations) * 100.0 + " percent of the time.");

                    System.IO.File.AppendAllText("connectivity_gnp_" + numNodes.ToString() + ".txt", (connectionCount / iterations).ToString() + Environment.NewLine);
                    System.IO.File.AppendAllText("msg_delays_gnp_" + numNodes.ToString() + ".txt", (avgDelay / iterations).ToString() + Environment.NewLine);
                }

                connectivity[index] = connectivity[index] / Convert.ToDouble(numSims);
                avgMsgDelays[index] = avgMsgDelays[index] / Convert.ToDouble(numSims);
                liveTime[index] = liveTime[index] / Convert.ToDouble(numSims);
                Console.WriteLine("GH graph family with {0} nodes is connected {1:N2}% of the time.", nValues[index], connectivity[index]);
                Console.WriteLine("The average message delay between two random nodes is {0:N3}.", avgMsgDelays[index]);
                Console.WriteLine("On average, {0:N2}% nodes are live at any given time", liveTime[index]);
                Console.WriteLine();

                System.IO.File.AppendAllText("graph_sizes_gnp.txt", nValues[index].ToString() + Environment.NewLine);
                System.IO.File.AppendAllText("avg_connectivity_gnp.txt", connectivity[index].ToString() + Environment.NewLine);
                System.IO.File.AppendAllText("avg_msg_delays_gnp.txt", avgMsgDelays[index].ToString() + Environment.NewLine);
                System.IO.File.AppendAllText("avg_up_time_gnp.txt", liveTime[index].ToString());

                index++;
            }
        }


        /// <summary>
        /// This simulation is set up for direct comparison of Gnp to Gunther-Hartnell.
        /// It also only samples each measured parameter once per generated graph.
        /// </summary>
        public void simGnp3()
        {
            if (upDistro == null)
                throw new NullReferenceException("Error: Must set up-time and down-time distributions!");

            int numSims = 10000;

            int cliqueMin = 5;
            while (cliqueMin * (cliqueMin - 1) < minOrder)
                cliqueMin++;


            int cliqueMax = cliqueMin;
            while (cliqueMax * (cliqueMax - 1) < maxOrder)
                cliqueMax++;

            int[] nValues = new int[cliqueMax - cliqueMin];

            double[] pValues = new double[cliqueMax - cliqueMin];
            double[] connectivity = new double[cliqueMax - cliqueMin];
            double[] liveTime = new double[cliqueMax - cliqueMin];
            double[] avgMsgDelays = new double[cliqueMax - cliqueMin];

            double percentLive = 0.0;

            int index = 0;
            int numNodes = 0;
            int numEdges = 0;
            int totalSims = 0;
            double p;
            //Console.WriteLine("The smallest number of cliques: " + cliqueMin);
            //Console.WriteLine("The largest number of cliques: " + cliqueMax);

            System.IO.File.WriteAllText("graph_sizes_gnp.txt", "");
            System.IO.File.WriteAllText("avg_connectivity_gnp.txt", "");
            System.IO.File.WriteAllText("avg_msg_delays_gnp.txt", "");
            System.IO.File.WriteAllText("avg_up_time_gnp.txt", "");

            for (int numCliques = cliqueMin; numCliques < cliqueMax; numCliques++)
            {
                numNodes = numCliques * (numCliques - 1);
                numEdges = numNodes * (numCliques - 1) / 2;
                System.IO.File.WriteAllText("msg_delays_gnp_" + numNodes.ToString() + ".txt", "");
                p = Convert.ToDouble(numEdges) / (Convert.ToDouble(numNodes * (numNodes - 1)) / 2.0);
                nValues[index] = numNodes;
                connectivity[index] = 0.0;
                avgMsgDelays[index] = 0.0;

                //Console.WriteLine("Number or nodes: " + numNodes);
                Console.WriteLine("p = " + p);
                totalSims = 0;
                for (int sim = 0; sim < numSims; sim++)
                {
                    totalSims++;
                    //Console.WriteLine("Simulation " + (sim + 1));
                    Network network = new Network(CommonGraphs.Gnp(numNodes, p));

                    // Gnp graphs are not necessarily connected.  We only want
                    // topologies where the full network is connected.
                    while (!network.isFullNetworkConnected())
                        network = new Network(CommonGraphs.Gnp(numNodes, p));

                    NetworkChurn netChurn = new NetworkChurn(numNodes);
                    netChurn.generateChurn(numSessions, baseTime, upDistro, downDistro);


                    double time = baseTime + 25.0;
                    double delay = 0.0;
                    percentLive = 0.0;

                    //Console.WriteLine("time = " + time);
                    bool[] status = netChurn.getStatusAtTime(time);

                    double numLive = 0;
                    for (int i = 0; i < status.Length; i++)
                    {
                        if (status[i])
                            numLive += 1.0;
                    }

                    percentLive += (numLive / Convert.ToDouble(status.Length)) * 100.0;
                    liveTime[index] += percentLive;

                    network.updateStatus(status);
                    if (network.isCurrentNetworkConnected())
                        connectivity[index] += 1.0;

                    Message msg = new Message(network, netChurn, time);
                    try
                    {
                        delay = msg.getMessageDelay();
                        avgMsgDelays[index] += delay;
                        System.IO.File.AppendAllText("msg_delays_gnp_" + numNodes.ToString() + ".txt", delay.ToString() + Environment.NewLine);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Simulation " + sim + ": " + e);
                        sim--;
                    }
                }

                connectivity[index] = (connectivity[index] / Convert.ToDouble(totalSims)) * 100.0;
                avgMsgDelays[index] = avgMsgDelays[index] / Convert.ToDouble(numSims);
                liveTime[index] = liveTime[index] / Convert.ToDouble(totalSims);
                Console.WriteLine("GH graph family with {0} nodes is connected {1:N2}% of the time.", nValues[index], connectivity[index]);
                Console.WriteLine("The average message delay between two random nodes is {0:N4}.", avgMsgDelays[index]);
                Console.WriteLine("On average, {0:N2}% nodes are live at any given time", liveTime[index]);
                Console.WriteLine();

                System.IO.File.AppendAllText("graph_sizes_gnp.txt", nValues[index].ToString() + Environment.NewLine);
                System.IO.File.AppendAllText("avg_connectivity_gnp.txt", connectivity[index].ToString() + Environment.NewLine);
                System.IO.File.AppendAllText("avg_msg_delays_gnp.txt", avgMsgDelays[index].ToString() + Environment.NewLine);
                System.IO.File.AppendAllText("avg_up_time_gnp.txt", liveTime[index].ToString() + Environment.NewLine);

                index++;
            }
        }

        /// <summary>
        /// This simulation is set up for direct comparison of Barabasi-Albert scale-free topologies to Gunther-Hartnell.  
        /// It also only samples each measured parameter once per generated graph.
        /// </summary>
        public void simBA3()
        {
            if (upDistro == null)
                throw new NullReferenceException("Error: Must set up-time and down-time distributions!");

            int numSims = 10000;

            int cliqueMin = 5;
            while (cliqueMin * (cliqueMin - 1) < minOrder)
                cliqueMin++;


            int cliqueMax = cliqueMin;
            while (cliqueMax * (cliqueMax - 1) < maxOrder)
                cliqueMax++;

            int[] nValues = new int[cliqueMax - cliqueMin];

            double[] pValues = new double[cliqueMax - cliqueMin];
            double[] connectivity = new double[cliqueMax - cliqueMin];
            double[] liveTime = new double[cliqueMax - cliqueMin];
            double[] avgMsgDelays = new double[cliqueMax - cliqueMin];

            double percentLive = 0.0;

            int index = 0;
            int numNodes = 0;
            int numEdges = 0;
            int totalSims = 0;
            double p;
            //Console.WriteLine("The smallest number of cliques: " + cliqueMin);
            //Console.WriteLine("The largest number of cliques: " + cliqueMax);

            System.IO.File.WriteAllText("graph_sizes_ba.txt", "");
            System.IO.File.WriteAllText("avg_connectivity_ba.txt", "");
            System.IO.File.WriteAllText("avg_msg_delays_ba.txt", "");
            System.IO.File.WriteAllText("avg_up_time_ba.txt", "");

            for (int numCliques = cliqueMin; numCliques < cliqueMax; numCliques++)
            {
                numNodes = numCliques * (numCliques - 1);
                numEdges = numNodes * (numCliques - 1) / 2;
                System.IO.File.WriteAllText("msg_delays_ba_" + numNodes.ToString() + ".txt", "");
                p = Convert.ToDouble(numEdges) / (Convert.ToDouble(numNodes * (numNodes - 1)) / 2.0);
                nValues[index] = numNodes;
                connectivity[index] = 0.0;
                avgMsgDelays[index] = 0.0;

                //Console.WriteLine("Number or nodes: " + numNodes);
                totalSims = 0;
                for (int sim = 0; sim < numSims; sim++)
                {
                    totalSims++;
                    //Console.WriteLine("Simulation " + (sim + 1));
                    Network network = new Network(CommonGraphs.BarabasiAlbert(numNodes, 4, 3));

                    NetworkChurn netChurn = new NetworkChurn(numNodes);
                    netChurn.generateChurn(numSessions, baseTime, upDistro, downDistro);


                    double time = baseTime + 25.0;
                    double delay = 0.0;
                    percentLive = 0.0;

                    //Console.WriteLine("time = " + time);
                    bool[] status = netChurn.getStatusAtTime(time);

                    double numLive = 0;
                    for (int i = 0; i < status.Length; i++)
                    {
                        if (status[i])
                            numLive += 1.0;
                    }

                    percentLive += (numLive / Convert.ToDouble(status.Length)) * 100.0;
                    liveTime[index] += percentLive;

                    network.updateStatus(status);
                    if (network.isCurrentNetworkConnected())
                        connectivity[index] += 1.0;

                    Message msg = new Message(network, netChurn, time);
                    try
                    {
                        delay = msg.getMessageDelay();
                        avgMsgDelays[index] += delay;
                        System.IO.File.AppendAllText("msg_delays_ba_" + numNodes.ToString() + ".txt", delay.ToString() + Environment.NewLine);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Simulation " + sim + ": " + e);
                        sim--;
                    }
                }

                connectivity[index] = (connectivity[index] / Convert.ToDouble(totalSims)) * 100.0;
                avgMsgDelays[index] = avgMsgDelays[index] / Convert.ToDouble(numSims);
                liveTime[index] = liveTime[index] / Convert.ToDouble(totalSims);
                Console.WriteLine("BA graph family with {0} nodes is connected {1:N2}% of the time.", nValues[index], connectivity[index]);
                Console.WriteLine("The average message delay between two random nodes is {0:N4}.", avgMsgDelays[index]);
                Console.WriteLine("On average, {0:N2}% nodes are live at any given time", liveTime[index]);
                Console.WriteLine();

                System.IO.File.AppendAllText("graph_sizes_ba.txt", nValues[index].ToString() + Environment.NewLine);
                System.IO.File.AppendAllText("avg_connectivity_ba.txt", connectivity[index].ToString() + Environment.NewLine);
                System.IO.File.AppendAllText("avg_msg_delays_ba.txt", avgMsgDelays[index].ToString() + Environment.NewLine);
                System.IO.File.AppendAllText("avg_up_time_ba.txt", liveTime[index].ToString() + Environment.NewLine);

                index++;
            }
        }


        /// <summary>
        /// This simulation is a modification of the sim3BA method to implement parallelism.
        /// </summary>
        public void simBA3Parallel()
        {
            if (upDistro == null)
                throw new NullReferenceException("Error: Must set up-time and down-time distributions!");

            int numSims = 10000;

            int cliqueMin = 14;
            while (cliqueMin * (cliqueMin - 1) < minOrder)
                cliqueMin++;


            int cliqueMax = cliqueMin;
            while (cliqueMax * (cliqueMax - 1) < maxOrder)
                cliqueMax++;

            int[] nValues = new int[cliqueMax - cliqueMin];

            double[] pValues = new double[cliqueMax - cliqueMin];
            double[] connectivity = new double[cliqueMax - cliqueMin];
            double[] liveTime = new double[cliqueMax - cliqueMin];
            double[] avgMsgDelays = new double[cliqueMax - cliqueMin];
                        
            int index = 0;
            int numNodes = 0;
            int numEdges = 0;
            int totalSims = 0;
            //Console.WriteLine("The smallest number of cliques: " + cliqueMin);
            //Console.WriteLine("The largest number of cliques: " + cliqueMax);

            System.IO.File.WriteAllText("graph_sizes_ba.txt", "");
            System.IO.File.WriteAllText("avg_connectivity_ba.txt", "");
            System.IO.File.WriteAllText("avg_msg_delays_ba.txt", "");
            System.IO.File.WriteAllText("avg_up_time_ba.txt", "");

            for (int numCliques = cliqueMin; numCliques < cliqueMax; numCliques++)
            {
                numNodes = numCliques * (numCliques - 1);
                numEdges = numNodes * (numCliques - 1) / 2;
                System.IO.File.WriteAllText("msg_delays_ba_" + numNodes.ToString() + ".txt", "");
                nValues[index] = numNodes;
                connectivity[index] = 0.0;
                avgMsgDelays[index] = 0.0;
                totalSims = 0;
                
                //Console.WriteLine("Number or nodes: " + numNodes);
                #region Parallel_Loop
                Parallel.For(0, numSims, new ParallelOptions { MaxDegreeOfParallelism = 20 }, sim =>
                {
                    lock (_lock[0])
                    {
                        totalSims++;
                    }
                    //Console.WriteLine("Simulation " + (sim + 1));
                    double delay = 0.0;
                    bool tryMessage = false;
                    while (!tryMessage)
                    {
                        Network network = new Network(CommonGraphs.BarabasiAlbert(numNodes, 4, 3));

                        NetworkChurn netChurn = new NetworkChurn(numNodes);
                        lock (_lock[1])
                        {
                            netChurn.generateChurn(numSessions, baseTime, upDistro, downDistro);
                        }

                        double time = baseTime + 25.0;
                        double percentLive = 0.0;
                        bool isConnected = false;

                        //Console.WriteLine("time = " + time);
                        bool[] status = netChurn.getStatusAtTime(time);

                        double numLive = 0;
                        for (int i = 0; i < status.Length; i++)
                        {
                            if (status[i])
                                numLive += 1.0;
                        }

                        percentLive = (numLive / Convert.ToDouble(status.Length)) * 100.0;
                        
                        network.updateStatus(status);
                        if (network.isCurrentNetworkConnected())
                        {
                            isConnected = true;
                        }

                        try
                        {
                            Message msg = new Message(network, netChurn, time);
                            delay = msg.getMessageDelay();
                            lock (_lock[2])
                            {
                                liveTime[index] += percentLive;
                            }

                            if (isConnected)
                            {
                                lock (_lock[3])
                                {
                                    connectivity[index] += 1.0;
                                }
                            }

                            tryMessage = true;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: Failed message attempt! " + e.Message);
                        }
                    }
                    lock (_lock[4])
                    {
                        avgMsgDelays[index] += delay;
                        System.IO.File.AppendAllText("msg_delays_ba_" + numNodes.ToString() + ".txt", delay.ToString() + Environment.NewLine);
                    }
                });
                #endregion

                connectivity[index] = (connectivity[index] / Convert.ToDouble(totalSims)) * 100.0;
                avgMsgDelays[index] = avgMsgDelays[index] / Convert.ToDouble(numSims);
                liveTime[index] = liveTime[index] / Convert.ToDouble(totalSims);
                Console.WriteLine("BA graph family with {0} nodes is connected {1:N2}% of the time.", nValues[index], connectivity[index]);
                Console.WriteLine("The average message delay between two random nodes is {0:N4}.", avgMsgDelays[index]);
                Console.WriteLine("On average, {0:N2}% nodes are live at any given time", liveTime[index]);
                Console.WriteLine();

                System.IO.File.AppendAllText("graph_sizes_ba.txt", nValues[index].ToString() + Environment.NewLine);
                System.IO.File.AppendAllText("avg_connectivity_ba.txt", connectivity[index].ToString() + Environment.NewLine);
                System.IO.File.AppendAllText("avg_msg_delays_ba.txt", avgMsgDelays[index].ToString() + Environment.NewLine);
                System.IO.File.AppendAllText("avg_up_time_ba.txt", liveTime[index].ToString() + Environment.NewLine);

                index++;
            }
        }


        /// <summary>
        /// This method generates a mechanism for sampling the residual distribution
        /// H(x) = P(R(t) &lt; x) that does not require restarting the process.  We
        /// generated a bus schedule from the Paretto distribution and the arrival
        /// times of passengers (waiting on the next bus) from the exponential
        /// distribution.
        /// 
        /// Using these times, we then calculated the wait time (residual) for each 
        /// passenger.  From multiple runs, we generated the CDF of the sampled
        /// residuals.
        /// </summary>
        public static void sampling()
        {
            const int SAMPLE_RUNS = 10000;
            const int NUMBER_SESSIONS = 10000;
            const int MONITOR_START = 200;
            const int H_SIZE = 500;

            double alpha = 3.0;
            double beta = 1.0;
            double lambda = (alpha - 1.0) / beta;

            // This distribution is for bus arrivals.  That is, each session
            // corresponds to the length of time from one bus arrival to the
            // next. 
            Paretto distro1 = new Paretto(alpha, beta);

            // This distribution is used to specify the amount of time between
            // the arrival of one bus and its departure. In this case, bus 
            // arrival /departure is treated as instantaneous; that is, the bus 
            // arrives and departs both within the single moment in time.
            Constant distro2 = new Constant(0);

            // This distribution is used for passenger arrivals.  Passengers
            // arrive at this rate to wait for the next bus.         
            Exponential distro3 = new Exponential(lambda);

            NodeTimeline busTimeline = new NodeTimeline(NUMBER_SESSIONS, MONITOR_START);
            NodeTimeline passengerTimeline = new NodeTimeline(NUMBER_SESSIONS, MONITOR_START);

            // Repeatedly calculate new bus timelines and passenger arrival timelines.
            // From these, calculate the residual wait times for the passengers.  Then,
            // add these to the histogram.
            double[] residuals;
            double[] cdf = new double[H_SIZE];
            for (int run = 0; run < SAMPLE_RUNS; run++)
            {
                busTimeline.generateTimeline(distro1, distro2);
                passengerTimeline.generateTimeline(distro3, distro2);

                residuals = busTimeline.getResiduals(passengerTimeline);
                double threshold = 0.0;
                for (int i = 0; i < H_SIZE; i++)
                {
                    threshold = Convert.ToDouble(i);
                    for (int j = 0; j < residuals.Length; j++)
                    {
                        if (residuals[j] <= threshold)
                            cdf[i]++;
                    }
                }
            }

            // Generate the cdf from the histogram.
            for (int i = 0; i < cdf.Length; i++)
                cdf[i] = cdf[i] / Convert.ToDouble(NUMBER_SESSIONS * SAMPLE_RUNS);

            // Write the cdf to a text file.
            System.IO.File.AppendAllLines("c:/Temp_For_Grading/sampling.txt", cdf.Select(d => d.ToString()).ToArray());
        }


        /// <summary>
        /// Assume that a P2P system has n participating users whose online
        /// presence is given by independently alternating renewal processes.
        /// Each user's ON duration is drawn from a Paretto distribution where 
        /// alpha = 3, and its OFF duration is drawn from an Exponential 
        /// distribution where lambda = 1.
        /// 
        /// The purpose of this simulation is to examine the arrival process
        /// of all users in the system and to determine the cdf of the inter-
        /// arrival delays between successive joins into the system.
        /// </summary>
        public static void superposition1()
        {
            const int SAMPLE_RUNS = 10000;
            const int NUMBER_NODES = 100;
            const int NUMBER_SESSIONS = 500;
            const int SAMPLE_SIZE = 1000;
            const int MONITOR_START = 200;
            const int H_SIZE = 100;

            double alpha = 3.0;
            double beta = 1.0;
            double lambda = 1.0;

            // This distribution is for up time generation.  That is, each 
            // session corresponds to the length of time that the node is
            // live.
            Paretto distro1 = new Paretto(alpha, beta);

            // This distribution is for down time generation.  That is, 
            // each value corresponds to the length of time from the 
            // previous session ends until the new session begins.
            Exponential distro2 = new Exponential(lambda);

            double[] interArrivalDelay = new double[SAMPLE_SIZE];
            double[] cdf = new double[H_SIZE];

            double mean = 0.0;
            for (int run = 0; run < SAMPLE_RUNS; run++)
            {
                Console.WriteLine("Run #" + run);
                NetworkChurn churn = new NetworkChurn(NUMBER_NODES);
                churn.generateChurn(NUMBER_SESSIONS, MONITOR_START, distro1, distro2);
                double[] startTimes = churn.getStartTimes();

                for (int i = 0; i < interArrivalDelay.Length; i++)
                {
                    interArrivalDelay[i] = startTimes[i + 1] - startTimes[i];
                    mean += interArrivalDelay[i];
                }

                double cap = 0.0;
                double rate = 0.01;
                for (int i = 0; i < H_SIZE; i++)
                {
                    for (int j = 0; j < interArrivalDelay.Length; j++)
                    {
                        if (interArrivalDelay[j] <= cap)
                            cdf[i]++;
                    }
                    cap += rate;
                }
            }

            mean = mean / (SAMPLE_RUNS * interArrivalDelay.Length);
            Console.WriteLine("Mean = " + mean);

            // Generate the cdf from the histogram.
            for (int i = 0; i < cdf.Length; i++)
                cdf[i] = cdf[i] / (SAMPLE_RUNS * interArrivalDelay.Length);

            // Write the cdf to a text file.
            System.IO.File.WriteAllText("c:/Temp_For_Grading/superposition1.txt", mean.ToString());
            System.IO.File.AppendAllLines("c:/Temp_For_Grading/superposition1.txt", cdf.Select(d => d.ToString()).ToArray());
        }


        /// <summary>
        /// 
        /// </summary>
        public static void Superposition1()
        {
            MersenneTwister randomNum = new MersenneTwister();

            const long n = 100;
            const long SAMPLE_RUNS = 1000;

            const long SAMPLE_SIZE = 10000;
            const int H_SIZE = 100;

            double[] next_arrival = new double[n];

            double[] inter_arrival = new double[SAMPLE_SIZE];
            double[] cdf = new double[H_SIZE];

            double alpha = 3.0;
            double beta = 1.0;
            double lambda = 1.0;

            double mean = 0.0;

            for (int i = 0; i < H_SIZE; i++)
                cdf[i] = 0.0;

            for (int i = 0; i < n; i++)
                next_arrival[i] = randomNum.genparet_real(alpha, beta) + randomNum.genexp_real(lambda);

            double time = 0.0;
            int index = 0;
            for (int i = 0; i < SAMPLE_RUNS; i++)
            {
                Console.WriteLine("Run {0}", i);
                for (int j = 0; j < SAMPLE_SIZE; j++)
                {
                    index = 0;
                    for (int k = 1; k < n; k++)                     // find the next arrival time
                    {
                        if (next_arrival[k] < next_arrival[index])
                            index = k;
                    }

                    inter_arrival[j] = next_arrival[index] - time;  // calculate this sample of inter-arrival time
                    mean += inter_arrival[j];
                    time = next_arrival[index];
                    next_arrival[index] += randomNum.genparet_real(alpha, beta) + randomNum.genexp_real(lambda);
                }

                double cap = 0.0;
                double rate = 0.01;
                for (int j = 0; j < H_SIZE; j++)
                {
                    for (int k = 0; k < SAMPLE_SIZE; k++)
                    {
                        if (inter_arrival[k] < cap)
                            cdf[j] += 1.0;
                    }

                    cap += rate;
                }
            }

            mean = mean / (SAMPLE_SIZE * SAMPLE_RUNS);
            for (int i = 0; i < H_SIZE; i++)
            {
                cdf[i] = cdf[i] / (SAMPLE_SIZE * SAMPLE_RUNS);
            }

            System.IO.File.WriteAllText("c:/Temp_For_Grading/superposition1.txt", mean.ToString());
            System.IO.File.AppendAllLines("c:/Temp_For_Grading/superposition1.txt", cdf.Select(d => d.ToString()).ToArray());
        }

        public static void Superposition2()
        {
            MersenneTwister randomNum = new MersenneTwister();

            const long n = 10;
            const long SAMPLE_RUNS = 10;
            const long SAMPLE_SIZE = 10000;
            const int H_SIZE = 20;

            const double time_base = 10.0 * n;
            const double interval = 1.0;

            double[] next_arrival = new double[n];
            double[] pmf = new double[H_SIZE];

            double arrivals = 0;

            double alpha = 3.0;
            double beta = 1.0;
            double lambda = 1.0;

            double mean = 0.0;

            for (int i = 0; i < H_SIZE; i++)
                pmf[i] = 0;

            for (int i = 0; i < SAMPLE_RUNS; i++)
            {
                Console.WriteLine("Run {0}", i);
                for (int j = 0; j < SAMPLE_SIZE; j++)
                {
                    arrivals = 0.0;
                    for (int k = 0; k < n; k++)
                    {
                        next_arrival[k] = 0;
                        while (next_arrival[k] < time_base)
                        {
                            next_arrival[k] += randomNum.genparet_real(alpha, beta) + randomNum.genexp_real(lambda);
                        }

                        while (next_arrival[k] <= (time_base + interval))
                        {
                            arrivals += 1.0;
                            next_arrival[k] += randomNum.genparet_real(alpha, beta) + randomNum.genexp_real(lambda);
                        }
                    }

                    mean += arrivals;
                    pmf[(int)arrivals] += 1.0;
                }
            }

            mean = mean / (SAMPLE_SIZE * SAMPLE_RUNS);
            for (int i = 0; i < H_SIZE; i++)
            {
                pmf[i] = pmf[i] / (SAMPLE_SIZE * SAMPLE_RUNS);
            }

            System.IO.File.WriteAllText("superposition2.txt", mean.ToString());
            System.IO.File.AppendAllLines("superposition2.txt", pmf.Select(d => d.ToString()).ToArray());
        }

        public static void LiveUsers()
        {
            MersenneTwister randomNum = new MersenneTwister();

            const long n = 1000;

            const long SAMPLE_RUNS = 10;
            const long SAMPLE_SIZE = 1000;
            const int H_SIZE = 1000;

            const double time_base = 200.0;

            double[] next_arrival = new double[n];

            double[] histogram = new double[H_SIZE];
            double[] cdf = new double[H_SIZE];

            double alpha = 3.0;
            double beta = 1.0;
            double lambda = 1.0;

            double total_arrivals = 0.0;

            double x_i = 0.0;
            double w_i = 0.0;
            double y_i = 0.0;

            double mean_x = 0.0;
            double mean_y = 0.0;

            for (int i = 0; i < H_SIZE; i++)
            {
                histogram[i] = 0.0;
                cdf[i] = 0.0;
            }

            for (int i = 0; i < SAMPLE_RUNS; i++)
            {
                for (int j = 0; j < SAMPLE_SIZE; j++)
                {
                    Console.WriteLine("Run {0}", j);
                    for (int k = 0; k < n; k++)
                    {
                        next_arrival[k] = 0;
                        while (next_arrival[k] < time_base)
                        {
                            x_i = randomNum.genparet_real(alpha, beta);
                            w_i = randomNum.genexp_real(lambda);
                            next_arrival[k] += x_i;
                            if (next_arrival[k] > time_base)
                            {
                                mean_x += x_i;
                                y_i = x_i + w_i;
                                mean_y += y_i;
                                total_arrivals += 1.0;
                                if (y_i < H_SIZE)
                                {
                                    histogram[(int)y_i] += 1.0;
                                }
                            }
                            next_arrival[k] += w_i;
                        }
                    }
                }
            }

            for (int i = 0; i < H_SIZE; i++)
            {
                for (int k = 0; k < H_SIZE; k++)
                {
                    if (k <= i)
                        cdf[i] += histogram[k];
                }
            }

            mean_x = mean_x / total_arrivals;
            mean_y = mean_y / total_arrivals;

            for (int i = 0; i < H_SIZE; i++)
            {
                cdf[i] = cdf[i] / total_arrivals;
            }

            System.IO.File.WriteAllText("live_users.txt", mean_x.ToString());
            System.IO.File.AppendAllText("live_users.txt", "\n");
            System.IO.File.AppendAllText("live_users.txt", mean_y.ToString());
            System.IO.File.AppendAllText("live_users.txt", "\n");
            System.IO.File.AppendAllLines("live_users.txt", cdf.Select(d => d.ToString()).ToArray());
        }
    }
}
