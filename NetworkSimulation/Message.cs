using System;

namespace NetworkSimulation
{
    /// <summary>
    /// Computes message-delivery delay over a network whose live nodes change
    /// according to a NetworkChurn process.
    /// </summary>
    class Message
    {
        private readonly Network network;
        private readonly NetworkChurn churn;
        private readonly double start_time;

        private static readonly Random r = new Random();

        /// <summary>
        /// Initializes an event-driven message-delivery calculation.
        /// </summary>
        /// <param name="nw">Static full-network topology.</param>
        /// <param name="ch">Generated, lazily extensible node churn.</param>
        /// <param name="time_t">Message creation time.</param>
        public Message(Network nw, NetworkChurn ch, double time_t)
        {
            if (nw == null)
                throw new ArgumentNullException("nw");
            if (ch == null)
                throw new ArgumentNullException("ch");
            if (time_t < 0.0 || double.IsNaN(time_t) || double.IsInfinity(time_t))
                throw new ArgumentOutOfRangeException("time_t");

            network = nw;
            churn = ch;
            start_time = time_t;
        }

        /// <summary>
        /// Computes delay from a random live source to a random destination.
        /// </summary>
        /// <returns>Elapsed simulated time until delivery.</returns>
        public double getMessageDelay()
        {
            if (network.CurrentOrder < 1)
                throw new Exception("Not enough nodes!");
            if (network.FullOrder < 2)
                throw new Exception("A distinct destination does not exist.");

            int currentSource;
            int destination;
            lock (r)
            {
                currentSource = r.Next(0, network.CurrentOrder);
                destination = r.Next(0, network.FullOrder - 1);
            }

            int source = network.getOldNodeLabel(currentSource);
            if (destination >= source)
                destination++;

            return GetFloodingDelay(source, destination);
        }

        /// <summary>
        /// Computes flooding delay from node 0 to node n - 1 on a path.
        /// </summary>
        /// <returns>Elapsed simulated time until delivery.</returns>
        public double getPathMessageDelay()
        {
            return GetFloodingDelay(0, network.FullOrder - 1);
        }

        /// <summary>
        /// Computes delay for a single-copy, sequential path forwarding model.
        /// </summary>
        /// <returns>Elapsed simulated time until delivery.</returns>
        public double getPathMessageDelay2()
        {
            bool[] status = churn.getStatusAtTime(start_time);
            if (!status[0])
                throw new Exception("Node 0 is not live!");

            int currentLocation = 0;
            double currentTime = start_time;

            while (currentLocation != network.FullOrder - 1)
            {
                while (currentLocation + 1 < network.FullOrder
                    && status[currentLocation + 1])
                {
                    currentLocation++;
                }

                if (currentLocation == network.FullOrder - 1)
                    break;

                currentTime = GetNextTransition(currentTime);
                status = churn.getStatusAtTime(currentTime);
            }

            return currentTime - start_time;
        }

        /// <summary>
        /// Computes flooding delay from node 0 to the diameter node of a cycle.
        /// </summary>
        /// <returns>Elapsed simulated time until delivery.</returns>
        public double getCycleMessageDelay()
        {
            return GetFloodingDelay(0, network.FullOrder / 2);
        }

        /// <summary>
        /// Computes flooding delay between shared multipath endpoints 0 and 1.
        /// </summary>
        /// <returns>Elapsed simulated time until delivery.</returns>
        public double getMultiPathMessageDelay()
        {
            return GetFloodingDelay(0, 1);
        }

        /// <summary>
        /// Dispatches to the endpoint interpretation selected by mode.
        /// </summary>
        /// <param name="mode">Topology-specific delivery mode.</param>
        /// <returns>Elapsed simulated time until delivery.</returns>
        public double getMessageDelay(MessageDelayMode mode)
        {
            switch (mode)
            {
                case MessageDelayMode.PathEndpoint:
                    return getPathMessageDelay();

                case MessageDelayMode.CycleDiameter:
                    return getCycleMessageDelay();

                case MessageDelayMode.MultiPathEndpoint:
                    return getMultiPathMessageDelay();

                default:
                    throw new ArgumentException("Unsupported delay mode.");
            }
        }

        /// <summary>
        /// Floods the message through live connected components, advancing only
        /// at actual node state transitions.
        /// </summary>
        private double GetFloodingDelay(int source, int destination)
        {
            if (source < 0 || source >= network.FullOrder)
                throw new ArgumentOutOfRangeException("source");
            if (destination < 0 || destination >= network.FullOrder)
                throw new ArgumentOutOfRangeException("destination");

            bool[] status = churn.getStatusAtTime(start_time);
            if (!status[source])
                throw new Exception("Source node is not live!");

            bool[] locations = new bool[network.FullOrder];
            locations[source] = true;

            double currentTime = start_time;
            while (true)
            {
                network.expandMessageLocations(locations, status);
                if (locations[destination])
                    return currentTime - start_time;

                currentTime = GetNextTransition(currentTime);
                status = churn.getStatusAtTime(currentTime);
            }
        }

        /// <summary>
        /// Returns and validates the next churn event after currentTime.
        /// </summary>
        private double GetNextTransition(double currentTime)
        {
            double nextTime = churn.getNextTransitionTime(currentTime);
            if (nextTime <= currentTime
                || double.IsNaN(nextTime)
                || double.IsInfinity(nextTime))
            {
                throw new InvalidOperationException(
                    "Churn did not produce a valid future transition.");
            }

            return nextTime;
        }
    }
}
