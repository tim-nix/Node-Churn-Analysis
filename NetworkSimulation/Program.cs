using System;


namespace NetworkSimulation
{
    class Program
    {
        static void Main(string[] args)
        {
            Exponential cud = new Exponential(5.0);
            double min = 2000.0;
            double max = -1000.0;
            double n = 0;
            double sum = 0;
            for (int i = 0; i < 1000000000; i++)
            {
                n = cud.generateRandom();
                sum += n;
                if (n > max)
                    max = n;
                if (n < min)
                    min = n;
            }

            Console.WriteLine("Min = " + min + " and Max = " + max + " and Avg = " + sum / 1000000000);
            Console.WriteLine("E[x] = " + cud.getExpectedValue());
            //Simulations sim = new Simulations();
            //sim.simGnp();
        }
    }
}
