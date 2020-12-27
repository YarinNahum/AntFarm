using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using Utils;
using DynamicObjects;
using MyBoard;

namespace MyGameLogic
{
    public class GameLogic : IGameLogic
    {
        #region variables
        private readonly Info info;
        private readonly Board board;
        private static long finishedTasksOfTheDay = 0;
        private readonly ConcurrentDictionary<int, AutoResetEvent> ares;
        private readonly AutoResetEvent mainThreadARE;
        private readonly Stopwatch clock;
        private readonly AutoResetEvent creatorTaskARE;
        bool playing;
        #endregion

        public GameLogic()
        {
            info = Info.Instance;
            board = new Board(info.Length, info.Hight);
      
            clock = new Stopwatch();
            ares = new ConcurrentDictionary<int, AutoResetEvent>();
            mainThreadARE = new AutoResetEvent(false);
            creatorTaskARE = new AutoResetEvent(false);
        }

        public void GenerateInitialObjects()
        {
            int count = info.NumberOfObjects;
            if (count > info.Length * info.Hight)
                throw new ArgumentException(String.Format("The number of objects to create {0} is bigger than the size of the board {1}", count, info.Length * info.Hight));
            var nonDeadObjects = board.GenerateInitialObjects(count);

            foreach (IDynamicObject obj in nonDeadObjects)
            {
                ares.TryAdd(obj.Id, new AutoResetEvent(false));
                Task.Factory.StartNew(() => DynamicObjectAction(obj), TaskCreationOptions.LongRunning);

            }
            playing = true;

            Task.Factory.StartNew(() =>
            {
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
            var nonDeadObjects = board.UpdateStatusAll();
            foreach (IDynamicObject obj in nonDeadObjects)
            {
                if (obj.SleepCount != 0)
                    obj.WakeUp();
            }
        }

        public void UpdateAlive()
        {
            board.UpdateStatusAll();
        }

        public void GenerateFood()
        {
            board.GenetareFood();
        }

        public void StartNewDay()
        {
            finishedTasksOfTheDay = 0;
            Console.WriteLine("Starting a new day!");
            clock.Start();

            foreach (AutoResetEvent are in ares.Values)
            {
                are.Set();
            }

            playing = true;
            long numberOfAnts = ares.Count;

            creatorTaskARE.Set();

            do { 
                mainThreadARE.WaitOne();
                numberOfAnts = ares.Count;
            }
            while (Interlocked.Read(ref finishedTasksOfTheDay) < Interlocked.Read(ref numberOfAnts));

            
            Console.WriteLine("The day has ended!");
            clock.Reset();
        }

        public int GetANumberOfAliveObjects()
        {
            List<IDynamicObject> l = board.GetAlive();
            return l.Count;
        }

        public void ActDepressedObject(IDynamicObject obj)
        {
            var dynamicObjects = board.GetNearObjects(obj.X, obj.Y);
            if (dynamicObjects.Count >= info.MinObjectsPerArea && dynamicObjects.Count <= info.MaxObjectsPerArea)
            {
                Console.WriteLine("Object number {0} is no longer depressed, found {1} objects near it", obj.Id, dynamicObjects.Count);
                obj.SetState(State.Alive);
            }
            else
            {
                Console.WriteLine("Object number {0} is still depressed, found {1} objects near it", obj.Id, dynamicObjects.Count);
            }
        }

        public void ActAliveObject(IDynamicObject obj)
        {
            string action = obj.DecideAction();
            if (action.Equals("Move"))
            {
                Move(obj);
            }
            else if (action.Equals("Fight"))
            {
                Console.WriteLine("Object number {0} is looking for a fight!", obj.Id);
                Fight(obj);
            }
        }

        public void Move(IDynamicObject obj)
        {
            int x, y;
            do
            {
                x = MyRandom.Next(Math.Max(0, obj.X - 1), Math.Min(info.Length, obj.X + 2));
                y = MyRandom.Next(Math.Max(0, obj.Y - 1), Math.Min(info.Hight, obj.Y + 2));
            } while (x == obj.X && y == obj.Y);

            Console.WriteLine("Object number {0} wants to move from position: {1},{2} to position {3},{4}", obj.Id, obj.X, obj.Y, x, y);
            int _x = obj.X;
            int _y = obj.Y;
            bool result = board.TryToMove(obj, x, y);
            if (result)
                Console.WriteLine("Object number {0} moved from {1},{2} to {3},{4}", obj.Id, _x, _y, x, y);
        }

        public void Fight(IDynamicObject obj)
        {
            Console.WriteLine("Object number {0} is looking for a fight!", obj.Id);
            var dynamicObjects = board.GetNearObjects(obj.X, obj.Y);

            if (dynamicObjects.Count == 0)
                return;
            int index = MyRandom.Next(0, dynamicObjects.Count);
            obj.Fight(dynamicObjects[index]);
        }

        private void DynamicObjectAction(object obj)
        {
            IDynamicObject myObject = (IDynamicObject)obj;
            ares.TryGetValue(myObject.Id, out AutoResetEvent are);
            are.WaitOne();
            while (myObject.State != State.Dead)
            {
                if (myObject.SleepCount == 0)
                {
                    switch (myObject.State)
                    {
                        case State.Alive:
                            ActAliveObject(myObject);
                            break;
                        case State.Depressed:
                            ActDepressedObject(myObject);
                            break;
                        case State.Dead:
                            break;
                    }
                    myObject.CalculateSleep(info.ObjectSleepDaysLow, info.ObjectSleepDaysHigh);
                    board.UpdateStatus(myObject);
                }
                Interlocked.Increment(ref finishedTasksOfTheDay);

                mainThreadARE.Set();

                are.WaitOne();
            }
            ares.TryRemove(myObject.Id, out are);
        }

        private void TryCreateObject()
        {
            int x = MyRandom.Next(0, info.Length);
            int y = MyRandom.Next(0, info.Hight);
            IDynamicObject obj = board.TryCreate(x, y);
            if (obj != null)
            {
                AutoResetEvent are = new AutoResetEvent(false);
                are.Set(); 
                ares.TryAdd(obj.Id, are);
                Task.Factory.StartNew(() => DynamicObjectAction(obj), TaskCreationOptions.LongRunning);
            }
        }

 
    }
}
