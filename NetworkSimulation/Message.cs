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

        private static Random r = new Random();

        public Message(Network nw, NetworkChurn ch, double time_t)
        {
            network = nw;
            churn = ch;
            start_time = time_t;
        }


        public double getMessageDelay()
        {
            if (network.CurrentOrder < 2)
                throw new Exception("Not enough nodes!");

            int startVertex = r.Next(0, network.CurrentOrder);
            startVertex = network.getOldNodeLabel(startVertex);
            int endVertex = startVertex;

            while (startVertex == endVertex)
                endVertex = r.Next(0, network.FullOrder);

            int currStart = network.getNewNodeLabel(startVertex);
            int currStop;

            HashSet<Int32> locations = new HashSet<int>();
            HashSet<Int32> newLocations = new HashSet<int>();

            for (int j = 0; j < network.FullOrder; j++)
            {
                currStop = network.getNewNodeLabel(j);
                if ((currStop != -1) && (network.isPathinCurrentNetwork(currStart, currStop)))
                    locations.Add(j);
            }

            double timeDelta = 0.01;
            double curr_time = start_time;

            while (!locations.Contains(endVertex))
            {
                curr_time += timeDelta;
                bool[] status = churn.getStatusAtTime(curr_time);
                network.updateStatus(status);

                foreach (int i in locations)
                {
                    for (int j = 0; j < network.FullOrder; j++)
                    {
                        if ((i != j) && (!locations.Contains(j)))
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

            return curr_time - start_time;
        }
    }
}
