using IDynamicObjects;
using StaticObjects;
using System.Threading;

namespace Tiles
{
    public interface ITile
    {
         IDynamicObject DynamicObject { get; set; }
         IStaticObject StaticObject { get; set; }
         ReaderWriterLockSlim Lock { get; }
    }
}
