using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyBoard;
using Utils;
using Tiles;


namespace Unit_Tests
{
    [TestClass]
    public class BoardUnitTests
    {
        [TestMethod]
        public void Test_GenerateInitialObjects()
        {
            Info info = Info.Instance;
            Tile[,] tiles = new Tile[info.Length, info.Hight];
            for (int i = 0; i < info.Length; i++)
                for (int j = 0; j < info.Hight; j++)
                    tiles[i, j] = new Tile();

            IBoard board = new Board(tiles, info);

            board.GenerateInitialObjects(info.Length + info.Hight);

            int count = 0;
            for (int i = 0; i < info.Length; i++)
                for (int j = 0; j < info.Hight; j++)
                    if (tiles[i, j].DynamicObject != null)
                        count++;
            Assert.AreEqual(info.Length + info.Hight, count);

        }
    }
}
