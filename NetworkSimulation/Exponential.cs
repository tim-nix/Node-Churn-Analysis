using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    public class Exponential : Distribution
    {
        MersenneTwister randomNum;
        double lambda = 0;
        double distroRate = 0.5;

        public double Lambda
        {
            get
            {
                return lambda;
            }
        }


        public double Rate
        {
            get
            {
                return distroRate;
            }
        }

        public Exponential(double lam)
        {
            if (lam <= 0.0)
                throw new ArgumentException("Error: Value for alpha must be positive!");

            randomNum = new MersenneTwister();
            lambda = lam;
        }

        public override double generateRandom()
        {
            return randomNum.genexp_real(lambda);
        }

        public override double getExpectedValue()
        {
            return 1.0 / lambda;
        }

        public void setRate(double r)
        {
            if (r <= 0.0)
                throw new ArgumentException("Error: Distribution increase rate must be positive!");

            distroRate = r;
        }

        public override double[] getPMF(int hSize = 50)
        {
            double[] pmf = new double[hSize];
            double cap = 0.0;
            for (int i = 0; i < hSize; i++)
            {
                pmf[i] = lambda * (Math.Pow(Math.E, -lambda * cap));
                cap += distroRate;
            }

            return pmf;
        }
    }
}
