using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityNet.Common.Serializa
{
    public interface DataInput
    {
        byte ReadByte();

        bool ReadBool();

        short ReadShort();

        ushort ReadUShort();

        int ReadInt();

        long ReadLong();

        float ReadFloat();

        double ReadDouble();

        string ReadUTF() ;

        void ReadBytes(byte[] buf,int offset,int length);

        byte[] ReadBytes(int count);

        void ReadObject(Serialization ser);
    }
}
