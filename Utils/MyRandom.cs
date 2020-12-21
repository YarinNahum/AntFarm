using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class MyRandom
    {
        private static Random _global = new Random();
        [ThreadStatic] private static Random _local;

        public static int Next(int low, int high)
        {
            if (_local == null)
            {
                lock (_global)
                {

                    int seed = _global.Next();
                    _local = new Random(seed);
                }
            }
            return _local.Next(low, high);
        }

        public static double NextDouble()
        {
            if (_local == null)
            {
                lock (_global)
                {

                    int seed = _global.Next();
                    _local = new Random(seed);
                }
            }
            return _local.NextDouble();
        }
    }

}
