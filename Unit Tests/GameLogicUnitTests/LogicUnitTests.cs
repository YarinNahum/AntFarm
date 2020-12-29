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

            GameLogic gameLogic = new GameLogic();
            gameLogic.GameLogicTest(info, board);

            List<IDynamicObject> l = new List<IDynamicObject>();

            int count = 10;
            A.CallTo(() => info.Length).Returns(5);
            A.CallTo(() => info.Hight).Returns(5);
            A.CallTo(() => info.NumberOfObjects).Returns(count);
            for (int i = 0; i < count; i++)
            {
                var obj = A.Fake<IDynamicObject>();
                A.CallTo(() => obj.Id).ReturnsLazily(()=>i);
                l.Add(obj);
            }

            A.CallTo(() => board.GenerateInitialObjects(count)).ReturnsLazily(() => l);

            gameLogic.GenerateInitialObjects();

            Assert.AreEqual(count, gameLogic.GetNumberOfAliveObjects());
        }

        [TestMethod]

        public void Test_WakeUp()
        {

            IInfo info = A.Fake<IInfo>();
            IBoard board = A.Fake<IBoard>();

            GameLogic gameLogic = new GameLogic();
            gameLogic.GameLogicTest(info, board);

            List<IDynamicObject> l = new List<IDynamicObject>();

            int count = 10;
            for (int i = 0; i < count; i++)
            {
                var obj = A.Fake<IDynamicObject>();
                A.CallTo(() => obj.Id).ReturnsLazily(() => i);
                A.CallTo(() => obj.SleepCount).Returns(1);
                l.Add(obj);
            }

            A.CallTo(() => board.UpdateStatusAll()).ReturnsLazily(() => l);

            var list = board.GenerateInitialObjects(count);

            Assert.AreEqual(count, list.Count);
        }
    }
}
