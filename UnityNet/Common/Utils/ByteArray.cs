using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UnityNet.Common.Utils
{
    public class ByteArray
    {
        private byte[] buffer;
        private int position;

        public ByteArray()
        {
            this.position = 0;
            this.buffer = new byte[0];
        }

        public ByteArray(byte[] buf)
        {
            this.position = 0;
            this.buffer = buf;
        }

        public bool ReadBool()
        {
            return (this.buffer[this.position++] == 1);
        }

        public byte ReadByte()
        {
            return this.buffer[this.position++];
        }

        public byte[] ReadBytes(int count)
        {
            byte[] dst = new byte[count];
            Buffer.BlockCopy(this.buffer, this.position, dst, 0, count);
            this.position += count;
            return dst;
        }

        public void ReadBytes(byte[] dst, int offset, int length)
        {
            Buffer.BlockCopy(this.buffer, this.position, dst, offset, length);
            this.position += length;
        }

        public double ReadDouble()
        {
            return BitConverter.ToDouble(this.ReverseOrder(this.ReadBytes(8)), 0);
        }

        public float ReadFloat()
        {
            return BitConverter.ToSingle(this.ReverseOrder(this.ReadBytes(4)), 0);
        }

        public int ReadInt()
        {
            return BitConverter.ToInt32(this.ReverseOrder(this.ReadBytes(4)), 0);
        }

        public long ReadLong()
        {

            return BitConverter.ToInt64(this.ReverseOrder(this.ReadBytes(8)), 0);
        }

        public short ReadShort()
        {
            return BitConverter.ToInt16(this.ReverseOrder(this.ReadBytes(2)), 0);
        }

        public ushort ReadUShort()
        {
            return BitConverter.ToUInt16(this.ReverseOrder(this.ReadBytes(2)), 0);
        }

        public string ReadUTF()
        {
            int count = this.ReadInt();
            if (count == -1) return null;
            string str = Encoding.UTF8.GetString(this.buffer, this.position, count);
            this.position += count;
            return str;
        }

        public void WriteBool(bool b)
        {
            byte[] data = new byte[] { !b ? ((byte)0) : ((byte)1) };
            this.WriteBytes(data);
        }

        public void WriteByte(byte b)
        {
            byte[] data = new byte[] { b };
            this.WriteBytes(data);
        }

        public void WriteBytes(byte[] data)
        {
            this.WriteBytes(data, 0, data.Length);
        }

        public void WriteBytes(byte[] data, int offset, int count)
        {
            byte[] dst = new byte[count + this.buffer.Length];
            Buffer.BlockCopy(this.buffer, 0, dst, 0, this.buffer.Length);
            Buffer.BlockCopy(data, offset, dst, this.buffer.Length, count);
            this.buffer = dst;
        }

        public void WriteDouble(double d)
        {
            byte[] bytes = BitConverter.GetBytes(d);
            this.WriteBytes(this.ReverseOrder(bytes));
        }

        public void WriteFloat(float f)
        {
            byte[] bytes = BitConverter.GetBytes(f);
            this.WriteBytes(this.ReverseOrder(bytes));
        }

        public void WriteInt(int i)
        {
            byte[] bytes = BitConverter.GetBytes(i);
            this.WriteBytes(this.ReverseOrder(bytes));
        }

        public void WriteLong(long l)
        {
            byte[] bytes = BitConverter.GetBytes(l);
            this.WriteBytes(this.ReverseOrder(bytes));
        }

        public void WriteShort(short s)
        {
            byte[] bytes = BitConverter.GetBytes(s);
            this.WriteBytes(this.ReverseOrder(bytes));
        }

        public void WriteUShort(ushort us)
        {
            byte[] bytes = BitConverter.GetBytes(us);
            this.WriteBytes(this.ReverseOrder(bytes));
        }

        public void WriteUTF(string str)
        {
            if (str == null)
            {
                this.WriteInt(-1);
                return;
            }
            int num = 0;
            for (int i = 0; i < str.Length; i++)
            {
                int num3 = str[i];
                if ((num3 >= 1) && (num3 <= 0x7f))
                {
                    num++;
                }
                else if (num3 > 0x7ff)
                {
                    num += 3;
                }
                else
                {
                    num += 2;
                }
            }
            if (num > 0x8000)
            {
                throw new FormatException("String length cannot be greater then 32768 !");
            }
            this.WriteInt(num);
            this.WriteBytes(Encoding.UTF8.GetBytes(str));
        }

        public byte[] ReverseOrder(byte[] dt)
        {
            if (!BitConverter.IsLittleEndian)
            {
                return dt;
            }
            byte[] buffer = new byte[dt.Length];
            int num = 0;
            for (int i = dt.Length - 1; i >= 0; i--)
            {
                buffer[num++] = dt[i];
            }
            return buffer;
        }

        public byte[] Bytes
        {
            get
            {
                return this.buffer;
            }
            set
            {
                this.buffer = value;
            }
        }

        public int BytesAvailable
        {
            get
            {
                int num = this.buffer.Length - this.position;
                if ((num <= this.buffer.Length) && (num >= 0))
                {
                    return num;
                }
                return 0;
            }
        }

        public int Length
        {
            get
            {
                return this.buffer.Length;
            }
        }

        public int Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
            }
        }

        internal void WriteShort(ushort p)
        {
            throw new NotImplementedException();
        }
    }
}
