using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyBoard;
using Utils;
using Tiles;
using System;
using FakeItEasy;


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
            Random r = new Random();
            IRandomTest rnd = A.Fake<IRandomTest>();

            A.CallTo(() => rnd.Next(A<int>.Ignored, A<int>.Ignored)).ReturnsLazily((int x, int y) => r.Next(x, y));
            A.CallTo(() => info.Length).Returns(5);
            A.CallTo(() => info.Hight).Returns(5);
            A.CallTo(() => info.ObjectStartStrengthHigh).Returns(2);
            A.CallTo(() => info.ObjectStartStrengthLow).Returns(1);
            A.CallTo(() => info.ID).Returns(0);

            Tile[,] tiles = new Tile[info.Length, info.Hight];
            for (int i = 0; i < info.Length; i++)
                for (int j = 0; j < info.Hight; j++)
                    tiles[i, j] = new Tile();

            IBoard board = new Board(tiles, info, rnd);


            //act
            board.GenerateInitialObjects(10);

            //assert
            int count = 0;
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
            Random r = new Random();
            IRandomTest rnd = A.Fake<IRandomTest>();

            A.CallTo(() => rnd.Next(A<int>.Ignored, A<int>.Ignored)).ReturnsLazily((int x, int y) => r.Next(x, y));
            A.CallTo(() => info.Length).Returns(5);
            A.CallTo(() => info.Hight).Returns(5);
            A.CallTo(() => info.FoodPerDay).Returns(10);

            Tile[,] tiles = new Tile[info.Length, info.Hight];
            for (int i = 0; i < info.Length; i++)
                for (int j = 0; j < info.Hight; j++)
                    tiles[i, j] = new Tile();

            IBoard board = new Board(tiles, info, rnd);

            board.GenetareFood();

            int count = 0;
            for (int i = 0; i < info.Length; i++)
                for (int j = 0; j < info.Hight; j++)
                    if (tiles[i, j].StaticObject != null)
                        count++;

            bool ans = count <= 10 && count >= 1;

            Assert.AreEqual(true, ans);
        }


    }
}
