using System;

namespace FBO.Classes.Mouse
{
    public class NormalDistribution
    {
        Random gaussian = new Random();
        double nextNextGaussian;

        /// <summary>
        /// Uses a Box-Muller polar method to generate a value from within a gaussian distribution with mean of 0 and standard deviation of 1
        /// </summary>
        /// <returns></returns>
        public double NextGaussian()
        {
                double v1, v2, s;
                do
                {
                    v1 = 2 * gaussian.NextDouble() - 1;   
                    v2 = 2 * gaussian.NextDouble() - 1;
   
                    s = v1 * v1 + v2 * v2;
                } while (s >= 1 || s == 0);

                double multiplier = Math.Sqrt(-2 * Math.Log(s) / s);
                nextNextGaussian = v2 * multiplier;
                
                return v1 * multiplier;

        }

    }
}
