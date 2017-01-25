using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    public class ContinuousUniform : Distribution
    {
        MersenneTwister randomNum;
        double a = 0.0;
        double b = 0.0;

        public double Min
        {
            get
            {
                return a;
            }
        }


        public double Max
        {
            get
            {
                return b;
            }
        }


        public ContinuousUniform(double min, double max)
        {
            if (min >= max)
                throw new ArgumentException("Error: lower bound must be less than upper bound!");

            randomNum = new MersenneTwister();
            a = min;
            b = max;
        }


        public override double generateRandom()
        {
            return a + (randomNum.genrand_real1() * (b - a));
        }

        public override double getExpectedValue()
        {
            return 0.5 * (a + b);
        }

        public override double[] getPMF(int hSize = 50)
        {
            setRate((b - a) / Convert.ToDouble(hSize - 1));
            double[] pmf = new double[hSize];

            for (int i = 0; i < hSize; i++)
                pmf[i] = 1.0 / (b - a);

            return pmf;
        }
    }
}
