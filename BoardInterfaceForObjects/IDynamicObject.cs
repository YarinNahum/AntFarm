using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using StaticObjects;
using BoardInterfaceForObjects;
using ProducerConsumer;

namespace IDynamicObjects
{
    /// <summary>
    /// An abstract class for any dynamic object that can be on the game board.
    /// Any object must inherit this class
    /// </summary>
    public abstract class IDynamicObject
    {

        protected IInfo info = Info.Instance;
        public virtual IBoardFunctions BoardFunctions { get; set; }

        /// <summary>
        /// The id of the object.
        /// </summary>
        public virtual int Id { get; protected set; }

        /// <summary>
        /// The current state of the object.
        /// See <see cref="State"/> for all the states
        /// </summary>
        public virtual State State { get; set; }

        /// <summary>
        /// The current strength value of the object.
        /// If the strength is 0, the object will die.
        /// </summary>
        public virtual int Strength { get; set; }

        /// <summary>
        /// The current agility value if the object
        /// </summary>
        public virtual int Agility { get; set; }

        /// <summary>
        /// The current age of the object.
        /// The age will grow by 1 each day that passes.
        /// </summary>
        public virtual int Age { get; set; }

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
        /// Try to fight another object
        /// </summary>
        public virtual void TryToFight()
        {
            ProducerConsumer.Produce(String.Format("Object number {0} is looking for a fight!", Id));

            // get all objects near the given object
            var dynamicObjects = BoardFunctions.GetNearObjects(X, Y);

            if (dynamicObjects.Count == 0)
            {
                ProducerConsumer.Produce(String.Format("Object number {0} found no one around him to fight",Id));
                return;
            }

            // fight a random object
            int index = 0;

            index = MyRandom.Next(0, dynamicObjects.Count);


            Fight(dynamicObjects[index]);
        }

        /// <summary>
        /// Calculate how much sleep to add to the SleepCount from low to high
        /// This function uses the <see cref="MyRandom"/> class for the randomizer
        /// </summary>
        /// <param name="low">The lower bound of the interval</param>
        /// <param name="high">The higher bound of the interval</param>
        public virtual void CalculateSleep()
        {
            SleepCount = info.ObjectSleepDaysLow == info.ObjectSleepDaysHigh ? info.ObjectSleepDaysLow : MyRandom.Next(info.ObjectSleepDaysLow, info.ObjectSleepDaysHigh + 1);
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

        /// <summary>
        /// Try to move the object on the board
        /// See <see cref="IBoardFunctions"/>
        /// <seealso cref="MyRandom"/>
        /// </summary>
        /// <param name="obj">The object trying to move</param>
        public abstract void TryToMove();

        /// <summary>
        /// Decide how to act when depressed
        /// See <see cref="State"/>
        /// <seealso cref="Info.MinObjectsPerArea"/>
        /// <seealso cref="Info.MaxObjectsPerArea"/>
        /// </summary>
        public abstract void ActDepressedObject();

        /// <summary>
        /// /// Decide how to act when alive
        /// See <see cref="State"/>
        /// </summary>
        public abstract void ActAliveObject();

        /// <summary>
        /// update the status of the given object. It must catch the lock in the same tile as the object
        /// in Read mode.
        /// See <see cref="ReaderWriterLockSlim"/>
        /// <seealso cref="Tile"/>
        /// </summary>
        /// <param name="obj">Update the given object</param>
        public virtual void UpdateStatus()
        {
            var objectsNear = BoardFunctions.GetNearObjects(X, Y);

            // count is the number of objects around the given object.
            int count = objectsNear.Count;


            /// if the count is not in the boundries given by the <see cref="Info"/> class,
            /// the object's state will become <see cref="State.Depressed"/>.
            if ((count < info.MinObjectsPerArea || count > info.MaxObjectsPerArea) && State == State.Alive)
            {
                ProducerConsumer.Produce(String.Format("Object number {0} became depressed!", Id));
                SetState(State.Depressed);
            }

        }


    }
    /// <summary>
    /// All possible states of a IDynamicObject.
    /// </summary>
    public enum State { Alive, Depressed, Dead };

    public enum DeBuff { Cocoon, UnAffected }

}

