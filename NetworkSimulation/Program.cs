using System;


namespace NetworkSimulation
{
    class Program
    {
        static void Main(string[] args)
        {
            //Simulations sim = new Simulations();
            //Distribution upD = new Exponential(1.0);
            //Distribution downD = new Exponential(1.0);
            //sim.setUpDistro(upD, downD);
            //sim.simBA3Parallel();

            int n = 20;
            int source = 0;
            Console.WriteLine("For Gunther-Harnell with n = " + n);
            for (int sink = 1; sink < n; sink++)
            {
                Console.WriteLine("\tsource = " + source + " and sink = " + sink);

                Network graph = new Network(CommonGraphs.GuntherHartnell(n));
                int[] path;
                bool[] status = new bool[n];

                for (int i = 0; i < n; i++)
                {
                    status[i] = true;
                }

                path = graph.getShortestPathFull(source, sink);
                while (-1 != path[0])
                {
                    Console.WriteLine("\t\tPath takes " + (path.Length - 1) + " hops.");

                    if (2 == path.Length)
                    {
                        graph.removeEdgeFull(source, sink);
                    }

                    for (int i = 1; i < path.Length - 1; i++)
                    {
                        status[path[i]] = false;
                    }

                    graph.updateStatus(status);
                    path = graph.getShortestPathCurrent(source, sink);
                }
            }
        }
    }
}
