using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;
using FoodStaticObject;
using Utils;

namespace AntLibrary
{
    public class Ant : IDynamicObject
    {
        public Ant(int id, int lowStrength, int highStrength)
        {
            State = State.Alive;
            this.Id = id;
            Age = 0;
            SleepCount = 1;
            Strength = lowStrength == highStrength? lowStrength : MyRandom.Next(lowStrength, highStrength + 1);
        }

        public override void Fight(IDynamicObject other)
        {
            void act(IDynamicObject winner, IDynamicObject loser) {
                winner.AddStrength(2); 
                loser.SetState(State.Depressed);
                Console.WriteLine("Object number {0} won agains object number {1}\nWinner position: {2},{3}\nLoser Position {4},{5}",
                    winner.Id,loser.Id, winner.X,winner.Y,loser.X,loser.Y); 
            }
            if (Strength > other.Strength)
                act(this, other);
            else if (other.Strength > Strength)
                act(other, this);
        }

        public override void ActOnStaticObject(IStaticObject obj)
        {
            if (obj.GetType().Name.Equals("Food"))
                ActOnStaticObject((Food)obj);
        }

        private void ActOnStaticObject(Food x)
        {
            Console.WriteLine("Ant number {0} ate food!", Id);
            Strength += 2;
        }
    }
}
