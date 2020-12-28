using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyBoard;
using Utils;
using Tiles;
using System;
using ProducerConsumer;
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
            A.CallTo(() => rnd.Next(A<int>.Ignored, A<int>.Ignored)).ReturnsNextFromSequence(values);

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
    }
}
