using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles;
using System.Threading;
using DynamicObjects;
using Utils;

namespace MyBoard
{
    interface IBoard
    {
        /// <summary>
        /// Generates dynamic objects on the board.
        /// </summary>
        /// <param name="count">The number of dynamic objects to create</param>
        /// <returns>A list of all the generated objects</returns>
        List<IDynamicObject> GenerateInitialObjects(int count);

        /// <summary>
        /// Update the locally stored list of dynamic objects that are alive.
        /// </summary>
        /// <returns>A list of all the dynamic objects that are alive after the update</returns>
        List<IDynamicObject> UpdateAndGetAlive();

        /// <summary>
        /// Generate static objects on the board by amount specified in the Info class
        /// See <see cref="Info"/>
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
        /// Trying to move the given object to the position (x,y).
        /// It must catch the lock at position (x,y) in upgradable mode, if the tile in (x,y)
        /// is empty, than updrade the lock to write mode and catch the lock at the given object position 
        /// in write mode.
        /// If x or y is out of bounds, it will throw a ArgumentExcaption exception.
        /// See <see cref="ReaderWriterLockSlim"/>
        /// <seealso cref="Tile"/>
        /// </summary>
        /// <param name="obj">The object trying to move</param>
        /// <param name="x">The x axis value to move</param>
        /// <param name="y">The y axis value to move</param>
        /// <returns></returns>
        bool TryToMove(IDynamicObject obj, int x, int y);

        /// <summary>
        /// Get all near objects with the give object at the center.
        /// Catches each near tile's lock in read mode.
        /// See <see cref="ReaderWriterLockSlim"/>
        /// <seealso cref="Tile"/>
        /// </summary>
        /// <param name="obj">The object at the center</param>
        /// <returns>A list of all the objects near the given object argument</returns>
        List<IDynamicObject> GetNearObjects(IDynamicObject obj);


    }
}
