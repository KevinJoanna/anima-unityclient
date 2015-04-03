using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityNet.Common.Serializa
{
    public interface Decodeable
    {
        void DeserializeTo(DataInput input);
    }
}
