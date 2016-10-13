using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    class Session
    {
        double start;
        double end;

        public Session(double s, double e)
        {
            start = s;
            end = e;
        }

        public bool isLive(double time_t)
        {
            if ((time_t >= start) && (time_t <= end))
                return true;
            else
                return false;
        }

        public void printSession()
        {
            Console.WriteLine("Start = {0} and End = {1}", start, end);
        }
            
    }
}
