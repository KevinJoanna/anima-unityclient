using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Common.Utils;

namespace UnityNet.Common.Serializa
{
	public class BinaryDataOutput : Dataoutput
	{
        private ByteArray output;

        public BinaryDataOutput(ByteArray output)
        {
            if (output == null)
                throw new ArgumentNullException("Output");
            this.output = output;
        }

        public void WriteByte(byte b)
        {
            this.output.WriteByte(b);
        }

        public void WriteBool(bool b)
        {
            this.output.WriteBool(b);
        }

        public void WriteShort(short s)
        {
            this.output.WriteShort(s);
        }

        public void WriteUShort(ushort s)
        {
            this.output.WriteUShort(s);
        }

        public void WriteInt(int i)
        {
            this.output.WriteInt(i);
        }

        public void WriteLong(long l)
        {
            this.output.WriteLong(l);
        }

        public void WriteFloat(float f)
        {
            this.output.WriteFloat(f);
        }

        public void WriteUTF(string s)
        {
            this.output.WriteUTF(s);
        }

        public void WriteDouble(double d)
        {
            this.output.WriteDouble(d);
        }

        public void WriteBytes(byte[] buf, int offset, int len)
        {
            if (buf == null)
            {
                this.output.WriteInt(-1);
                return;
            }
            this.output.WriteInt(len);
            this.output.WriteBytes(buf, offset, len);
        }

        public void WriteObject(Serialization ser)
        {
            if (ser == null)
            {
                this.output.WriteByte(Convert.ToByte(0));
                return;
            }
			else
			{
				this.output.WriteByte(Convert.ToByte(1));
			}

            ser.SerializaTo(this);
        }

        public void WriteBytes(byte[] buf)
        {
            if (buf == null)
            {
                this.output.WriteInt(-1);
                return;
            }
            this.output.WriteInt(buf.Length);
            this.output.WriteBytes(buf);
        }
    }
}
