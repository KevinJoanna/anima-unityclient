using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityNet.Common.Serializa
{
    public interface Serialization
    {
        void SerializaTo(Dataoutput output);

        void DeserializeTo(DataInput input);
    }
}
