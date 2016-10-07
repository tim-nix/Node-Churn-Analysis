using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    class Program
    {
        static void Main(string[] args)
        {
            int[,] g1 = { { 1, 1, 1 },
                          { 1, 1, 1 },
                          { 1, 1, 1 } };

            AdjacencyMatrix am1 = new AdjacencyMatrix(g1);

            Console.WriteLine("Inital:");
            am1.displayGraph();
            int[,] g2 = am1.Graph;

            g2[0, 0] = 0;
            g2[1, 1] = 0;
            g2[2, 2] = 0;

            Console.WriteLine("Final:");
            am1.displayGraph();
        }
    }
}
