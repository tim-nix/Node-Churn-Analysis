using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    /// <summary>
    /// Computes message-delivery delay over a dynamic network whose current
    /// live-node subgraph changes according to a NetworkChurn process.
    /// 
    /// A message begins at a source node at start_time and propagates whenever
    /// a node holding the message is connected to another live node in the
    /// current network. Delay is measured as the elapsed simulated time until
    /// the destination receives the message.
    /// </summary>
    class Message
    {
        private Network network;
        private NetworkChurn churn;
        private double start_time;
        private double final_time;

        private static Random r = new Random();

        /// <summary>
        /// Initializes a message-delay calculation over the supplied network and
        /// churn process.
        /// </summary>
        /// <param name="nw">Network topology whose current status changes over time.</param>
        /// <param name="ch">Generated node churn process.</param>
        /// <param name="time_t">Message generation time.</param>
        public Message(Network nw, NetworkChurn ch, double time_t)
        {
            network = nw;
            churn = ch;
            start_time = time_t;
            final_time = churn.getLastFinalTime();
        }

        /// <summary>
        /// Computes message delay between a randomly selected live source and a
        /// randomly selected destination in the full network.
        /// </summary>
        /// <returns>
        /// The elapsed time between message generation and delivery.
        /// </returns>
        /// <remarks>
        /// This method is useful for general topology experiments but is not the
        /// controlled endpoint-to-endpoint path/cycle comparison used in the
        /// current redundancy experiments.
        /// </remarks>
        public double getMessageDelay()
        {
            if (network.CurrentOrder < 1)
                throw new Exception("Not enough nodes!");
            
            int startVertex = r.Next(0, network.CurrentOrder);      // Pick a random live source
            startVertex = network.getOldNodeLabel(startVertex);     // Convert to full network source label
            int endVertex = startVertex;
            
            while (startVertex == endVertex)                        // Pick a random sink different
                endVertex = r.Next(0, network.FullOrder);           // than the source

            int currStart = network.getNewNodeLabel(startVertex);   // Convert source to current label
            int currStop;
            
            HashSet<Int32> locations = new HashSet<int>();          // Stores message locations
            HashSet<Int32> newLocations = new HashSet<int>();
            
            // Determine spread of message from source at start time.
            locations.Add(startVertex);
            for (int j = 0; j < network.FullOrder; j++)             
            {
                currStop = network.getNewNodeLabel(j);              // Convert each full label to current label
                if ((currStart != currStop) && (currStop != -1) && (network.isPathinCurrentNetwork(currStart, currStop)))
                    locations.Add(j);                               // if live and path from source, then add
            }
            
            double timeDelta = 0.001;
            double curr_time = start_time;
            
            while (!locations.Contains(endVertex) && (curr_time < final_time))               
            {
                curr_time += timeDelta;
                bool[] status = churn.getStatusAtTime(curr_time);
                network.updateStatus(status);
                
                foreach (int i in locations)
                {
                    
                    for (int j = 0; j < network.FullOrder; j++)
                    {
                        
                        if (!locations.Contains(j))
                        {
                            currStart = network.getNewNodeLabel(i);
                            currStop = network.getNewNodeLabel(j);
                            if ((currStart != -1) && (currStop != -1))
                            {
                                if (network.isPathinCurrentNetwork(currStart, currStop))
                                    newLocations.Add(j);
                            }
                        }
                    }
                }
                
                foreach (int i in newLocations)
                    locations.Add(i);
                newLocations.Clear();
            }

            //Console.WriteLine("Message delay: " + (curr_time - start_time));
            if (curr_time >= final_time)
                throw new Exception("Error: Message not delivered!");

            return curr_time - start_time;
        }


        /// <summary>
        /// Computes endpoint-to-endpoint message delay on a path topology.
        /// </summary>
        /// <returns>
        /// The elapsed time required for a message starting at node 0 to reach
        /// node n - 1.
        /// </returns>
        /// <remarks>
        /// The source node must be live at the message start time. The message
        /// spreads through currently connected live components and continues
        /// propagating as churn changes the current network.
        /// </remarks>
        public double getPathMessageDelay()
        {
            if (network.getNewNodeLabel(0) == -1)
                throw new Exception("Node 0 is not live!");

            int startVertex = 0;                                    // Pick the first node
            int endVertex = network.FullOrder - 1;

            int currStart = network.getNewNodeLabel(startVertex);   // Convert source to current label
            int currStop;

            HashSet<Int32> locations = new HashSet<int>();          // Stores message locations
            HashSet<Int32> newLocations = new HashSet<int>();

            // Determine spread of message from source at start time.
            locations.Add(startVertex);
            for (int j = 0; j < network.FullOrder; j++)
            {
                currStop = network.getNewNodeLabel(j);              // Convert each full label to current label
                if ((currStart != currStop) && (currStop != -1) && (network.isPathinCurrentNetwork(currStart, currStop)))
                    locations.Add(j);                               // if live and path from source, then add
            }

            double timeDelta = 0.001;
            double curr_time = start_time;

            // Track spread of message from source until the destination is reached.

            while (!locations.Contains(endVertex) && (curr_time < final_time))
            {
                curr_time += timeDelta;
                bool[] status = churn.getStatusAtTime(curr_time);
                network.updateStatus(status);

                foreach (int i in locations)
                {

                    for (int j = 0; j < network.FullOrder; j++)
                    {

                        if (!locations.Contains(j))
                        {
                            currStart = network.getNewNodeLabel(i);
                            currStop = network.getNewNodeLabel(j);
                            if ((currStart != -1) && (currStop != -1))
                            {
                                if (network.isPathinCurrentNetwork(currStart, currStop))
                                    newLocations.Add(j);
                            }
                        }
                    }
                }

                foreach (int i in newLocations)
                    locations.Add(i);
                newLocations.Clear();
            }

            //Console.WriteLine("Message delay: " + (curr_time - start_time));
            if (curr_time >= final_time)
                throw new Exception("Error: Message not delivered!");

            return curr_time - start_time;
        }


        /// <summary>
        /// Computes path message delay using a sequential forwarding model in
        /// which the message advances to the next path node when that next node
        /// becomes live.
        /// </summary>
        /// <returns>
        /// The elapsed time required for a message starting at node 0 to reach
        /// node n - 1.
        /// </returns>
        /// <remarks>
        /// This method differs from getPathMessageDelay by tracking a single
        /// current message location rather than a set of all reached nodes.
        /// </remarks>
        public double getPathMessageDelay2()
        {
            if (network.getNewNodeLabel(0) == -1)                   // Start only if source is ON
                throw new Exception("Node 0 is not live!");

            int startVertex = 0;                                    // Pick the source from full network
            int endVertex = network.FullOrder - 1;                  // Pick the destination from full network

            int currStart = network.getNewNodeLabel(startVertex);   // Convert source to current label
            int currStop;

            // Determine spread of message from source at start time.
            int currLoc = startVertex;                      // Source has a copy of the message
            for (int j = 0; j < network.FullOrder; j++)
            {
                currStop = network.getNewNodeLabel(j);              // Convert each full label to current label
                if ((currStart != currStop) && (currStop != -1) && (network.isPathinCurrentNetwork(currStart, currStop)))
                    currLoc = j;
            }

            double timeDelta = 0.001;
            double curr_time = start_time;

            // Track spread of message from source until the destination is reached.

            while ((currLoc != endVertex) && (curr_time < final_time))
            {
                curr_time += timeDelta;
                bool[] status = churn.getStatusAtTime(curr_time);
                network.updateStatus(status);

                currStop = network.getNewNodeLabel(currLoc + 1);
                if (currStop != -1)
                    currLoc += 1;
            }

            //Console.WriteLine("Message delay: " + (curr_time - start_time));
            if (curr_time >= final_time)
                throw new Exception("Error: Message not delivered!");

            return curr_time - start_time;
        }

        /// <summary>
        /// Computes source-to-diameter message delay on a cycle topology.
        /// </summary>
        /// <returns>
        /// The elapsed time required for a message starting at node 0 to reach
        /// node n / 2 in the cycle graph.
        /// </returns>
        /// <remarks>
        /// The destination node is chosen at graph distance n / 2 so that the
        /// cycle provides two equal-length alternate routes from source to target.
        /// </remarks>
        public double getCycleMessageDelay()
        {
            if (network.getNewNodeLabel(0) == -1)
                throw new Exception("Node 0 is not live!");

            int startVertex = 0;                                    // Pick the first node
            int endVertex = network.FullOrder / 2;

            int currStart = network.getNewNodeLabel(startVertex);   // Convert source to current label
            int currStop;

            HashSet<Int32> locations = new HashSet<int>();          // Stores message locations
            HashSet<Int32> newLocations = new HashSet<int>();

            // Determine spread of message from source at start time.
            locations.Add(startVertex);
            for (int j = 0; j < network.FullOrder; j++)
            {
                currStop = network.getNewNodeLabel(j);              // Convert each full label to current label
                if ((currStart != currStop) && (currStop != -1) && (network.isPathinCurrentNetwork(currStart, currStop)))
                    locations.Add(j);                               // if live and path from source, then add
            }

            double timeDelta = 0.001;
            double curr_time = start_time;

            // Track spread of message from source until the destination is reached.

            while (!locations.Contains(endVertex) && (curr_time < final_time))
            {
                curr_time += timeDelta;
                bool[] status = churn.getStatusAtTime(curr_time);
                network.updateStatus(status);

                foreach (int i in locations)
                {

                    for (int j = 0; j < network.FullOrder; j++)
                    {

                        if (!locations.Contains(j))
                        {
                            currStart = network.getNewNodeLabel(i);
                            currStop = network.getNewNodeLabel(j);
                            if ((currStart != -1) && (currStop != -1))
                            {
                                if (network.isPathinCurrentNetwork(currStart, currStop))
                                    newLocations.Add(j);
                            }
                        }
                    }
                }

                foreach (int i in newLocations)
                    locations.Add(i);
                newLocations.Clear();
            }

            //Console.WriteLine("Message delay: " + (curr_time - start_time));
            if (curr_time >= final_time)
                throw new Exception("Error: Message not delivered!");

            return curr_time - start_time;
        }

        /// <summary>
        /// Dispatches to the appropriate controlled message-delay calculation.
        /// </summary>
        /// <param name="mode">
        /// Delay mode specifying whether to use path endpoint delivery or cycle
        /// diameter delivery.
        /// </param>
        /// <returns>
        /// The message-delivery delay for the selected mode.
        /// </returns>
        public double getMessageDelay(MessageDelayMode mode)
        {
            switch (mode)
            {
                case MessageDelayMode.PathEndpoint:
                    return getPathMessageDelay();

                case MessageDelayMode.CycleDiameter:
                    return getCycleMessageDelay();

                default:
                    throw new ArgumentException("Unsupported delay mode.");
            }
        }
    }
}
