﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    public abstract class Distribution
    {
        public abstract double generateRandom();

        public abstract double getExpectedValue();

        public abstract double[] getPMF(int hSize = 50);

        public double[] getCDF(int hSize = 50)
        {
            double[] cdf = getPMF(hSize);

            for (int i = 1; i < cdf.Length; i++)
                cdf[i] = cdf[i - 1] + cdf[i];

            return cdf;
        }

        public double[] getCCDF(int hSize = 50)
        {
            double[] ccdf = getCDF(hSize);

            for (int i = 0; i < ccdf.Length; i++)
                ccdf[i] = 1.0 - ccdf[i];

            return ccdf;
        }
    }
}
