using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Tiles;
using Utils;
using DynamicObjects;
using StaticObjects;

namespace MyBoard
{
    public class Board
    {
        private readonly Tile[,] tiles;
        private readonly Info info;

        public Board(int length, int hight)
        {
            info = Info.Instance;
            tiles = new Tile[length, hight];
            for (int i = 0; i < length; i++)
                for (int j = 0; j < hight; j++)
                    tiles[i, j] = new Tile();
        }

        public List<IDynamicObject> GenerateInitialObjects(int count)
        {
            List<IDynamicObject> l = new List<IDynamicObject>();
            while (count > 0)
            {
                int x = MyRandom.Next(0, info.Length);
                int y = MyRandom.Next(0, info.Hight);
                if (tiles[x, y].DynamicObject == null)
                {
                    Ant ant = new Ant(info.ID, info.ObjectStartStrengthLow, info.ObjectStartStrengthHigh) { X = x, Y = y };
                    tiles[x, y].DynamicObject = ant;
                    info.ID++;
                    count--;
                    l.Add(ant);
                }
            }
            return l;
        }

        public List<IDynamicObject> UpdateAndGetAlive()
        {
            var l = new List<IDynamicObject>();
            for (int i = 0; i < info.Length; i++)
                for (int j = 0; j < info.Hight; j++)
                {
                    var tile = tiles[i, j];
                    if (tile.DynamicObject != null)
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
            return l;
        }

        public void GenetareFood()
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

        public void UpdateStatus(IDynamicObject obj)
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
            if ((count < info.MinObjectsPerArea || count > info.MaxObjectsPerArea) && obj.State == State.Alive)
            {
                Console.WriteLine("Object number {0} became depressed!", obj.Id);
                obj.SetState(State.Depressed);
            }
        }

        public void Release(int i, int j)
        {
            var _lock = tiles[i, j].ReaderWriterLockSlim;
            if (_lock.IsReadLockHeld)
                _lock.ExitReadLock();
            else if (_lock.IsUpgradeableReadLockHeld)
                _lock.ExitUpgradeableReadLock();
            else if (_lock.IsWriteLockHeld)
                _lock.ExitWriteLock();
            else throw new SynchronizationLockException("Trying to release the lock without helding it first");
        }

        public bool TryToMove(IDynamicObject obj, int x, int y)
        {

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
                            tileToMove.DynamicObject = obj;
                            myTile.DynamicObject = null;
                            obj.X = x;
                            obj.Y = y;
                            if (tileToMove.StaticObject != null)
                                tileToMove.StaticObject.ActOnDynamicObject(obj);
                            return true;
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
            return false;
        }

        public List<IDynamicObject> GetNearObjects(IDynamicObject obj)
        {
            List<IDynamicObject> objects = new List<IDynamicObject>();
            for (int i = Math.Max(0, obj.X - 1); i <= Math.Min(info.Length - 1, obj.X + 1); i++)
                for (int j = Math.Max(0, obj.Y - 1); j <= Math.Min(info.Hight - 1, obj.Y + 1); j++)
                {
                    tiles[i, j].ReaderWriterLockSlim.EnterReadLock();
                    try
                    {
                        if ((i != obj.X || j != obj.Y) && tiles[i, j].DynamicObject != null)
                        {
                            objects.Add(tiles[i, j].DynamicObject);
                        }
                    }
                    finally
                    {
                        tiles[i, j].ReaderWriterLockSlim.ExitReadLock();
                    }
                }
            return objects;
        }
    }
}
