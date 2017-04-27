using System;


namespace NetworkSimulation
{
    class Program
    {
        static void Main(string[] args)
        { 
            Simulations sim = new Simulations();
            ContinuousUniform upD = new ContinuousUniform(0.0, 1.0);
            ContinuousUniform downD = new ContinuousUniform(0.0, 2.0);
            sim.setUpDistro(upD, downD);
            sim.simGnp2();
        }
    }
}
