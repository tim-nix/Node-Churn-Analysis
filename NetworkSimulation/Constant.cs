using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    class Constant : Distribution
    {
        double v;


        public Constant(double value)
        {
            v = value;
        }


        public override double generateRandom()
        {
            return v;
        }

        public override double getExpectedValue()
        {
            return v;
        }


        public override double[] getPMF(int hSize = 50)
        {
            setRate(v / Convert.ToDouble(hSize - 1));
            double[] pmf = new double[hSize];

            double cap = 0;
            for (int i = 0; i < hSize; i++)
            {
                if (cap == v)
                    pmf[i] = 1.0;
                else
                    pmf[i] = 0.0;

                cap += Rate;
            }

            return pmf;
        }
    }
}
