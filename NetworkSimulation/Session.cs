using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    class Session
    {
        private double start;
        private double end;

        public double StartTime
        {
            get
            {
                return start;
            }
        }

        public double EndTime
        {
            get
            {
                return end;
            }
        }

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

        public double getTimeLive()
        {
            return end - start;
        }

        public void printSession()
        {
            Console.WriteLine("Start = {0} and End = {1}", start, end);
        }
            
    }
}
