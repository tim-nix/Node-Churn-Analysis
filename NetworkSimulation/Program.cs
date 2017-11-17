using System;


namespace NetworkSimulation
{
    class Program
    {
        static void Main(string[] args)
        { 
            Simulations sim = new Simulations();
            Distribution upD = new ContinuousUniform(0.0, 2.0);
            Distribution downD = new ContinuousUniform(0.0, 2.0);
            sim.setUpDistro(upD, downD);
            sim.simGnp3();
        }
    }
}
