using System;
using System.Collections.Generic;
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
        private readonly Info info;
        private readonly Board board;
        private List<IDynamicObject> nonDeadObjects;
        private static long finishedTasksOfTheDay = 0;
        private readonly Stopwatch clock;

        public GameLogic()
        {
            info = Info.Instance;
            board = new Board(info.Length, info.Hight);
            nonDeadObjects = new List<IDynamicObject>();
            clock = new Stopwatch();
        }

        public void GenerateInitialObjects()
        {
            int count = info.NumberOfObjects;
            if (count > info.Length * info.Hight)
                throw new ArgumentException(String.Format("The number of objects to create {0} is bigger than the size of the board {1}", count, info.Length * info.Hight));
            nonDeadObjects = board.GenerateInitialObjects(count);
        }

        public void WakeUp()
        {
            foreach (IDynamicObject obj in nonDeadObjects)
            {
                if (obj.SleepCount != 0)
                    obj.WakeUp();
            }
        }

        public void UpdateAlive()
        {
            nonDeadObjects = board.UpdateAndGetAlive();
        }

        public void GenerateFood()
        {
            board.GenetareFood();
        }

        public void StartNewDay()
        {
            
            Console.WriteLine("Starting a new day!");
            List<Task> tasks = new List<Task>();
            Stopwatch clock = new Stopwatch();

            clock.Start();
            foreach (IDynamicObject obj in nonDeadObjects)
            {
                tasks.Add(Task.Factory.StartNew(DynamicObjectAction, obj));
            }
            Task.WaitAll(tasks.ToArray());
            Console.WriteLine("The day has ended!");
            clock.Reset();
        }

        public int GetANumberOfAliveObjects()
        {
            return nonDeadObjects.Count;
        }

        public void ActDepressedObject(IDynamicObject obj)
        {
            var dynamicObjects = board.GetNearObjects(obj);
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
            bool result = board.TryToMove(obj, x, y);
            if (result)
                Console.WriteLine("Object number {0} moved from {1},{2} to {3},{4}", obj.Id, obj.X, obj.Y, x, y);
        }

        public void Fight(IDynamicObject obj)
        {
            Console.WriteLine("Object number {0} is looking for a fight!", obj.Id);
            var dynamicObjects = board.GetNearObjects(obj);

            if (dynamicObjects.Count == 0)
                return;
            int index = MyRandom.Next(0, dynamicObjects.Count);
            obj.Fight(dynamicObjects[index]);
        }

        private void DynamicObjectAction(object obj)
        {
            IDynamicObject myObject = (IDynamicObject)obj;
            if (myObject.SleepCount > 0)
                return;
            do
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
                board.UpdateStatus(myObject);
            } while (clock.ElapsedMilliseconds < info.TimePerDay);
            myObject.CalculateSleep(info.ObjectSleepDaysLow, info.ObjectSleepDaysHigh);
        }
    }
}
