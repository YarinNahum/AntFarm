using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using FakeItEasy;
using GameLogicNameSpace;
using DynamicObjects;
using Utils;
using BoardNamespace;


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

        public void Test_ActDepressed()
        {
            //initialize
            IInfo info = A.Fake<IInfo>();
            IBoard board = A.Fake<IBoard>();
            IRandomTest rnd = A.Fake<IRandomTest>();

            GameLogic gameLogic = new GameLogic();
            gameLogic.GameLogicTest(info, board, rnd);

            List<IDynamicObject> l = new List<IDynamicObject>();

            int count = 3;
            for (int i = 0; i < count; i++)
            {
                var obj = A.Fake<IDynamicObject>();
                l.Add(obj);
            }

            var dynamicObject = A.Fake<IDynamicObject>();
            A.CallTo(() => dynamicObject.X).Returns(1);
            A.CallTo(() => dynamicObject.Y).Returns(1);
            A.CallTo(() => board.GetNearObjects(1, 1)).ReturnsLazily(() => l);
            A.CallTo(() => info.MaxObjectsPerArea).Returns(4);
            A.CallTo(() => info.MinObjectsPerArea).Returns(2);

            //act
            gameLogic.ActDepressedObject(dynamicObject);
            //assert
            A.CallTo(() => dynamicObject.SetState(State.Alive)).MustHaveHappened();
        }

        [TestMethod]

        public void Test_ActAlive_Move()
        {
            IInfo info = A.Fake<IInfo>();
            IBoard board = A.Fake<IBoard>();
            IRandomTest rnd = A.Fake<IRandomTest>();

            GameLogic gameLogic = new GameLogic();
            gameLogic.GameLogicTest(info, board, rnd);


            var dynamicObject = A.Fake<IDynamicObject>();

            A.CallTo(() => dynamicObject.DecideAction()).Returns("Move");
            A.CallTo(() => dynamicObject.X).Returns(1);
            A.CallTo(() => dynamicObject.Y).Returns(1);
            A.CallTo(() => rnd.Next(A<int>.Ignored, A<int>.Ignored)).Returns(0);

            gameLogic.ActAliveObject(dynamicObject);

            A.CallTo(() => board.TryToMove(dynamicObject, 0, 0)).MustHaveHappened();

        }

        [TestMethod]

        public void Test_ActAlive_Fight()
        {
            IInfo info = A.Fake<IInfo>();
            IBoard board = A.Fake<IBoard>();
            IRandomTest rnd = A.Fake<IRandomTest>();

            GameLogic gameLogic = new GameLogic();
            gameLogic.GameLogicTest(info, board, rnd);

            List<IDynamicObject> l = new List<IDynamicObject>();

            int count = 3;
            for (int i = 0; i < count; i++)
            {
                var obj = A.Fake<IDynamicObject>();
                l.Add(obj);
            }
            

            var dynamicObject = A.Fake<IDynamicObject>();
            A.CallTo(() => board.GetNearObjects(1, 1)).ReturnsLazily(() => l);

            A.CallTo(() => dynamicObject.DecideAction()).Returns("Fight");
            A.CallTo(() => dynamicObject.X).Returns(1);
            A.CallTo(() => dynamicObject.Y).Returns(1);
            A.CallTo(() => rnd.Next(A<int>.Ignored, A<int>.Ignored)).Returns(0);

            gameLogic.ActAliveObject(dynamicObject);

            A.CallTo(() => dynamicObject.Fight(l[0])).MustHaveHappened();
        }

        [TestMethod]

        public void Test_Move()
        {
            IInfo info = A.Fake<IInfo>();
            IBoard board = A.Fake<IBoard>();
            IRandomTest rnd = A.Fake<IRandomTest>();

            GameLogic gameLogic = new GameLogic();
            gameLogic.GameLogicTest(info, board, rnd);



            var dynamicObject = A.Fake<IDynamicObject>();

            A.CallTo(() => dynamicObject.X).Returns(1);
            A.CallTo(() => dynamicObject.Y).Returns(1);
            A.CallTo(() => rnd.Next(A<int>.Ignored, A<int>.Ignored)).Returns(0);

            gameLogic.Move(dynamicObject);

            A.CallTo(() => board.TryToMove(dynamicObject,0,0)).MustHaveHappened();

        }

        [TestMethod]
        public void Test_Fight()
        {
            IInfo info = A.Fake<IInfo>();
            IBoard board = A.Fake<IBoard>();
            IRandomTest rnd = A.Fake<IRandomTest>();

            GameLogic gameLogic = new GameLogic();
            gameLogic.GameLogicTest(info, board, rnd);

            List<IDynamicObject> l = new List<IDynamicObject>();

            int count = 3;
            for (int i = 0; i < count; i++)
            {
                var obj = A.Fake<IDynamicObject>();
                
                l.Add(obj);
            }
            var dynamicObject = A.Fake<IDynamicObject>();

            A.CallTo(() => dynamicObject.X).Returns(1);
            A.CallTo(() => dynamicObject.Y).Returns(1);
            A.CallTo(() => rnd.Next(A<int>.Ignored, A<int>.Ignored)).Returns(0);

            A.CallTo(() => board.GetNearObjects(1, 1)).ReturnsLazily(() => l);

            gameLogic.Fight(dynamicObject);

            A.CallTo(() => dynamicObject.Fight(l[0])).MustHaveHappened();

        }
    }
}
