using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StaticObjects;
using Utils;


namespace DynamicObjects
{
    public class Ant : IDynamicObject
    {

        //For testing purposes
        public Ant()
        {
            Id = 0;
            State = State.Alive;
            Age = 0;
            Strength = 3;
        }
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
            // a local Action for the winner and the loser
            void act(IDynamicObject winner, IDynamicObject loser) {
                // add 2 to the strength of the winner
                winner.AddStrength(2); 
                // the loser is now depressed
                loser.SetState(State.Depressed);

                Console.WriteLine("Object number {0} won agains object number {1}\nWinner position: {2},{3}\nLoser Position {4},{5}",
                    winner.Id,loser.Id, winner.X,winner.Y,loser.X,loser.Y); 
            }
            /// the winner of the fight is the object with more strength. if the strengths of objects are the same
            /// than there is no winner
            if (Strength > other.Strength)
                act(this, other);
            else if (other.Strength > Strength)
                act(other, this);
        }

        public override void ActOnStaticObject(Food x)
        {
            Console.WriteLine("Ant number {0} ate food!", Id);
            Strength += 2;
        }
    }
}
