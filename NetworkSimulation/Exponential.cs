using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    public class Exponential : Distribution
    {
        IRandomSource randomNum;
        double lambda = 0;
        

        public double Lambda
        {
            get
            {
                return lambda;
            }
        }


        public Exponential(double lam)
            : this(lam, new MersenneTwister())
        {
        }

        public Exponential(double lam, IRandomSource randomSource)
        {
            if (lam <= 0.0)
                throw new ArgumentException("Error: Value for alpha must be positive!");
            if (randomSource == null)
                throw new ArgumentNullException("randomSource");

            randomNum = randomSource;
            lambda = lam;
        }

        public override double generateRandom()
        {
            return -Math.Log(randomNum.NextOpenUnit()) / lambda;
        }

        public override Distribution WithRandomSource(IRandomSource randomSource)
        {
            return new Exponential(lambda, randomSource);
        }

        public override double getExpectedValue()
        {
            return 1.0 / lambda;
        }


        public override double[] getPMF(int hSize = 50)
        {
            double[] pmf = new double[hSize];
            double cap = 0.0;
            for (int i = 0; i < hSize; i++)
            {
                pmf[i] = lambda * (Math.Pow(Math.E, -lambda * cap));
                cap += Rate;
            }

            return pmf;
        }
    }
}
