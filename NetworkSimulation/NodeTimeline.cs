using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    class NodeTimeline
    {
        double baseTime = 0.0;
        int numSessions = 100;
        Session[] timeline;

        public NodeTimeline(int size, double time)
        {
            baseTime = time;
            numSessions = size;
            timeline = new Session[numSessions];
        }


        public void generateTimeline()
        {
            MersenneTwister randomNum = new MersenneTwister();

            double alpha = 3.0;
            double beta = 1.0;
            double lambda = 1.0;

            double startTime = 0.0;
            double endTime = 0.0;
            while (endTime < baseTime)
            {
                startTime = endTime + randomNum.genexp_real(lambda);
                endTime = startTime + randomNum.genparet_real(alpha, beta);
            }

            for(int i = 0; i < numSessions; i++)
            {
                timeline[i] = new Session(startTime, endTime);

                startTime = endTime + randomNum.genexp_real(lambda);
                endTime = startTime + randomNum.genparet_real(alpha, beta);
            }
        }

        public bool timeIsLive(double time_t)
        {
            bool live = false;
            int index = 0;

            while ((index < numSessions) && !(live))
            {
                live = timeline[index].isLive(time_t);
                index++;
            }

            return live;
        }

        public void displayTimeline()
        {
            for (int i = 0; i < timeline.Length; i++)
                timeline[i].printSession();
        }
    }
}
