using System;


namespace NetworkSimulation
{
    class Program
    {
        static void Main(string[] args)
        {
            Simulations sim = new Simulations();
            Distribution upD = new Exponential(1.0);
            Distribution downD = new Exponential(1.0);
            sim.setUpDistro(upD, downD);
            sim.simBA3();
        }
    }
}
