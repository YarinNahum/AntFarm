using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLibrary
{
    public abstract class IAnt
    {
        public int Id { get; set; }
        public int Age { get; set; }
        public int Strength { get; set; }
        public int SleepCount { get; set; }

        public State State { get; set; }
        public abstract string DecideAction(); 

        public abstract void Fight(IAnt other);

        public abstract void CalculateSleep(int low, int high);
    }

    public enum State{ Alive, Depressed, Dead};
}
