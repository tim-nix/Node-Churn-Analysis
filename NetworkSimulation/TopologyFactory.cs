using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    public static class TopologyFactory
    {
        public static Network CreatePath(int numNodes)
        {
            return new Network(CommonGraphs.Path(numNodes));
        }
    }
}
