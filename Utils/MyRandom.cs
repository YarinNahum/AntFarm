using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    /// <summary>
    /// A thread-safe static class to use by several threads.
    /// It uses a global Random instance to generate the seed for
    /// a local Random instance for each thread.
    /// See <see cref="Random"/>
    /// <seealso cref="ThreadStaticAttribute"/>
    /// </summary>
    public static class MyRandom
    {
        private static readonly Random global = new Random();
        [ThreadStatic] private static Random local;

        /// <summary>
        /// Returns a random integer an interval
        /// See <see cref="Random.Next(int, int)"/>
        /// </summary>
        /// <param name="low">The lower value of the interval</param>
        /// <param name="high">The higher value of the interval</param>
        /// <returns>A random integer value from an interval </returns>
        public static int Next(int low, int high)
        {
            if (local == null)
            {
                lock (global)
                {

                    int seed = global.Next();
                    local = new Random(seed);
                }
            }
            return local.Next(low, high);
        }

        /// <summary>
        /// Returns a random double value from the interval (0,1).
        /// See <see cref="Random.NextDouble()"/>
        /// </summary>
        /// <returns>A random double value from the interval (0,1) </returns>
        public static double NextDouble()
        {
            if (local == null)
            {
                lock (global)
                {

                    int seed = global.Next();
                    local = new Random(seed);
                }
            }
            return local.NextDouble();
        }
    }

}
