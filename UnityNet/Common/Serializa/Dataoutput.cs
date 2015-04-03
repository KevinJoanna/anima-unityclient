using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityNet.Common.Serializa
{
    public interface Dataoutput
    {
        void WriteByte(byte b);

        void WriteBool(bool b);

        void WriteShort(short s);

        void WriteUShort(ushort s);

        void WriteInt(int i);

        void WriteLong(long l);

        void WriteFloat(float f);

        void WriteUTF(string s);

        void WriteDouble(double d);

        void WriteBytes(byte[] buf, int offset, int len);

        void WriteBytes(byte[] buf);

        void WriteObject(Serialization ser);
    }
}
