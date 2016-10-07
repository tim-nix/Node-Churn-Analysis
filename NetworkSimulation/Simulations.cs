using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSimulation
{
    class Simulations
    {
        public static void Sampling()
        {
            MersenneTwister randomNum = new MersenneTwister();

            const long SAMPLE_RUNS = 10000;
            const long SAMPLE_SIZE = 10000;
            const int H_SIZE = 500;

            double[] residuals = new double[SAMPLE_SIZE];
            double[] cdf = new double[H_SIZE];

            double alpha = 3.0;
            double beta = 1.0;
            double lambda = (alpha - 1.0) / beta;

            for (int i = 0; i < H_SIZE; i++)
                cdf[i] = 0.0;

            double bus_time = randomNum.genparet_real(alpha, beta);
            double sample_time = randomNum.genexp_real(lambda);

            for(int i = 0; i < SAMPLE_RUNS; i++)
            {
                for(int j = 0; j < SAMPLE_RUNS; j++)
                {
                    while (bus_time < sample_time)
                    {
                        bus_time += randomNum.genparet_real(alpha, beta);
                    }

                    residuals[j] = bus_time - sample_time;
                    sample_time += randomNum.genexp_real(lambda);
                }

                for (int j = 0; j < H_SIZE; j++)                // calculate the CCDF of the wait times
                {
                    for (int k = 0; k < SAMPLE_SIZE; k++)
                    {
                        if (residuals[k] <= (double)j)
                            cdf[j] += 1.0;
                    }
                }
            }

            for (int i = 0; i < H_SIZE; i++)
            {
                cdf[i] = cdf[i] / (SAMPLE_SIZE * SAMPLE_RUNS);
            }

            System.IO.File.WriteAllLines("cdf.txt", cdf.Select(d => d.ToString()).ToArray());
        }

        public static void Superposition1()
        {
            MersenneTwister randomNum = new MersenneTwister();

            const long n = 100;
            const long SAMPLE_RUNS = 1000;

            const long SAMPLE_SIZE = 10000;
            const int H_SIZE = 50;

            double[] next_arrival = new double[n];

            double[] inter_arrival = new double[SAMPLE_SIZE];
            double[] cdf = new double[H_SIZE];

            double alpha = 3.0;
            double beta = n;
            double lambda = 1.0 / n;

            double mean = 0.0;

            for (int i = 0; i < H_SIZE; i++)
                cdf[i] = 0.0;

            for (int i = 0; i < n; i++)
                next_arrival[i] = randomNum.genparet_real(alpha, beta) + randomNum.genexp_real(lambda);

            double time = 0.0;
            int index = 0;
            for (int i = 0; i < SAMPLE_RUNS; i++)
            {
                Console.WriteLine("Run {0}", i);
                for (int j = 0; j < SAMPLE_SIZE; j++)
                {
                    index = 0;
                    for (int k = 1; k < n; k++)                     // find the next arrival time
                    {
                        if (next_arrival[k] < next_arrival[index])
                            index = k;
                    }

                    inter_arrival[j] = next_arrival[index] - time;  // calculate this sample of inter-arrival time
                    mean += inter_arrival[j];
                    time = next_arrival[index];
                    next_arrival[index] += randomNum.genparet_real(alpha, beta) + randomNum.genexp_real(lambda);
                }

                double cap = 0.0;
                double rate = 0.5;
                for (int j = 0; j < H_SIZE; j++)
                {
                    for (int k = 0; k < SAMPLE_SIZE; k++)
                    {
                        if (inter_arrival[k] < cap)
                            cdf[j] += 1.0;
                    }

                    cap += rate;
                }
            }

            mean = mean / (SAMPLE_SIZE * SAMPLE_RUNS);
            for (int i = 0; i < H_SIZE; i++)
            {
                cdf[i] = cdf[i] / (SAMPLE_SIZE * SAMPLE_RUNS);
            }

            System.IO.File.WriteAllText("superposition1.txt", mean.ToString());
            System.IO.File.AppendAllLines("superposition1.txt", cdf.Select(d => d.ToString()).ToArray());
        }

        public static void Superposition2()
        {
            MersenneTwister randomNum = new MersenneTwister();

            const long n = 10;
            const long SAMPLE_RUNS = 10;
            const long SAMPLE_SIZE = 10000;
            const int H_SIZE = 20;

            const double time_base = 10.0 * n;
            const double interval = 1.0;

            double[] next_arrival = new double[n];
            double[] pmf = new double[H_SIZE];

            double arrivals = 0;

            double alpha = 3.0;
            double beta = 1.0;
            double lambda = 1.0;

            double mean = 0.0;

            for (int i = 0; i < H_SIZE; i++)
                pmf[i] = 0;

            for (int i = 0; i < SAMPLE_RUNS; i++)
            {
                Console.WriteLine("Run {0}", i);
                for (int j = 0; j < SAMPLE_SIZE; j++)
                {
                    arrivals = 0.0;
                    for (int k = 0; k < n; k++)
                    {
                        next_arrival[k] = 0;
                        while (next_arrival[k] < time_base)
                        {
                            next_arrival[k] += randomNum.genparet_real(alpha, beta) + randomNum.genexp_real(lambda);
                        }

                        while (next_arrival[k] <= (time_base + interval))
                        {
                            arrivals += 1.0;
                            next_arrival[k] += randomNum.genparet_real(alpha, beta) + randomNum.genexp_real(lambda);
                        }
                    }

                    mean += arrivals;
                    pmf[(int)arrivals] += 1.0;
                }
            }

            mean = mean / (SAMPLE_SIZE * SAMPLE_RUNS);
            for (int i = 0; i < H_SIZE; i++)
            {
                pmf[i] = pmf[i] / (SAMPLE_SIZE * SAMPLE_RUNS);
            }

            System.IO.File.WriteAllText("superposition2.txt", mean.ToString());
            System.IO.File.AppendAllLines("superposition2.txt", pmf.Select(d => d.ToString()).ToArray());
        }

        public static void LiveUsers()
        {
            MersenneTwister randomNum = new MersenneTwister();

            const long n = 1000;

            const long SAMPLE_RUNS = 10;
            const long SAMPLE_SIZE = 1000;
            const int H_SIZE = 1000;

            const double time_base = 200.0;

            double[] next_arrival = new double[n];

            double[] histogram = new double[H_SIZE];
            double[] cdf = new double[H_SIZE];

            double alpha = 3.0;
            double beta = 1.0;
            double lambda = 1.0;

            double total_arrivals = 0.0;

            double x_i = 0.0;
            double w_i = 0.0;
            double y_i = 0.0;

            double mean_x = 0.0;
            double mean_y = 0.0;

            for (int i = 0; i < H_SIZE; i++)
            {
                histogram[i] = 0.0;
                cdf[i] = 0.0;
            }

            for (int i = 0; i < SAMPLE_RUNS; i++)
            {
                for (int j = 0; j < SAMPLE_SIZE; j++)
                {
                    Console.WriteLine("Run {0}", j);
                    for (int k = 0; k < n; k++)
                    {
                        next_arrival[k] = 0;
                        while (next_arrival[k] < time_base)
                        {
                            x_i = randomNum.genparet_real(alpha, beta);
                            w_i = randomNum.genexp_real(lambda);
                            next_arrival[k] += x_i;
                            if (next_arrival[k] > time_base)
                            {
                                mean_x += x_i;
                                y_i = x_i + w_i;
                                mean_y += y_i;
                                total_arrivals += 1.0;
                                if (y_i < H_SIZE)
                                {
                                    histogram[(int)y_i] += 1.0;
                                }
                            }
                            next_arrival[k] += w_i;
                        }
                    }
                }
            }

            for (int i = 0; i < H_SIZE; i++)
            {
                for (int k = 0; k < H_SIZE; k++)
                {
                    if (k <= i)
                        cdf[i] += histogram[k];
                }
            }

            mean_x = mean_x / total_arrivals;
            mean_y = mean_y / total_arrivals;

            for (int i = 0; i < H_SIZE; i++)
            {
                cdf[i] = cdf[i] / total_arrivals;
            }

            System.IO.File.WriteAllText("live_users.txt", mean_x.ToString());
            System.IO.File.AppendAllText("live_users.txt", "\n");
            System.IO.File.AppendAllText("live_users.txt", mean_y.ToString());
            System.IO.File.AppendAllText("live_users.txt", "\n");
            System.IO.File.AppendAllLines("live_users.txt", cdf.Select(d => d.ToString()).ToArray());
        }
    }
}
