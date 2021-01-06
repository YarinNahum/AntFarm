using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using Utils;
using IDynamicObjects;
using BoardNamespace;
using ProducerConsumer;

namespace GameLogicNameSpace
{
    public class GameLogic : IGameLogic
    {
        #region variables
        private IInfo info;
        private IBoard board;

        // the number of finished tasks each day
        private static long finishedTasksOfTheDay = 0;

        // a concurrent dictionary that the key is the object id, and the value is the AutoResetEvent associated with it.
        private readonly ConcurrentDictionary<int, AutoResetEvent> ares;

        // an AutoResetEvent for the main thread of the program
        private readonly AutoResetEvent mainThreadARE;

        private readonly Stopwatch clock;

        //an AutoResetEvent for the task that creates additional dynamic objects mid-run
        private readonly AutoResetEvent creatorTaskARE;
        bool playing;

        //producer-consumer for the prints in the console
        private readonly IProducerConsumerMessages<string> producerConsumer;

        //for testing purposes
        private RandomOption randomOption = RandomOption.RealTime;
        private IRandomTest rnd;
        #endregion

        public GameLogic()
        {
            info = Info.Instance;
            clock = new Stopwatch();
            ares = new ConcurrentDictionary<int, AutoResetEvent>();
            mainThreadARE = new AutoResetEvent(false);
            creatorTaskARE = new AutoResetEvent(false);
            producerConsumer = new ProducerConsumerMessages();
            board = new Board(info.Length, info.Hight, producerConsumer);
        }

        public void GameLogicTest(IInfo info, IBoard board, IRandomTest rnd)
        {
            this.info = info;
            this.board = board;
            this.rnd = rnd;
            randomOption = RandomOption.Testing;
        }

        public void GenerateInitialObjects()
        {
            int count = info.NumberOfObjects;
            if (count > info.Length * info.Hight)
                throw new ArgumentException(String.Format("The number of objects to create {0} is bigger than the size of the board {1}", count, info.Length * info.Hight));

            // generate the initial objects
            var generatedInitial = board.GenerateInitialObjects(count);

            // create a task for each object, and add a new AutoResetEvent to the dictinory associated with the object id.
            foreach (IDynamicObject obj in generatedInitial)
            {
                ares.TryAdd(obj.Id, new AutoResetEvent(false));
                Task.Factory.StartNew(() => DynamicObjectAction(obj), TaskCreationOptions.LongRunning);
            }
            playing = true;
            // create a task for the purpose of creating additional dynamic object mid-run
            Task.Factory.StartNew(() =>
            {
                creatorTaskARE.WaitOne();
                while (playing)
                {
                    while (clock.ElapsedMilliseconds < info.TimePerDay)
                        TryCreateObject();
                    creatorTaskARE.WaitOne();
                }
            });

        }

        public void WakeUp()
        {
            var nonDeadObjects = board.GetAlive();
            foreach (IDynamicObject obj in nonDeadObjects)
            {
                //only wake up an object that is sleeping
                if (obj.SleepCount != 0)
                    obj.WakeUp();
            }
        }

        public void UpdateAlive()
        {
            var l = board.GetAlive();

            var alive = new List<IDynamicObject>();
            /// for each object alive we calculate the strength of the object.
            /// if the strength is 0 ore below, we set the state as State.Dead and we remove it from the tile.
            /// we return only the non-dead objects on the board.
            foreach (IDynamicObject obj in l)
            {
                
                obj.AddStrength(-info.ObjectStrengthDecay);
                if (obj.Strength > 0)
                {
                    obj.Age++;
                    alive.Add(obj);
                }
                else
                {
                    producerConsumer.Produce(String.Format("Object number {0} died!", obj.Id));
                    obj.SetState(State.Dead);
                    board.ClearDynamicObjectOnTile(obj.X, obj.Y);
                }

            }
            board.SetAlive(alive);

        }

        public void GenerateFood()
        {
            board.GenetareFood();
        }

        public void StartNewDay()
        {
            finishedTasksOfTheDay = 0;


            // start the clock for the day
            clock.Start();

            long numberOfAnts = ares.Count;

            // call Set() for the AutoResetEvent that controls the task that creates new dynamic objects.
            creatorTaskARE.Set();

            foreach (AutoResetEvent are in ares.Values)
            // call Set() for each AutoResetEvent in the dictinory. 
            {
                are.Set();
            }

            /// this while acts as a barrier. the main thread will pass this barrier only when all 
            /// the dynamic object's tasks will finish for the given day. 

            while (Interlocked.Read(ref finishedTasksOfTheDay) < Interlocked.Read(ref numberOfAnts))
            {
                mainThreadARE.WaitOne();
                numberOfAnts = GetNumberOfAliveObjects();
            }
            // reset the clock
            clock.Reset();
        }

        public int GetNumberOfAliveObjects()
        {

            return board.GetAlive().Count;
        }


        private void DynamicObjectAction(IDynamicObject obj)
        {
            // get the AutoResetEvent associated with the object id
            ares.TryGetValue(obj.Id, out AutoResetEvent are);

            // put the task to sleep until the main thread calls the Set() method.
            are.WaitOne();

            //As long as the object is alive
            while (obj.State != State.Dead)
            {
                // if the object is not sleeping
                if (obj.SleepCount == 0)
                {
                    //depends on the state of the object dedice what action to do
                    switch (obj.State)
                    {
                        case State.Alive:
                            obj.ActAliveObject();
                            break;
                        case State.Depressed:
                            obj.ActDepressedObject();
                            break;
                        case State.Dead:
                            break;
                    }
                    //at the end of the action go to sleep 
                    obj.CalculateSleep();
                    //check if we need to update the status of the object
                    obj.UpdateStatus();
                }
                // at the end of the day we inceremnt the number of finished tasks of the day.
                Interlocked.Increment(ref finishedTasksOfTheDay);

                // we call Set() with the AutoResetEvent variable associated with the main thread
                mainThreadARE.Set();

                // we are withing for the main thread to call Set() at the next day.
                are.WaitOne();
            }
            // after the object died we remove the key-value pair associated with it
            ares.TryRemove(obj.Id, out _);
        }

        private void TryCreateObject()
        {
            // get a random position on the board
            int x = MyRandom.Next(0, info.Length);
            int y = MyRandom.Next(0, info.Hight);

            //try to create an object at position (x,y)
            IDynamicObject obj = board.TryCreate(x, y);
            if (obj != null)
            {
                /// if an object was created, add a new enrty in the dictinory and create 
                /// a new task for the dynamic object
                AutoResetEvent are = new AutoResetEvent(false);
                are.Set();
                ares.TryAdd(obj.Id, are);
                Task.Factory.StartNew(() => DynamicObjectAction(obj), TaskCreationOptions.LongRunning);
            }
        }

        public void ConsumeAllMessages()
        {
            producerConsumer.ConsumeAll();
        }
    }
}
