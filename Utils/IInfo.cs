using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public interface IInfo
    {
         int FoodPerDay { get; }
         int ObjectStrengthDecay { get; }
         int ObjectStartStrengthLow { get; }
         int ObjectStartStrengthHigh { get; }
         int MinObjectsPerArea { get; }
         int MaxObjectsPerArea { get; }
         int TimePerDay { get; }
         int NumberOfObjects { get; }
         int Length { get; }
         int Hight { get; }
         int ObjectSleepDaysLow { get; }
         int ObjectSleepDaysHigh { get; }
         int ID { get; }
    }
}
