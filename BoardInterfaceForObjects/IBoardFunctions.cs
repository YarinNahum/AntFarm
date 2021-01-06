using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDynamicObjects;

namespace BoardInterfaceForObjects
{
    public interface IBoardFunctions
    {
        /// <summary>
        /// Get all the objects that are alive on the board
        /// </summary>
        /// <returns>A list of IDynamicObject</returns>
        List<IDynamicObject> GetAlive();


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
        /// Try to move the given object in position (fromX,fromY) to position (x,y)
        /// If x or y are  out of bounds, it will throw a ArgumentExcaption exception.
        /// <seealso cref="Tile"/>
        /// </summary>
        /// <param name="fromX">The x axis value to move from</param>
        /// <param name="fromY">The y axis value to move from</param>
        /// <param name="x">The x axis value to move</param>
        /// <param name="y">The y axis value to move</param>
        /// <returns></returns>
        bool TryToMove(int fromX, int fromY, int toX, int toY);

    }
}
