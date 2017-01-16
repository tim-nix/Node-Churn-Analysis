using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    public class Paretto : Distribution
    {
        MersenneTwister randomNum;
        double alpha = 0.0;
        double beta = 0.0;
        double distroRate = 0.5;


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

        public double Rate
        {
            get
            {
                return distroRate;
            }
        }


        public Paretto(double a, double b)
        {
            if (a <= 1.0)
                throw new ArgumentException("Error: Value for alpha must be greater then 1.0!");

            if (b <= 0.0)
                throw new ArgumentException("Error: Value for beta must be positive!");

            randomNum = new MersenneTwister();
            alpha = a;
            beta = b;
        }

        public override double generateRandom()
        {
            return randomNum.genparet_real(alpha, beta);
        }

        public override double getExpectedValue()
        {
            return beta / (alpha - 1.0);
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
                pmf[i] = 1.0 - Math.Pow(1.0 + (cap / beta), -alpha);
                cap += distroRate;
            }

            return pmf;
        }
    }
}
