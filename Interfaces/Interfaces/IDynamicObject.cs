using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using StaticObjects;
using ProducerConsumer;

namespace DynamicObjects
{
    /// <summary>
    /// An abstract class for any dynamic object that can be on the game board.
    /// Any object must inherit this class
    /// </summary>
    public abstract class IDynamicObject
    {
        /// <summary>
        /// The id of the object.
        /// </summary>
        public virtual int Id { get;protected set; }

        /// <summary>
        /// The current state of the object.
        /// See <see cref="State"/> for all the states
        /// </summary>
        public virtual State State { get; set; }

        /// <summary>
        /// The current strength value of the object.
        /// If the strength is 0, the object will die.
        /// </summary>
        public virtual int Strength { get;  set; }

        /// <summary>
        /// The current age of the object.
        /// The age will grow by 1 each day that passes.
        /// </summary>
        public virtual int Age { get;  set; }

        /// <summary>
        /// The current value of how many days the object will sleep.
        /// If SleepCount is bigger than 0, the object will not do any actions that day.
        /// SleepCount decreases each day by 1.
        /// </summary>
        public virtual int SleepCount { get; protected set; }

        /// <summary>
        /// The X axis of the object's position.
        /// </summary>
        public virtual int X { get; set; }

        /// <summary>
        /// The Y axis of the object's position.
        /// </summary>
        public virtual int Y { get; set; }

        public virtual IProducerConsumerMessages<string> ProducerConsumer { get; set; }


        /// <summary>
        /// Add to Strength value by amount
        /// </summary>
        /// <param name="amount"></param>
        public virtual void AddStrength(int amount)
        {
            Strength += amount;
        }


        /// <summary>
        /// Set the State value by state
        /// </summary>
        /// <param name="state"></param>
        public virtual void SetState(State state)
        {
            State = state;
        }

        /// <summary>
        /// Randomly decide what action to perform.
        /// 50% to move, 50% to fight.
        /// uses the <see cref="MyRandom"/> class for the randomizer.
        /// </summary>
        /// <returns>A string: Move or Fight</returns>
        public virtual string DecideAction()
        {
            return MyRandom.NextDouble() >= 0.5 ? "Move" : "Fight";
        }

        /// <summary>
        /// Decide how to act when fighing another object.
        /// Every IDynamicObject class that inherits this class must override this function
        /// </summary>
        /// <param name="other">another IDynamicObject instance</param>
        public abstract void Fight(IDynamicObject other);

        /// <summary>
        /// Calculate how much sleep to add to the SleepCount from low to high
        /// This function uses the <see cref="MyRandom"/> class for the randomizer
        /// </summary>
        /// <param name="low">The lower bound of the interval</param>
        /// <param name="high">The higher bound of the interval</param>
        public virtual void CalculateSleep(int low, int high)
        {
            SleepCount = low == high ? low : MyRandom.Next(low, high + 1);
        }

        /// <summary>
        /// Decrease the SleepCount by 1
        /// </summary>
        public virtual void WakeUp()
        {
            SleepCount--;
        }

        /// <summary>
        /// Decide what to do when there is a Food object at the same position as this IDynamicObject.
        /// Every DynamicObject class that inherits this class must override this function.
        /// </summary>
        /// <param name="obj">An instance of a Food class</param>
        public abstract void ActOnStaticObject(Food obj);

    }
    /// <summary>
    /// All possible states of a IDynamicObject.
    /// </summary>
    public enum State { Alive, Depressed, Dead };

}

