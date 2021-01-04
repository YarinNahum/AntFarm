using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles;
using ProducerConsumer;
using System.Threading;
using DynamicObjects;
using Utils;

namespace BoardNamespace
{
    public interface IBoard
    {
        /// <summary>
        /// Generates dynamic objects on the board.
        /// </summary>
        /// <param name="count">The number of dynamic objects to create</param>
        /// <returns>A list of all the generated objects</returns>
        List<IDynamicObject> GenerateInitialObjects(int count);

        /// <summary>
        /// Set the dynamic object in position (x,y) with obj
        /// </summary>
        /// <returns ></returns>
        void SetTileObject(IDynamicObject obj, int x, int y);

        /// <summary>
        /// Generate static objects on the board by at most the amount specified in the Info class
        /// See <see cref="Info.FoodPerDay"/>
        /// </summary>
        void GenetareFood();

        /// <summary>
        /// update the status of the given object. It must catch the lock in the same tile as the object
        /// in Read mode.
        /// See <see cref="ReaderWriterLockSlim"/>
        /// <seealso cref="Tile"/>
        /// </summary>
        /// <param name="obj">Update the given object</param>
        void UpdateStatus(IDynamicObject obj);

        /// <summary>
        /// Try to move the given object to the position (x,y).
        /// If x or y are  out of bounds, it will throw a ArgumentExcaption exception.
        /// <seealso cref="Tile"/>
        /// </summary>
        /// <param name="obj">The object trying to move</param>
        /// <param name="x">The x axis value to move</param>
        /// <param name="y">The y axis value to move</param>
        /// <returns></returns>
        bool TryToMove(IDynamicObject obj, int x, int y);

        /// <summary>
        /// Get all near objects with the given position as the center.
        /// Catches each near tile's lock in read mode.
        /// See <see cref="ReaderWriterLockSlim"/>
        /// <seealso cref="Tile"/>
        /// </summary>
        /// <param name="x">The x axis value of the position</param>
        /// <param name="y">The y axis value of the position</param>
        /// <returns>A list of all the objects near the given object argument</returns>
        List<IDynamicObject> GetNearObjects(int x, int y);

        /// <summary>
        /// Will try to create a IDynamicObject on the board.
        /// If x or y are out of bounds, it will throw a ArgumentExcaption exception.
        /// </summary>
        /// <param name="x">The x axis value of the position</param>
        /// <param name="y">The y axis value of the position</param>
        /// <returns>The IDynamicObject if it was created successfully, null otherwise</returns>
        IDynamicObject TryCreate(int x, int y);

        /// <summary>
        /// Get all the objects that are alive on the board
        /// </summary>
        /// <returns>A list of IDynamicObject</returns>
        List<IDynamicObject> GetAlive();

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
