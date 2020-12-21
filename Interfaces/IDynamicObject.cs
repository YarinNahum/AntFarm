using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Interfaces
{
    public abstract class IDynamicObject
    {
        public int Id { get;protected set; }

        public State State { get; protected set; }

        public int Strength { get; protected set; }

        public int Age { get;  set; }

        public int SleepCount { get; protected set; }
        public int X { get; set; }
        public int Y { get; set; }

        public virtual void AddStrength(int amount)
        {
            Strength += amount;
        }

        public virtual void SetState(State state)
        {
            State = state;
        }

        public virtual string DecideAction()
        {
            return MyRandom.NextDouble() >= 0.5 ? "Move" : "Fight";
        }

        public abstract void Fight(IDynamicObject other);

        public virtual void CalculateSleep(int low, int high)
        {
            SleepCount = low == high ? low : MyRandom.Next(low, high + 1);
        }

        public virtual void WakeUp()
        {
            SleepCount--;
        }

        public abstract void ActOnStaticObject(IStaticObject obj);

    }
    public enum State { Alive, Depressed, Dead };

}

