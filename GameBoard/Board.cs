using System;
using System.Collections.Generic;
using Tiles;
using Utils;
using DynamicObjects;
using StaticObjects;
using ProducerConsumer;

namespace MyBoard
{
    public class Board : IBoard
    {
        private ITile[,] tiles;
        private IInfo info;
        private IRandomTest rnd;
        private RandomOption randomOption = RandomOption.RealTime;
        private IProducerConsumerMessages<string> producerConsumer;


        public Board() { }
        //for testing purposes
        public void TestBoard(ITile[,] tiles, IInfo info, IRandomTest rnd, IProducerConsumerMessages<string> producerConsumer)
        {
            this.info = info;
            this.tiles = tiles;
            this.rnd = rnd;
            randomOption = RandomOption.Testing;
            this.producerConsumer = producerConsumer;
        }


        public Board(int length, int hight, IProducerConsumerMessages<string> producerConsumer)
        {
            info = Info.Instance;
            this.producerConsumer = producerConsumer;
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
                var t = GetRandomPosition();
                int x = t.Item1, y = t.Item2;

                // create a IDynamicObject at (x,y) if there isn't one already.
                if (tiles[x, y].DynamicObject == null)
                {
                    IDynamicObject ant = new Ant(info.ID, info.ObjectStartStrengthLow, info.ObjectStartStrengthHigh) { X = x, Y = y, ProducerConsumer = producerConsumer };
                    tiles[x, y].DynamicObject = ant;
                    count--;
                    l.Add(ant);
                }
            }
            return l;
        }

        public List<IDynamicObject> UpdateStatusAll()
        {
            // get all the alive objects.
            var l = GetAlive();
            List<IDynamicObject> antsAlive = new List<IDynamicObject>();

            /// for each object alive we calculate the strength of the object.
            /// if the strength is 0 ore below, we set the state as State.Dead and we remove it from the tile.
            /// we return only the non-dead objects on the board.
            foreach (IDynamicObject obj in l)
            {
                var tile = tiles[obj.X, obj.Y];
                obj.AddStrength(-info.ObjectStrengthDecay);
                if (obj.Strength > 0)
                {
                    obj.Age++;
                    antsAlive.Add(obj);
                }
                else
                {
                    producerConsumer.Produce(String.Format("Object number {0} died!", obj.Id));
                    obj.SetState(State.Dead);
                    tile.DynamicObject = null;
                }

            }
            return antsAlive;
        }

        public void GenetareFood()
        {
            // tries to genenarte StaticObjects on the board
            for (int i = 0; i < info.FoodPerDay; i++)
            {
                var t = GetRandomPosition();
                int x = t.Item1, y = t.Item2;
                if (tiles[x, y].StaticObject == null)
                    // if there is already a dynamic object, generate food and act on it directly.
                    if (tiles[x, y].DynamicObject != null)
                    {
                        tiles[x, y].DynamicObject.ActOnStaticObject(new Food());
                    }
                    // otherwise, put the generated food on the tile.
                    else
                    {
                        tiles[x, y].StaticObject = new Food();
                    }
            }
        }


        public void UpdateStatus(IDynamicObject obj)
        {
            // count is the number of objects around the given object.
            int count = 0;
            // loop around the given object position.
            for (int i = Math.Max(0, obj.X - 1); i <= Math.Min(info.Length - 1, obj.X + 1); i++)
                for (int j = Math.Max(0, obj.Y - 1); j <= Math.Min(info.Hight - 1, obj.Y + 1); j++)
                {
                    // get the lock of the tile and enter with read mode.
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
            /// if the count is not in the boundries given by the <see cref="Info"/> class,
            /// the object's state will become <see cref="State.Depressed"/>.
            if ((count < info.MinObjectsPerArea || count > info.MaxObjectsPerArea) && obj.State == State.Alive)
            {
                producerConsumer.Produce(String.Format("Object number {0} became depressed!", obj.Id));
                obj.SetState(State.Depressed);
            }
        }


        public bool TryToMove(IDynamicObject obj, int x, int y)
        {
            if (x >= info.Length || x < 0 || y >= info.Hight || y < 0)
                throw new ArgumentException("Trying to move outside of the board");

            // catch both the locks at (x,y) and the object position in upgradable mode.
            ITile tileToMove = tiles[x, y];
            ITile myTile = tiles[obj.X, obj.Y];
            myTile.Lock.EnterUpgradeableReadLock();
            tileToMove.Lock.EnterUpgradeableReadLock();
            try
            {
                // if the tile at (x,y) is empty
                if (tileToMove.DynamicObject == null)
                {
                    // upgrade the locks to write mode
                    myTile.Lock.EnterWriteLock();
                    tileToMove.Lock.EnterWriteLock();

                    try
                    {
                        // move the object to the position (x,y)
                        tileToMove.DynamicObject = obj;
                        myTile.DynamicObject = null;
                        obj.X = x;
                        obj.Y = y;

                        // if there is a static object at (x,y)
                        if (tileToMove.StaticObject != null)
                        {
                            tileToMove.StaticObject.ActOnDynamicObject(obj);
                            tileToMove.StaticObject = null;
                        }
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
            // loop around position (x,y)
            for (int i = Math.Max(0, x - 1); i <= Math.Min(info.Length - 1, x + 1); i++)
                for (int j = Math.Max(0, y - 1); j <= Math.Min(info.Hight - 1, y + 1); j++)
                {
                    if (i != x || j != y)
                    {
                        // catch the lock at (i,j) with read mode
                        tiles[i, j].Lock.EnterReadLock();
                        try
                        {
                            if (tiles[i, j].DynamicObject != null)
                                objects.Add(tiles[i, j].DynamicObject);
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

            ITile tile = tiles[x, y];

            //catch the lock at (x,y) with upgradable mode
            tile.Lock.EnterUpgradeableReadLock();
            try
            {
                // first we check if there is an dynamic object at (x,y) already.
                if (tile.DynamicObject == null)
                {
                    // if there isn't an object we get all the dynamic objects around it
                    var ants = GetNearObjects(x, y);

                    // only if there isn't any objects around (x,y) we create a new dynamic object there.
                    if (ants.Count == 0)
                    {
                        // we upgrade the lock to write mode
                        tile.Lock.EnterWriteLock();
                        try
                        {
                            // creates a new and and return it.
                            IDynamicObject newAnt = new Ant(info.ID, info.ObjectStartStrengthLow, info.ObjectStartStrengthHigh) { X = x, Y = y, ProducerConsumer = producerConsumer };
                            producerConsumer.Produce(String.Format("Id: {2} was created at position {0},{1}", x, y, newAnt.Id));
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

            // loop around the board
            for (int i = 0; i < info.Length; i++)
                for (int j = 0; j < info.Hight; j++)
                {
                    // if there is a dymanic object at (i,j), and it's not dead than add it to the list.
                    if (tiles[i, j].DynamicObject != null && tiles[i, j].DynamicObject.State != State.Dead)
                        l.Add(tiles[i, j].DynamicObject);
                }
            return l;
        }

        public Tuple<int, int> GetRandomPosition()
        {
            int x = 0, y = 0;
            switch (randomOption)
            {
                case RandomOption.RealTime:
                    // get a random position on the board
                    x = MyRandom.Next(0, info.Length);
                    y = MyRandom.Next(0, info.Hight);
                    break;
                case RandomOption.Testing:
                    x = rnd.Next(0, info.Length);
                    y = rnd.Next(0, info.Hight);
                    break;
            }
            return new Tuple<int, int>(x, y);
        }
    }

    public enum RandomOption { RealTime, Testing };
}
