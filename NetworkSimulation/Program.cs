using System;
using System.Collections;

namespace NetworkSimulation
{
    class Program
    {
        // The purpose of this method is to simulate transmission of a
        // message from a source node to a destination node in which each
        // has an ON duration of lambda_1 and an OFF duration of lambda_2.  
        // The source node is ON at the start each simulation.  Time to 
        // message delivery calculated for 10,000 simulations.
        // *** This set of simulations use simPath1 in which the last node ***
        // *** to receive the message may switch OFF before it delivers    ***
        // *** the message to the next node along the path. The message is ***
        // *** only delivered once the node with the message and the next  ***
        // *** node along the path are both ON.                            *** 
        public static void pathExponential1()
        {
            Simulations sim = new Simulations(minN: 2, maxN: 6, nDelta: 1);
            Distribution upD = new Exponential(10.0);
            Distribution downD = new Exponential(1.0);
            sim.setUpDistro(upD, downD);
            sim.simPath1();
        }

        // The purpose of this method is to simulate transmission of a
        // message from a source node to a destination node in which each
        // has an ON duration of lambda_1 = 1 and an OFF duration of 
        // lambda_2 = 1.  The source node is ON at the start each
        // simulation.  Time to message delivery calculated for 10,000 
        // simulations. 
        // *** This set of simulations use simPath2 in which the last node ***
        // *** to receive the message will stay ON until it delivers the   ***
        // *** message to the next node along the path.                    *** 
        public static void pathExponential2()
        {
            Simulations sim = new Simulations(minN: 2, maxN: 6, nDelta: 1);
            Distribution upD = new Exponential(1.0);
            Distribution downD = new Exponential(1.0);
            sim.setUpDistro(upD, downD);
            sim.simPath2();
        }

        static void Main(string[] args)
        {
            pathExponential1();
        }
    }
}
