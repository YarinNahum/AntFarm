using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StaticObjects;
using IDynamicObjects;
using Utils;


namespace DynamicObjects
{
    public class Ant : IDynamicObject
    {

        //For testing purposes
        public Ant() : base()
        {
            int lowStrength = info.ObjectStartStrengthLow;
            int highStrength = info.ObjectStartStrengthHigh;
            Strength = lowStrength == highStrength? lowStrength: MyRandom.Next(lowStrength, highStrength + 1);
        }

        public Ant(IInfo info,IRandomTest rnd) : base()
        {
            this.rnd = rnd;
            this.info = info; 
        }

        public override void Fight(IDynamicObject other)
        {
            // a local Action for the winner and the loser
            void act(IDynamicObject winner, IDynamicObject loser) {
                // add 2 to the strength of the winner
                winner.AddStrength(2); 
                // the loser is now depressed
                loser.SetState(State.Depressed);

                ProducerConsumer.Produce(String.Format("Object number {0} won agains object number {1}",
                    winner.Id,loser.Id)); 
            }

            /// the winner of the fight is the object with more strength. if the strengths of objects are the same
            /// than there is no winner
            if (Strength > other.Strength || other.DeBuff == DeBuff.Cocoon)
                act(this, other);
            else if (other.Strength > Strength)
                act(other, this);
        }

        public override void ActOnStaticObject(Food x)
        {
            ProducerConsumer.Produce(String.Format("Ant number {0} ate food!", Id));
            Strength += 2;
        }

        protected override void ActAliveObject()
        {
            
            // decide what action to perform
            string action = DecideAction();
            if (action.Equals("Move"))
            {
                TryToMove();
            }
            else if (action.Equals("Fight"))
            {
                TryToFight();
            }
        }

        protected override void ActDepressedObject()
        {
            var dynamicObjects = BoardFunctions.GetNearObjects(X, Y,1);

            /// if the number of adjacent objects are in the range of [info.MinObjectsPerArea,info.MaxObjectsPerArea]
            /// than the given object will no longer be depressed.
            if (dynamicObjects.Count >= info.MinObjectsPerArea && dynamicObjects.Count <= info.MaxObjectsPerArea)
            {
                ProducerConsumer.Produce(String.Format("Object number {0} is no longer depressed, found {1} objects near it", Id, dynamicObjects.Count));
                SetState(State.Alive);
            }
            else
            {
                ProducerConsumer.Produce(String.Format("Object number {0} is still depressed, found {1} objects near it", Id, dynamicObjects.Count));
            }
        }
    }
}
