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
    public class Board : IBoard
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
                    IDynamicObject ant = new Ant(info.ID, info.ObjectStartStrengthLow, info.ObjectStartStrengthHigh) { X = x, Y = y };
                    tiles[x, y].DynamicObject = ant;
                    count--;
                    l.Add(ant);
                }
            }
            return l;
        }

        public List<IDynamicObject> UpdateStatusAll()
        {
            var l = GetAlive();
            List<IDynamicObject> antsAlive = new List<IDynamicObject>();
            foreach (IDynamicObject obj in l)
            {
                var tile = tiles[obj.X, obj.Y];
                tile.Lock.EnterWriteLock();
                try
                {
                    obj.AddStrength(-info.ObjectStrengthDecay);
                    if (obj.Strength > 0)
                    {
                        obj.Age++;
                        antsAlive.Add(obj);
                    }
                    else
                    {
                        Console.WriteLine("Object number {0} died!", obj.Id);
                        obj.SetState(State.Dead);
                        tile.DynamicObject = null;
                    }
                }
                finally { tile.Lock.ExitWriteLock(); }
            }
            return antsAlive;
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
                    tiles[i, j].Lock.EnterReadLock();
                    try
                    {
                        if (tiles[i, j].DynamicObject != null && (obj.X != i || obj.Y != j))
                            count++;
                    }
                    finally
                    {
                        tiles[i, j].Lock.ExitReadLock();
                    }
                }
            if ((count < info.MinObjectsPerArea || count > info.MaxObjectsPerArea) && obj.State == State.Alive)
            {
                Console.WriteLine("Object number {0} became depressed!", obj.Id);
                obj.SetState(State.Depressed);
            }
        }

        public bool TryToMove(IDynamicObject obj, int x, int y)
        {
            if (x >= info.Length || x < 0 || y >= info.Hight || y < 0)
                throw new ArgumentException("Trying to move outside of the board");
            Tile tileToMove = tiles[x, y];
            Tile myTile = tiles[obj.X, obj.Y];
            myTile.Lock.EnterUpgradeableReadLock();
            tileToMove.Lock.EnterUpgradeableReadLock();
            try
            {
                if (tileToMove.DynamicObject == null)
                {
                    myTile.Lock.EnterWriteLock();
                    tileToMove.Lock.EnterWriteLock();

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
                        myTile.Lock.ExitWriteLock();
                        tileToMove.Lock.ExitWriteLock();
                    }
                }
                else return false;
            }
            finally
            {
                myTile.Lock.ExitUpgradeableReadLock();
                tileToMove.Lock.ExitUpgradeableReadLock();
            }
        }

        public List<IDynamicObject> GetNearObjects(int x, int y)
        {
            List<IDynamicObject> objects = new List<IDynamicObject>();
            for (int i = Math.Max(0, x - 1); i <= Math.Min(info.Length - 1, x + 1); i++)
                for (int j = Math.Max(0, y - 1); j <= Math.Min(info.Hight - 1, y + 1); j++)
                {
                    if (i != x || j != y)
                    {
                        tiles[i, j].Lock.EnterReadLock();
                        try
                        {
                            var obj = tiles[i, j].DynamicObject;
                            if (obj != null)
                                objects.Add(obj);
                        }
                        finally
                        {
                            tiles[i, j].Lock.ExitReadLock();
                        }
                    }

                }
            return objects;
        }

        public IDynamicObject TryCreate(int x, int y)
        {
            if (x >= info.Length || x < 0 || y >= info.Hight || y < 0)
                throw new ArgumentException("Trying to move outside of the board");

            Tile tile = tiles[x, y];

            tile.Lock.EnterUpgradeableReadLock();
            try
            {
                if (tile.DynamicObject == null)
                {
                    var ants = GetNearObjects(x, y);
                    if (ants.Count == 0)
                    {
                        tile.Lock.EnterWriteLock();
                        try
                        {
                            IDynamicObject newAnt = new Ant(info.ID, info.ObjectStartStrengthLow, info.ObjectStartStrengthHigh) { X = x, Y = y };
                            Console.WriteLine("Id: {2} was created at position {0},{1}", x, y, newAnt.Id);
                            tile.DynamicObject = newAnt;
                            return newAnt;
                        }
                        finally { tile.Lock.ExitWriteLock(); }
                    }
                    else return null;
                }
                else return null;
            }
            finally
            {
                tile.Lock.ExitUpgradeableReadLock();
            }
        }

        public List<IDynamicObject> GetAlive()
        {
            List<IDynamicObject> l = new List<IDynamicObject>();
            for(int i = 0; i < info.Length; i++)
                for(int j = 0; j < info.Hight; j++)
                {
                    tiles[i, j].Lock.EnterReadLock();
                    try
                    {
                        if (tiles[i, j].DynamicObject != null && tiles[i, j].DynamicObject.State != State.Dead)
                            l.Add(tiles[i, j].DynamicObject);
                    }
                    finally
                    {
                        tiles[i, j].Lock.ExitReadLock();
                    }
                }
            return l;
        }
    }
}
