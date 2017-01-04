using System;


namespace NetworkSimulation
{
    class Program
    {
        static void Main(string[] args)
        {
            NodeTimeline n = new NodeTimeline(100000, 200);
            n.generatePETimeline();
            
            
            double cap = 0.0;
            double rate = 0.5;
            double[] cdf = n.getUpTimeCDF(50, rate);
            for (int i = 0; i < cdf.Length; i++)
            {
                Console.WriteLine("% < " + cap + ": " + cdf[i]);
                cap += rate;
            }
            //Simulations sim = new Simulations();
            //sim.simGnp();
        }
    }
}
