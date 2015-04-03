using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityNet.Event
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class NetCallBackParame : Attribute
    {
        private Type myParameType;

        public NetCallBackParame(Type parmeType)
        {
            if (parmeType == null)
            {
                throw new ArgumentNullException("parmeType == nulll");
            }

            this.myParameType = parmeType;
        }

        public Type ParameType
        {
            get { return this.myParameType; }
        }
    }
}
