using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDynamicObjects;

namespace StaticObjects
{
    public class Food : IStaticObject
    {
        public void ActOnDynamicObject(IDynamicObject obj)
        {
            obj.ActOnStaticObject(this);
        }
    }
}
