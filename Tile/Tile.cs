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
    public class Tile
    {
        public IDynamicObject DynamicObject { get; set; }
        public IStaticObject StaticObject { get; set; }
        public ReaderWriterLockSlim ReaderWriterLockSlim { get; }
        
        public Tile()
        {
            this.ReaderWriterLockSlim = new ReaderWriterLockSlim();
        }
    }
}
