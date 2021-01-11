using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using FakeItEasy;
using GameLogicNameSpace;
using IDynamicObjects;
using Tiles;
using Utils;
using BoardNamespace;
using ProducerConsumer;


namespace GameLogicUnitTests
{
    [TestClass]
    public class LogicUnitTests
    {
        [TestMethod]
        public void Test_GenerateInitialObjects()
        {
            IInfo info = A.Fake<IInfo>();
            IBoard board = A.Fake<IBoard>();
            IRandomTest rnd = A.Fake<IRandomTest>();

            IGameLogic gameLogic = new GameLogic();
            gameLogic.GameLogicTest(info, board, rnd);

            List<IDynamicObject> l = new List<IDynamicObject>();

            int count = 10;
            A.CallTo(() => info.Length).Returns(5);
            A.CallTo(() => info.Hight).Returns(5);
            A.CallTo(() => info.NumberOfObjects).Returns(count);
            for (int i = 0; i < count; i++)
            {
                var obj = A.Fake<IDynamicObject>();
                A.CallTo(() => obj.Id).ReturnsLazily(() => i);
                l.Add(obj);
            }

            A.CallTo(() => board.GenerateInitialObjects(count)).ReturnsLazily(() => l);
            A.CallTo(() => board.GetAlive()).ReturnsLazily(() => l);

            gameLogic.GenerateInitialObjects();

            Assert.AreEqual(count, gameLogic.GetNumberOfAliveObjects());
        }

        [TestMethod]

        public void Test_WakeUp()
        {

            IInfo info = A.Fake<IInfo>();
            IBoard board = A.Fake<IBoard>();
            IRandomTest rnd = A.Fake<IRandomTest>();

            GameLogic gameLogic = new GameLogic();
            gameLogic.GameLogicTest(info, board, rnd);

            List<IDynamicObject> l = new List<IDynamicObject>();

            int count = 10;
            for (int i = 0; i < count; i++)
            {
                var obj = A.Fake<IDynamicObject>();
                A.CallTo(() => obj.SleepCount).Returns(1);
                l.Add(obj);
            }

            A.CallTo(() => board.GetAlive()).ReturnsLazily(() => l);

            gameLogic.WakeUp();

            foreach (var obj in l)
                A.CallTo(() => obj.WakeUp()).MustHaveHappenedOnceExactly();
        }

        [TestMethod]

        public void Test_UpdateStatusAll()
        {
            IInfo info = A.Fake<IInfo>();
            IRandomTest rnd = A.Fake<IRandomTest>();
            IBoard board = A.Fake<IBoard>();

            int[] values = new int[50];
            ITile[,] tiles = new ITile[3, 3];

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    ITile tile = A.Fake<ITile>();
                    tiles[i, j] = tile;
                }


            var positions = new List<(int, int)> { (0, 0), (1, 0), (2, 1) };
            var objects = new List<IDynamicObject>();
            foreach (var t in positions)
            {
                int x = t.Item1;
                int y = t.Item2;
                IDynamicObject fakeObj = A.Fake<IDynamicObject>();
                A.CallTo(() => fakeObj.X).ReturnsLazily(() => x);
                A.CallTo(() => fakeObj.Y).ReturnsLazily(() => y);
                A.CallTo(() => fakeObj.Strength).Returns(2);
                objects.Add(fakeObj);
            }

            var obj = A.Fake<IDynamicObject>();
            int strength = 1;
            A.CallTo(() => obj.Strength).ReturnsLazily(() => strength);
            A.CallTo(() => obj.X).Returns(2);
            A.CallTo(() => obj.Y).Returns(2);
            A.CallTo(() => obj.AddStrength(-1)).Invokes(() => strength--);

            objects.Add(obj);

            A.CallTo(() => info.Length).Returns(3);
            A.CallTo(() => info.Hight).Returns(3);
            A.CallTo(() => info.ObjectStrengthDecay).Returns(1);

            A.CallTo(() => board.GetAlive()).Returns(objects);


            var gameLogic = new GameLogic();
            gameLogic.GameLogicTest(info, board, rnd);
            gameLogic.UpdateAlive();
            A.CallTo(() => obj.SetState(State.Dead)).MustHaveHappened();
        }


        [TestMethod]

        public void Test_GenerateFood()
        {
            IInfo info = A.Fake<IInfo>();
            IBoard board = A.Fake<IBoard>();
            IRandomTest rnd = A.Fake<IRandomTest>();

            GameLogic gameLogic = new GameLogic();
            gameLogic.GameLogicTest(info, board, rnd);

            gameLogic.GenerateFood();

            A.CallTo(() => board.GenetareFood()).MustHaveHappenedOnceExactly();
        }

    }
}
