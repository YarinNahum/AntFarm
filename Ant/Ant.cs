using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLibrary
{
    public class Ant : IAnt
    {

        public Ant(int id, int lowStrength, int highStrength)
        {
            State = State.Alive;
            Id = id;
            Age = 0;
            SleepCount = 0;
            Strength = lowStrength == highStrength? lowStrength : new Random().Next(lowStrength, highStrength + 1);
        }

        public override void CalculateSleep(int low, int high)
        {
            SleepCount = low == high? low : new Random().Next(low, high + 1);
        }

        public override string DecideAction()
        {
            return new Random().NextDouble() >= 0.5 ? "Move" : "Fight";
        }

        public override void Fight(IAnt other)
        {
            Action<IAnt, IAnt> act = (IAnt winner, IAnt loser) => {winner.Strength += 2; loser.State = State.Depressed; };
            if (Strength > other.Strength)
                act(this, other);
            else if (other.Strength > Strength)
                act(other, this);
        }
    }
}
