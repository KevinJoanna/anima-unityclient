using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Common.Utils;

namespace UnityNet.Common.Serializa
{
	public class BinaryDataInput : DataInput 
	{
        private ByteArray input;

        public BinaryDataInput(ByteArray input)
        {
            if (input == null)
                throw new ArgumentNullException("Input");
            this.input = input;
        }

        public byte ReadByte()
        {
            return this.input.ReadByte();
        }

        public bool ReadBool()
        {
            return this.input.ReadBool();
        }

        public short ReadShort()
        {
            return this.input.ReadShort();
        }

        public ushort ReadUShort()
        {
            return this.input.ReadUShort();
        }

        public int ReadInt()
        {
            return this.input.ReadInt();
        }

        public long ReadLong()
        {
            return this.input.ReadLong();
        }

        public float ReadFloat()
        {
            return this.input.ReadFloat();
        }

        public double ReadDouble()
        {
            return this.input.ReadDouble();
        }

        public string ReadUTF()
        {
            return this.input.ReadUTF();
        }

        public void ReadBytes(byte[] dst, int offset, int length)
        {
            int count = this.ReadInt();
            if (count == -1)
            {
                return;
            }
            this.input.ReadBytes(dst, offset, length);
        }

        public byte[] ReadBytes(int count)
        {
            int len = this.ReadInt();
            if (len == -1)
            {
                return null;
            }
            return this.input.ReadBytes(count);
        }

        public void ReadObject(Serialization ser)
        {
            int b = this.ReadByte();
            if (b == 0)
            {
                ser = null;
                return;
            }  
            if (ser != null)
                ser.DeserializeTo(this);
        }
    }
}
