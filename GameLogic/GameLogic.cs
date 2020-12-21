using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Collections.Concurrent;
using Utils;
using AntLibrary;
using Tiles;
using FoodStaticObject;
using Interfaces;

namespace Logic
{
    public class GameLogic
    {
        private readonly Info info;
        private readonly Tile[,] tiles;
        private List<IDynamicObject> nonDeadObjects;
        public GameLogic(Info info)
        {
            this.info = info;
            tiles = new Tile[info.Length, info.Hight];
            for (int i = 0; i < info.Length; i++)
                for (int j = 0; j < info.Hight; j++)
                    tiles[i, j] = new Tile();
            nonDeadObjects = new List<IDynamicObject>();
        }

        public void GenerateInitialObjects()
        {
            int count = info.NumberOfObjects;
            if (count > info.Length * info.Hight)
                throw new ArgumentException(String.Format("The number of objects to create {0} is bigger than the size of the board {1}", count, info.Length * info.Hight));

            while (count > 0)
            {
                int x = MyRandom.Next(0, info.Length);
                int y = MyRandom.Next(0, info.Hight);
                if (tiles[x, y].DynamicObject == null)
                {
                    Ant ant = new Ant(info.ID, info.ObjectStartStrengthLow, info.ObjectStartStrengthHigh) { X = x, Y = y };
                    tiles[x, y].DynamicObject = ant;
                    nonDeadObjects.Add(ant);
                    info.ID++;
                    count--;
                }
            }
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
            var l = new List<IDynamicObject>();
            for(int i = 0; i < info.Length; i++)
                for(int j = 0; j < info.Hight; j++)
                {
                    var tile = tiles[i, j];
                    if(tile.DynamicObject != null)
                    {
                        tile.DynamicObject.AddStrength(-info.ObjectStrengthDecay);
                        if (tile.DynamicObject.Strength > 0)
                        {
                            l.Add(tile.DynamicObject);
                            tile.DynamicObject.Age++;
                        }
                        else
                        {
                            Console.WriteLine("Object number {0} died!", tile.DynamicObject.Id);
                            tile.DynamicObject = null;
                        }
                    }
                }
            nonDeadObjects = l;
        }

        public void GenerateFood()
        {
            for (int i = 0; i < info.FoodPerDay; i++)
            {
                int x = MyRandom.Next(0, info.Length);
                int y = MyRandom.Next(0, info.Hight);
                if (tiles[x, y].StaticObject == null)
                    if (tiles[x, y].DynamicObject != null)
                    {
                        tiles[x, y].DynamicObject.ActOnStaticObject(new Food());
                    }
                    else
                    {
                        tiles[x, y].StaticObject = new Food();
                    }
            }
        }

        public void StartNewDay()
        {
            Console.WriteLine("Starting a new day!");
            List<Task> tasks = new List<Task>();
            Stopwatch clock = new Stopwatch();
            clock.Start();
            foreach (IDynamicObject obj in nonDeadObjects)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    if (obj.SleepCount > 0)
                        return;
                    do
                    {
                        switch (obj.State)
                        {
                            case State.Alive:
                                ActAliveObject(obj);
                                break;
                            case State.Depressed:
                                ActDepressedObject(obj);
                                break;
                            case State.Dead:
                                break;
                        }
                        int count = 0;
                        for (int i = Math.Max(0, obj.X - 1); i <= Math.Min(info.Length - 1, obj.X + 1); i++)
                            for (int j = Math.Max(0, obj.Y - 1); j <= Math.Min(info.Hight - 1, obj.Y + 1); j++)
                            {
                                tiles[i, j].ReaderWriterLockSlim.EnterReadLock();
                                try
                                {
                                    if (tiles[i, j].DynamicObject != null && (obj.X != i || obj.Y != j))
                                        count++;
                                }
                                finally
                                {
                                    tiles[i, j].ReaderWriterLockSlim.ExitReadLock();
                                }
                            }
                        if((count < info.MinObjectsPerArea || count > info.MaxObjectsPerArea) && obj.State == State.Alive)
                        {
                            Console.WriteLine("Object number {0} became depressed!", obj.Id);
                            obj.SetState(State.Depressed);
                        }
                        Thread.Sleep(20);
                    } while (clock.ElapsedMilliseconds < info.TimePerDay);
                    obj.CalculateSleep(info.ObjectSleepDaysLow, info.ObjectSleepDaysHigh);
                }));
            }
            Task.WaitAll(tasks.ToArray());
            Console.WriteLine("The day has ended!");
        }

        public int GetANumberOfAliveObjects()
        {
            return nonDeadObjects.Count;
        }

        private void ActDepressedObject(IDynamicObject obj)
        {
            int count = 0;
            for (int i = Math.Max(0, obj.X - 1); i <= Math.Min(info.Length - 1, obj.X + 1); i++)
                for (int j = Math.Max(0, obj.Y - 1); j <= Math.Min(info.Hight - 1, obj.Y + 1); j++)
                {
                    tiles[i, j].ReaderWriterLockSlim.EnterReadLock();
                    try
                    {
                        if (tiles[i, j].DynamicObject != null && (obj.X != i || obj.Y != j))
                            count++;
                    }
                    finally
                    {
                        tiles[i, j].ReaderWriterLockSlim.ExitReadLock();
                    }
                }
            if (count >= info.MinObjectsPerArea && count <= info.MaxObjectsPerArea)
            {
                Console.WriteLine("Object number {0} is no longer depressed, found {1} objects near it", obj.Id, count);
                obj.SetState(State.Alive);
            }
            else
            {
                Console.WriteLine("Object number {0} is still depressed, found {1} objects near it", obj.Id, count);
            }
        }

        private void ActAliveObject(IDynamicObject obj)
        {
            string act = obj.DecideAction();
            if (act.Equals("Move"))
            {
                int x, y;
                do
                {
                    x = MyRandom.Next(Math.Max(0, obj.X - 1), Math.Min(info.Length, obj.X + 2));
                    y = MyRandom.Next(Math.Max(0, obj.Y - 1), Math.Min(info.Hight, obj.Y + 2));
                } while (x == obj.X && y == obj.Y);

                Console.WriteLine("Object number {0} wants to move from position: {1},{2} to position {3},{4}", obj.Id, obj.X, obj.Y, x, y);
                Tile tileToMove = tiles[x, y];
                Tile myTile = tiles[obj.X, obj.Y];
                tileToMove.ReaderWriterLockSlim.EnterUpgradeableReadLock();
                try
                {
                    if (tileToMove.DynamicObject == null)
                    {
                        tileToMove.ReaderWriterLockSlim.EnterWriteLock();
                        try
                        {
                            myTile.ReaderWriterLockSlim.EnterWriteLock();
                            try
                            {
                                Console.WriteLine("Object number {0} moved from {1},{2} to {3},{4}", obj.Id, obj.X, obj.Y, x, y);
                                tileToMove.DynamicObject = obj;
                                myTile.DynamicObject = null;
                                obj.X = x;
                                obj.Y = y;
                                if (tileToMove.StaticObject != null)
                                    obj.ActOnStaticObject(tileToMove.StaticObject);
                                return;
                            }
                            finally
                            {
                                myTile.ReaderWriterLockSlim.ExitWriteLock();
                            }
                        }
                        finally
                        {
                            tileToMove.ReaderWriterLockSlim.ExitWriteLock();
                        }
                    }
                }
                finally
                {
                    tileToMove.ReaderWriterLockSlim.ExitUpgradeableReadLock();
                }
            }
            else if (act.Equals("Fight"))
            {
                Console.WriteLine("Object number {0} is looking for a fight!", obj.Id);
                var ants = new List<IDynamicObject>();
                for (int i = Math.Max(0, obj.X - 1); i <= Math.Min(info.Length - 1, obj.X + 1); i++)
                    for (int j = Math.Max(0, obj.Y - 1); j <= Math.Min(info.Hight - 1, obj.Y + 1); j++)
                    {
                        tiles[i, j].ReaderWriterLockSlim.EnterReadLock();
                        try
                        {
                            if ((i != obj.X || j != obj.Y) && tiles[i, j].DynamicObject != null)
                            {
                                ants.Add(tiles[i, j].DynamicObject);
                            }
                        }
                        finally
                        {
                            tiles[i, j].ReaderWriterLockSlim.ExitReadLock();
                        }
                    }
                if (ants.Count == 0)
                    return;
                int index = MyRandom.Next(0, ants.Count);
                obj.Fight(ants[index]);

            }
        }

    }

}
