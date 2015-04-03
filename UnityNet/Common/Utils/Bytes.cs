using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityNet.Common.Utils
{
	public sealed class Bytes
	{

        public Bytes() { }

        public static long Int32ToInt64(int iLower, int iHigher)
        {
            long iResult = 0;
            iResult += (long)iHigher << 32;
            iResult += iLower;
            return iResult;
        }

        public static void Int64ToInt32(long value, ref int iHigher, ref int lower)
        {
            iHigher = (int)(value >> 32);
            lower = (int)(value & 0x00000000ffffffff);
        }

        public static byte[] Short2bytes(short v)
        {
            byte[] ret = { 0, 0 };
            Short2bytes(v, ret);
            return ret;
        }

        public static void Short2bytes(short v, byte[] b)
        {
            Short2bytes(v, b, 0);
        }

        public static void Short2bytes(short v, byte[] b, int off)
        {
            b[off + 1] = (byte)v;
            b[off + 0] = (byte)(v >> 8);
        }
    }
}
