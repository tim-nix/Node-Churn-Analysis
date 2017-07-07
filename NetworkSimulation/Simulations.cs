using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private static Random r = new Random();


        public Simulations(int minN = 100, 
                           int maxN = 1000, 
                           int nDelta = 100, 
                           double startTime = 200.0, 
                           double tDelta = 0.5, 
                           int sessionsPerNode = 100)
        {
            setNodeRange(minN, maxN, nDelta);
            setTimeRange(startTime, tDelta);
            setNumberOfSessions(sessionsPerNode);
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


        public void simClique()
        {
            if (upDistro == null)
                throw new NullReferenceException("Error: Must set up-time and down-time distributions!");

            for (int numNodes = minOrder; numNodes < maxOrder; numNodes += nodeDelta)
            {
                Network network = new Network(CommonGraphs.Clique(numNodes));

                NetworkChurn netChurn = new NetworkChurn(numNodes);
                netChurn.generateChurn(numSessions, baseTime, upDistro, downDistro);

                double time = baseTime;
                double connectionCount = 0.0;
                double iterations = 0.0;
                double percentLive = 0;
                double endTime = netChurn.getEarliestFinalTime();

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
                    time += timeDelta;
                }

                Console.WriteLine("Clique with {0} nodes is connected {1}% of the time.", numNodes, (connectionCount / iterations) * 100);
            }
        }

        public void simGH()
        {
            if (upDistro == null)
                throw new NullReferenceException("Error: Must set up-time and down-time distributions!");

            int numSims = 100;

            int cliqueMin = 11;
            while (cliqueMin * (cliqueMin - 1) < minOrder)
                cliqueMin++;


            int cliqueMax = cliqueMin;
            while (cliqueMax * (cliqueMax - 1) < maxOrder)
                cliqueMax++;

            int[] nValues = new int[cliqueMax - cliqueMin];

            double[] pValues = new double[cliqueMax - cliqueMin];
            double[] connectivity = new double[cliqueMax - cliqueMin];

            double percentLive = 0.0;
            double iterations = 0.0;

            int index = 0;
            int numNodes = 0;
            for (int numCliques = cliqueMin; numCliques < cliqueMax; numCliques++)
            {
                numNodes = numCliques * (numCliques - 1);

                nValues[index] = numNodes;
                connectivity[index] = 0.0;


                for (int sim = 0; sim < numSims; sim++)
                {
                    Network network = new Network(CommonGraphs.GuntherHartnell(numNodes));

                    NetworkChurn netChurn = new NetworkChurn(numNodes);
                    netChurn.generateChurn(numSessions, baseTime, upDistro, downDistro);

                    double time = baseTime;
                    double connectionCount = 0.0;
                    percentLive = 0.0;
                    iterations = 0;

                    double endTime = netChurn.getEarliestFinalTime();

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
                        time += timeDelta;
                    }

                    connectivity[index] += (connectionCount / iterations) * 100.0;
                }

                connectivity[index] = connectivity[index] / Convert.ToDouble(numSims);

                Console.WriteLine("GH graph family with {0} nodes is connected {1:N2}% of the time.", nValues[index], connectivity[index]);
                Console.WriteLine("Each node is live {0:N2}% of the time", (percentLive / iterations) * 100.0);

                index++;
            }

            System.IO.File.WriteAllLines("c:/Temp_For_Grading/nvalues_gh.txt", nValues.Select(d => d.ToString()).ToArray());
            System.IO.File.WriteAllLines("c:/Temp_For_Grading/cvalues_gh.txt", connectivity.Select(d => d.ToString()).ToArray());
        }

        public void simGnp(double p)
        {
            int numSims = 100;

            int[] nValues = new int[maxOrder - minOrder];

            double[] connectivity = new double[maxOrder - minOrder];

            double percentLive = 0.0;
            double iterations = 0.0;

            int index = 0;
            for (int numNodes = minOrder; numNodes < maxOrder; numNodes += nodeDelta)
            {
                nValues[index] = numNodes;
                connectivity[index] = 0.0;

                for (int sim = 0; sim < numSims; sim++)
                {
                    Network network = new Network(CommonGraphs.Gnp(numNodes, p));

                    NetworkChurn netChurn = new NetworkChurn(numNodes);
                    netChurn.generateChurn(numSessions, baseTime, upDistro, downDistro);

                    double time = baseTime;
                    double connectionCount = 0.0;
                    percentLive = 0.0;
                    iterations = 0;

                    double endTime = netChurn.getEarliestFinalTime();

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
                        time += timeDelta;
                    }

                    connectivity[index] += (connectionCount / iterations) * 100.0;
                }

                connectivity[index] = connectivity[index] / Convert.ToDouble(numSims);
                
                Console.WriteLine("Gnp graph family with {0} nodes and p = {1:N2} is connected {2:N2}% of the time.", nValues[index], p, connectivity[index]);
                Console.WriteLine("Each node is live {0:N2}% of the time", (percentLive / iterations) * 100.0);

                index++;
            }

            System.IO.File.WriteAllLines("c:/Temp_For_Grading/nvalues_gnp.txt", nValues.Select(d => d.ToString()).ToArray());
            System.IO.File.WriteAllLines("c:/Temp_For_Grading/cvalues_gnp.txt", connectivity.Select(d => d.ToString()).ToArray());
        }

        /// <summary>
        /// This simulation is set up for direct comparison of Gnp to Gunther-Hartnell.
        /// </summary>
        public void simGnp2()
        {
            if (upDistro == null)
                throw new NullReferenceException("Error: Must set up-time and down-time distributions!");

            int numSims = 500;

            int cliqueMin = 11;
            while (cliqueMin * (cliqueMin - 1) < minOrder)
                cliqueMin++;


            int cliqueMax = cliqueMin;
            while (cliqueMax * (cliqueMax - 1) < maxOrder)
                cliqueMax++;

            int[] nValues = new int[cliqueMax - cliqueMin];

            double[] pValues = new double[cliqueMax - cliqueMin];
            double[] connectivity = new double[cliqueMax - cliqueMin];
            double[] liveTime = new double[cliqueMax - cliqueMin];
            double[] msgDelays = new double[cliqueMax - cliqueMin];

            double percentLive = 0.0;
            double iterations = 0.0;

            int index = 0;
            int numNodes = 0;
            int numEdges = 0;
            double p;
            for (int numCliques = cliqueMin; numCliques < cliqueMax; numCliques++)
            {
                numNodes = numCliques * (numCliques - 1);
                numEdges = numNodes * (numCliques - 1) / 2;
                p = Convert.ToDouble(numEdges) / (Convert.ToDouble(numNodes * (numNodes - 1)) / 2.0);
                nValues[index] = numNodes;
                connectivity[index] = 0.0;
                msgDelays[index] = 0.0;


                for (int sim = 0; sim < numSims; sim++)
                {
                    Network network = new Network(CommonGraphs.Gnp(numNodes, p));

                    NetworkChurn netChurn = new NetworkChurn(numNodes);
                    netChurn.generateChurn(numSessions, baseTime, upDistro, downDistro);

                    double time = (baseTime + netChurn.getEarliestFinalTime()) / 2.0;
                    double connectionCount = 0.0;
                    double avgDelay = 0.0;
                    percentLive = 0.0;
                    iterations = 0;

                    //double endTime = netChurn.getEarliestFinalTime();
                    double endTime = time + timeDelta;
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

                        Message msg = new Message(network, netChurn, time);
                        avgDelay += msg.getMessageDelay();

                        iterations += 1.0;
                        time += timeDelta;
                    }

                    connectivity[index] += (connectionCount / iterations) * 100.0;
                    msgDelays[index] += avgDelay / iterations;
                    //Console.WriteLine("On this iteration, a path existed " + (pathCount / iterations) * 100.0 + " percent of the time.");
                }

                connectivity[index] = connectivity[index] / Convert.ToDouble(numSims);
                msgDelays[index] = msgDelays[index] / Convert.ToDouble(numSims);
                liveTime[index] = (percentLive / iterations) * 100.0;
                Console.WriteLine("GH graph family with {0} nodes is connected {1:N2}% of the time.", nValues[index], connectivity[index]);
                Console.WriteLine("The average message delay between two random nodes is {0:N2}.", msgDelays[index]);
                Console.WriteLine("On average, {0:N2}% nodes are live at any given time", liveTime[index]);

                index++;
            }

            System.IO.File.WriteAllLines("c:/Temp_For_Grading/nvalues_gh.txt", nValues.Select(d => d.ToString()).ToArray());
            System.IO.File.WriteAllLines("c:/Temp_For_Grading/cvalues_gh.txt", connectivity.Select(d => d.ToString()).ToArray());
            System.IO.File.WriteAllLines("c:/Temp_For_Grading/pathvalues_gh.txt", msgDelays.Select(d => d.ToString()).ToArray());
            System.IO.File.WriteAllLines("c:/Temp_For_Grading/lvalues_gh.txt", liveTime.Select(d => d.ToString()).ToArray());
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
        /// distribution where lambda = 2.
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
