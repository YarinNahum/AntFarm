using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDynamicObjects;

namespace StaticObjects
{
    /// <summary>
    /// An interface for all the static objects
    /// </summary>
    public interface IStaticObject
    {
        void ActOnDynamicObject(IDynamicObject obj);
    
    }

}
