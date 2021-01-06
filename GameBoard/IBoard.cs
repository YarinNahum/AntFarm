using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles;
using ProducerConsumer;
using System.Threading;
using IDynamicObjects;
using BoardInterfaceForObjects;
using Utils;

namespace BoardNamespace
{
    public interface IBoard : IBoardFunctions
    {
        /// <summary>
        /// Generates dynamic objects on the board.
        /// </summary>
        /// <param name="count">The number of dynamic objects to create</param>
        /// <returns>A list of all the generated objects</returns>
        List<IDynamicObject> GenerateInitialObjects(int count);

        /// <summary>
        /// Set the dynamic object in position (x,y) with null
        /// </summary>
        /// <returns ></returns>
        void ClearDynamicObjectOnTile(int x, int y);

        /// <summary>
        /// Generate static objects on the board by at most the amount specified in the Info class
        /// See <see cref="Info.FoodPerDay"/>
        /// </summary>
        void GenetareFood();

        /// <summary>
        /// Will try to create a IDynamicObject on the board.
        /// If x or y are out of bounds, it will throw a ArgumentExcaption exception.
        /// </summary>
        /// <param name="x">The x axis value of the position</param>
        /// <param name="y">The y axis value of the position</param>
        /// <returns>The IDynamicObject if it was created successfully, null otherwise</returns>
        IDynamicObject TryCreate(int x, int y);

        /// <summary>
        /// Set the alive objects
        /// </summary>
        /// <param name="alive">A list of non-dead objects</param>
        void SetAlive(List<IDynamicObject> alive);

        /// <summary>
        /// Return a random position on the board
        /// </summary>
        /// <returns>A tuple of position</returns>
        Tuple<int, int> GetRandomPosition();

        /// <summary>
        /// For testing purposes
        /// </summary>
        /// <param name="tiles"></param>
        /// <param name="info"></param>
        /// <param name="rnd"></param>
        /// <param name="producerConsumer"></param>
        void TestBoard(ITile[,] tiles, IInfo info, IRandomTest rnd, IProducerConsumerMessages<string> producerConsumer);
    }
}
