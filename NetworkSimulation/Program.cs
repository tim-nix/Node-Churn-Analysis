using System;
using System.Collections;

namespace NetworkSimulation
{
    class Program
    {
        // The purpose of this method is to simulate transmission of a
        // message from a source node to a destination node in which each
        // has an ON duration of lambda and an OFF duration of mu. The source
        // node is ON at the start each simulation.  Time to message delivery
        // calculated for the specified number of simulations.
        // *** This set of simulations use simPath in which the last node ***
        // *** to receive the message may switch OFF before it delivers    ***
        // *** the message to the next node along the path. The message is ***
        // *** only delivered once the node with the message and the next  ***
        // *** node along the path are both ON.                            *** 
        public static void pathExponential()
        {
            Simulations sim = new Simulations(minN: 2, maxN: 6, nDelta: 1, numSims: 1000);
            Distribution upD = new Exponential(2.0);
            Distribution downD = new Exponential(3.0);
            sim.setUpDistro(upD, downD);
            sim.simPath();
        }

        public static void cycleExponential()
        {
            Simulations sim = new Simulations(minN: 4, maxN: 12, nDelta: 2, numSims: 1000);
            Distribution upD = new Exponential(2.0);
            Distribution downD = new Exponential(3.0);
            sim.setUpDistro(upD, downD);
            sim.simCycle();
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Starting path experiment...");
            pathExponential();
            Console.WriteLine("Finished path experiment. Starting cycle experiment...");
            cycleExponential();
            Console.WriteLine("Finished cycle experiment.");
        }
    }
}
