using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    class Message
    {
        private Network network;
        private NetworkChurn churn;
        private double start_time;
        private double final_time;

        private static Random r = new Random();

        public Message(Network nw, NetworkChurn ch, double time_t)
        {
            network = nw;
            churn = ch;
            start_time = time_t;
            final_time = churn.getLastFinalTime();
        }


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
            
            //if (locations.Contains(endVertex))
            //    Console.WriteLine("Source and sink both live.");
            //else
            //    Console.WriteLine("Residue: " + (churn.getNextStartTimeForNode(endVertex, curr_time) - curr_time));

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
    }
}
