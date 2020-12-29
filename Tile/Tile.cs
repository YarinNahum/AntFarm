using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DynamicObjects;
using StaticObjects;

namespace Tiles
{
    /// <summary>
    /// A class to represent a single tile on the board.
    /// A Tile instance has: IDynamicObject, IStaticObject and a ReaderWriterLockSlim lock
    /// See <see cref="IDynamicObject"/>
    /// <seealso cref="IStaticObject"/>
    /// <seealso cref="ReaderWriterLockSlim"/>
    /// </summary>
    public class Tile : ITile
    {
        public IDynamicObject DynamicObject { get; set; }
        public IStaticObject StaticObject { get; set; }
        public ReaderWriterLockSlim Lock { get; }
        
        public Tile()
        {
            this.Lock = new ReaderWriterLockSlim();
        }
    }
}
