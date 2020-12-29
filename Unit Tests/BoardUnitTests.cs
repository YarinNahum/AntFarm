using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyBoard;
using Utils;
using Tiles;
using DynamicObjects;
using ProducerConsumer;
using FakeItEasy;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Threading.Tasks;

namespace Unit_Tests
{
    [TestClass]
    public class BoardUnitTests
    {
        [TestMethod]
        public void Test_GenerateInitialObjects()
        {
            //initialize
            IInfo info = A.Fake<IInfo>();
            IRandomTest rnd = A.Fake<IRandomTest>();
            IProducerConsumerMessages<string> producerConsumer = A.Fake<IProducerConsumerMessages<string>>();

            int count = 0;
            int[] values = new int[50];
            Tile[,] tiles = new Tile[5, 5];


            A.CallTo(() => info.Length).Returns(5);
            A.CallTo(() => info.Hight).Returns(5);
            A.CallTo(() => info.ObjectStartStrengthHigh).Returns(2);
            A.CallTo(() => info.ObjectStartStrengthLow).Returns(1);
            A.CallTo(() => info.ID).Returns(0);
            A.CallTo(() => producerConsumer.Produce(A<string>.Ignored));

            for (int i = 0; i < info.Length; i++)
                for (int j = 0; j < info.Hight; j++)
                {
                    if (count < 50)
                    {
                        values[count] = i;
                        count++;
                        values[count] = j;
                        count++;
                    }
                    tiles[i, j] = new Tile();
                }

            A.CallTo(() => rnd.Next(A<int>.Ignored, A<int>.Ignored)).ReturnsNextFromSequence(values);

            IBoard board = new Board();

            board.TestBoard(tiles, info, rnd, producerConsumer);
            
            board.GenerateInitialObjects(10);
            //assert
            count = 0;
            for (int i = 0; i < info.Length; i++)
                for (int j = 0; j < info.Hight; j++)
                    if (tiles[i, j].DynamicObject != null)
                        count++;
            Assert.AreEqual(10, count);

        }

        [TestMethod]
        public void Test_GenetareFood()
        {
            //initialize
            IInfo info = A.Fake<IInfo>();
            IRandomTest rnd = A.Fake<IRandomTest>();
            IProducerConsumerMessages<string> producerConsumer = A.Fake<IProducerConsumerMessages<string>>();

            int count = 0;
            int[] values = new int[50];
            Tile[,] tiles = new Tile[5, 5];


            A.CallTo(() => info.Length).Returns(5);
            A.CallTo(() => info.Hight).Returns(5);
            A.CallTo(() => info.ID).Returns(0);
            A.CallTo(() => producerConsumer.Produce(A<string>.Ignored));
            A.CallTo(() => rnd.Next(A<int>.Ignored, A<int>.Ignored)).ReturnsNextFromSequence(values);
            A.CallTo(() => info.FoodPerDay).Returns(10);

            for (int i = 0; i < info.Length; i++)
                for (int j = 0; j < info.Hight; j++)
                {
                    if (count < 50)
                    {
                        values[count] = i;
                        count++;
                        values[count] = j;
                        count++;
                    }
                    tiles[i, j] = new Tile();
                }



            IBoard board = new Board();
            board.TestBoard(tiles, info, rnd, producerConsumer);
            board.GenetareFood();

            count = 0;
            for (int i = 0; i < info.Length; i++)
                for (int j = 0; j < info.Hight; j++)
                    if (tiles[i, j].StaticObject != null)
                        count++;

            bool ans = count <= 10 && count >= 1;

            Assert.AreEqual(true, ans);
        }

        [TestMethod]
        public void Test_UpdateStatus()
        {
            //initialize
            IInfo info = A.Fake<IInfo>();
            IRandomTest rnd = A.Fake<IRandomTest>();
            IProducerConsumerMessages<string> producerConsumer = A.Fake<IProducerConsumerMessages<string>>();

            IDynamicObject obj = A.Fake<IDynamicObject>();
            A.CallTo(() => obj.X).Returns(1);
            A.CallTo(() => obj.Y).Returns(1);
            A.CallTo(() => obj.State).Returns(State.Alive);

            int[] values = new int[50];
            Tile[,] tiles = new Tile[3, 3];

            for (int i = 0; i < 3; i++)
                for (int j = 0; j <3; j++)
                {
                    tiles[i, j] = new Tile();
                }


            var positions =new List<(int, int)> { (0, 0), (1, 0), (2, 1) };
            foreach (var t in positions)
            {
                int x = t.Item1;
                int y = t.Item2;
                IDynamicObject fakeObj = A.Fake<IDynamicObject>();
                A.CallTo(() => fakeObj.X).ReturnsLazily(() => x);
                A.CallTo(() => fakeObj.X).ReturnsLazily(() => y);
                tiles[x, y].DynamicObject = fakeObj;
            }


            A.CallTo(() => info.Length).Returns(3);
            A.CallTo(() => info.Hight).Returns(3);
            A.CallTo(() => info.MinObjectsPerArea).Returns(1);
            A.CallTo(() => info.MaxObjectsPerArea).Returns(2);
            A.CallTo(() => info.ID).Returns(0);
            A.CallTo(() => producerConsumer.Produce(A<string>.Ignored));

            //Act
            IBoard board = new Board();
            board.TestBoard(tiles, info, rnd, producerConsumer);

            board.UpdateStatus(obj);
            //Assert
            A.CallTo(() => obj.SetState(State.Depressed)).MustHaveHappened();
        }

        [TestMethod]

        public void Test_UpdateStatusAll()
        {
            IInfo info = A.Fake<IInfo>();
            IRandomTest rnd = A.Fake<IRandomTest>();
            IProducerConsumerMessages<string> producerConsumer = A.Fake<IProducerConsumerMessages<string>>();

            int[] values = new int[50];
            ITile[,] tiles = new ITile[3, 3];

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    ITile tile = A.Fake<ITile>();
                    tiles[i, j] = tile;
                }


            var positions = new List<(int, int)> { (0, 0), (1, 0), (2, 1) };
            foreach (var t in positions)
            {
                int x = t.Item1;
                int y = t.Item2;
                IDynamicObject fakeObj = A.Fake<IDynamicObject>();
                A.CallTo(() => fakeObj.X).ReturnsLazily(() => x);
                A.CallTo(() => fakeObj.X).ReturnsLazily(() => y);
                A.CallTo(() => fakeObj.Strength).Returns(2);
                A.CallTo(() => tiles[x, y].DynamicObject).ReturnsLazily(()=>fakeObj);
            }

            var obj = A.Fake<IDynamicObject>();
            int strength = 1;
            A.CallTo(() => obj.Strength).ReturnsLazily(()=>strength);
            A.CallTo(() => obj.X).Returns(2);
            A.CallTo(() => obj.Y).Returns(2);
            A.CallTo(() => tiles[2, 2].DynamicObject).ReturnsLazily(() => obj);
            A.CallTo(() => obj.AddStrength(-1)).Invokes(() => strength--);


            A.CallTo(() => info.Length).Returns(3);
            A.CallTo(() => info.Hight).Returns(3);
            A.CallTo(() => info.ObjectStrengthDecay).Returns(1);


            IBoard board = new Board();
            board.TestBoard(tiles, info, rnd, producerConsumer);

            var l = board.UpdateStatusAll();

            Assert.AreEqual(3, l.Count);
            A.CallTo(() => obj.SetState(State.Dead)).MustHaveHappened();
        }

        [TestMethod]

        public void Test_GetNearObjects()
        {
            //initialize
            IInfo info = A.Fake<IInfo>();
            IRandomTest rnd = A.Fake<IRandomTest>();
            IProducerConsumerMessages<string> producerConsumer = A.Fake<IProducerConsumerMessages<string>>();

            int[] values = new int[50];
            ITile[,] tiles = new ITile[4, 4];

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    ITile tile = A.Fake<ITile>();
                    tiles[i, j] = tile;
                    tile.DynamicObject = null;
                    ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();
                    A.CallTo(() => tile.Lock).ReturnsLazily(() => readerWriterLock);
                }

            var positions = new List<(int, int)> { (0, 0), (1, 0), (2, 1), (3, 3) };
            foreach (var t in positions)
            {
                int x = t.Item1;
                int y = t.Item2;
                IDynamicObject fakeObj = A.Fake<IDynamicObject>();
                A.CallTo(() => fakeObj.X).ReturnsLazily(() => x);
                A.CallTo(() => fakeObj.X).ReturnsLazily(() => y);
                A.CallTo(() => fakeObj.Strength).Returns(2);
                A.CallTo(() => tiles[x, y].DynamicObject).ReturnsLazily(() => fakeObj);
            }

            var obj = A.Fake<IDynamicObject>();
            A.CallTo(() => obj.X).Returns(1);
            A.CallTo(() => obj.Y).Returns(1);
            A.CallTo(() => tiles[1, 1].DynamicObject).ReturnsLazily(() => obj);

            A.CallTo(() => info.Length).Returns(4);
            A.CallTo(() => info.Hight).Returns(4);

            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            

            IBoard board = new Board();
            board.TestBoard(tiles, info, rnd, producerConsumer);

            //act
            Thread thr1 = new Thread(() => { tiles[2, 1].Lock.EnterWriteLock(); Console.WriteLine("enter lock"); autoResetEvent.Set();  Thread.Sleep(100); tiles[2, 1].Lock.ExitWriteLock(); Console.WriteLine("exit lock"); });
            thr1.Start();

            autoResetEvent.WaitOne();
            Console.WriteLine("called funct");

            List<IDynamicObject> l = board.GetNearObjects(obj.X,obj.Y);
            
            //assert
            Assert.AreEqual(3, l.Count);
        }

        [TestMethod]

        public void Test_TryCreate()
        {
            //initialize
            IInfo info = A.Fake<IInfo>();
            IRandomTest rnd = A.Fake<IRandomTest>();
            IProducerConsumerMessages<string> producerConsumer = A.Fake<IProducerConsumerMessages<string>>();

            int length = 4;
            int hight = 4;
            int[] values = new int[50];
            ITile[,] tiles = new ITile[length,hight];

            for (int i = 0; i < length; i++)
                for (int j = 0; j < hight; j++)
                {
                    ITile tile = A.Fake<ITile>();
                    tiles[i, j] = tile;
                    tile.DynamicObject = null;
                    ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();
                    A.CallTo(() => tile.Lock).ReturnsLazily(() => readerWriterLock);
                }

            var positions = new List<(int, int)> { (0, 0)};
            foreach (var t in positions)
            {
                int x = t.Item1;
                int y = t.Item2;
                IDynamicObject fakeObj = A.Fake<IDynamicObject>();
                A.CallTo(() => fakeObj.X).ReturnsLazily(() => x);
                A.CallTo(() => fakeObj.X).ReturnsLazily(() => y);
                A.CallTo(() => fakeObj.Strength).Returns(2);
                A.CallTo(() => tiles[x, y].DynamicObject).ReturnsLazily(() => fakeObj);
            }


            A.CallTo(() => info.Length).Returns(length);
            A.CallTo(() => info.Hight).Returns(hight);

            IBoard board = new Board();
            board.TestBoard(tiles, info, rnd, producerConsumer);

            //act
            bool ans1=false, ans2=false;
            Thread t1 = new Thread(() => ans1 = board.TryCreate(1, 2) == null);
            Thread t2 = new Thread(() => ans2 = board.TryCreate(2, 2) == null);

            t1.Start(); t2.Start();
            t1.Join(); t2.Join();

            //Assert, only one object should be created
            Assert.AreEqual(true, ans1 ^ ans2);
        }

        [TestMethod]
        public void Test_GetAlive()
        {
            //initialize
            IInfo info = A.Fake<IInfo>();
            IRandomTest rnd = A.Fake<IRandomTest>();
            IProducerConsumerMessages<string> producerConsumer = A.Fake<IProducerConsumerMessages<string>>();

            int length = 4;
            int hight = 4;
            int[] values = new int[50];
            ITile[,] tiles = new ITile[length, hight];

            for (int i = 0; i < length; i++)
                for (int j = 0; j < hight; j++)
                {
                    ITile tile = A.Fake<ITile>();
                    tiles[i, j] = tile;
                    tile.DynamicObject = null;
                    ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();
                    A.CallTo(() => tile.Lock).ReturnsLazily(() => readerWriterLock);
                }

            var positions = new List<(int, int)> { (0, 0), (1,0), (0,1), (2,2), (3,1), (3,3)};
            foreach (var t in positions)
            {
                int x = t.Item1;
                int y = t.Item2;
                IDynamicObject fakeObj = A.Fake<IDynamicObject>();
                A.CallTo(() => fakeObj.X).ReturnsLazily(() => x);
                A.CallTo(() => fakeObj.X).ReturnsLazily(() => y);
                A.CallTo(() => fakeObj.Strength).Returns(2);
                A.CallTo(() => tiles[x, y].DynamicObject).ReturnsLazily(() => fakeObj);
            }


            A.CallTo(() => info.Length).Returns(length);
            A.CallTo(() => info.Hight).Returns(hight);

            IBoard board = new Board();
            board.TestBoard(tiles, info, rnd, producerConsumer);

            var l = board.GetAlive();

            Assert.AreEqual(positions.Count, l.Count);
        }

        [TestMethod]

        public void Test_TryMove_Both_Should_Fail()
        {
            IInfo info = A.Fake<IInfo>();
            IRandomTest rnd = A.Fake<IRandomTest>();
            IProducerConsumerMessages<string> producerConsumer = A.Fake<IProducerConsumerMessages<string>>();

            int length = 4;
            int hight = 4;
            int[] values = new int[50];
            ITile[,] tiles = new ITile[length, hight];

            for (int i = 0; i < length; i++)
                for (int j = 0; j < hight; j++)
                {
                    ITile tile = A.Fake<ITile>();
                    tiles[i, j] = tile;
                    tile.DynamicObject = null;
                    ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();
                    A.CallTo(() => tile.Lock).ReturnsLazily(() => readerWriterLock);
                }

            var positions = new List<(int, int)> { (0, 0), (1,1)};
            foreach (var t in positions)
            {
                int x = t.Item1;
                int y = t.Item2;
                IDynamicObject fakeObj = A.Fake<IDynamicObject>();
                A.CallTo(() => fakeObj.X).ReturnsLazily(() => x);
                A.CallTo(() => fakeObj.X).ReturnsLazily(() => y);
                A.CallTo(() => tiles[x, y].DynamicObject).ReturnsLazily(() => fakeObj);
                A.CallTo(() => tiles[x, y].StaticObject).Returns(null);
            }


            A.CallTo(() => info.Length).Returns(length);
            A.CallTo(() => info.Hight).Returns(hight);

            IBoard board = new Board();
            board.TestBoard(tiles, info, rnd, producerConsumer);

            //act
            bool ans1 = false, ans2 = false;
            Thread t1 = new Thread(() => ans1 = board.TryToMove(tiles[0,0].DynamicObject,1, 1) );
            Thread t2 = new Thread(() => ans2 = board.TryToMove(tiles[1, 1].DynamicObject,0, 0));

            t1.Start(); t2.Start();
            t1.Join(); t2.Join();

            //Assert, both sould fail
            Assert.AreEqual(false, ans1);
            Assert.AreEqual(false, ans2);
        }

        [TestMethod]
        public void Test_TryMove_Only_Should_One_Succeed()
        {
            IInfo info = A.Fake<IInfo>();
            IRandomTest rnd = A.Fake<IRandomTest>();
            IProducerConsumerMessages<string> producerConsumer = A.Fake<IProducerConsumerMessages<string>>();

            int length = 4;
            int hight = 4;
            int[] values = new int[50];
            ITile[,] tiles = new ITile[length, hight];

            for (int i = 0; i < length; i++)
                for (int j = 0; j < hight; j++)
                {
                    ITile tile = A.Fake<ITile>();
                    tiles[i, j] = tile;
                    tile.DynamicObject = null;
                    ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();
                    A.CallTo(() => tile.Lock).ReturnsLazily(() => readerWriterLock);
                }

            var positions = new List<(int, int)> { (0, 0), (1, 1) };
            foreach (var t in positions)
            {
                int x = t.Item1;
                int y = t.Item2;
                IDynamicObject fakeObj = A.Fake<IDynamicObject>();
                A.CallTo(() => fakeObj.X).ReturnsLazily(() => x);
                A.CallTo(() => fakeObj.X).ReturnsLazily(() => y);
                A.CallTo(() => tiles[x, y].DynamicObject).ReturnsLazily(() => fakeObj);
                A.CallTo(() => tiles[x, y].StaticObject).Returns(null);
            }


            A.CallTo(() => info.Length).Returns(length);
            A.CallTo(() => info.Hight).Returns(hight);

            IBoard board = new Board();
            board.TestBoard(tiles, info, rnd, producerConsumer);

            //act
            bool ans1 = false, ans2 = false;
            Thread t1 = new Thread(() => ans1 = board.TryToMove(tiles[0, 0].DynamicObject, 0, 1));
            Thread t2 = new Thread(() => ans2 = board.TryToMove(tiles[1, 1].DynamicObject, 0, 1));

            t1.Start(); t2.Start();
            t1.Join(); t2.Join();

            //Assert, both sould fail
            Assert.AreEqual(true, ans1 ^ ans2);
            
        }
    }
}
