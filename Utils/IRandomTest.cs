using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public interface IRandomTest
    {
        int Next(int low, int high);
        double NextDouble();

    }
}
