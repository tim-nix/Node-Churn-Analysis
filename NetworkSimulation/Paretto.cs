using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    public class Paretto : Distribution
    {
        IRandomSource randomNum;
        double alpha = 0.0;
        double beta = 0.0;


        public double Alpha
        {
            get
            {
                return alpha;
            }
        }


        public double Beta
        {
            get
            {
                return beta;
            }
        }

        
        public Paretto(double a, double b)
            : this(a, b, new MersenneTwister())
        {
        }

        public Paretto(double a, double b, IRandomSource randomSource)
        {
            if (a <= 1.0)
                throw new ArgumentException("Error: Value for alpha must be greater then 1.0!");

            if (b <= 0.0)
                throw new ArgumentException("Error: Value for beta must be positive!");
            if (randomSource == null)
                throw new ArgumentNullException("randomSource");

            randomNum = randomSource;
            alpha = a;
            beta = b;
        }

        public override double generateRandom()
        {
            double random = 1.0 / (1.0 - randomNum.NextClosedUnit());
            return (Math.Pow(random, 1.0 / alpha) - 1.0) * beta;
        }

        public override Distribution WithRandomSource(IRandomSource randomSource)
        {
            return new Paretto(alpha, beta, randomSource);
        }

        public override double getExpectedValue()
        {
            return beta / (alpha - 1.0);
        }


        public override double[] getPMF(int hSize = 50)
        {
            double[] pmf = new double[hSize];
            double cap = 0.0;
            for (int i = 0; i < hSize; i++)
            {
                pmf[i] = 1.0 - Math.Pow(1.0 + (cap / beta), -alpha);
                cap += Rate;
            }

            return pmf;
        }
    }
}
