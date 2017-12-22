using System;


namespace NetworkSimulation
{
    class Program
    {
        static void Main(string[] args)
        { 
            Simulations sim = new Simulations();
            Distribution upD = new Paretto(3.0, 2.0);
            Distribution downD = new Exponential(1.0);
            sim.setUpDistro(upD, downD);
            sim.simGH();
        }
    }
}
