using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityNet.Common.Utils
{
	public sealed class HexDumpFormatter
	{
        private HexDumpFormatter() { }

        private static readonly char DOT = '.';
        private static readonly int HEX_BYTES_PER_LINE = 0x10;
        private static readonly int MAX_DUMP_LENGTH = 0x400;
        private static readonly char NEW_LINE = '\n';
        private static readonly char TAB = '\t';

        public static string HexDump(ByteArray ba)
        {
            return HexDump(ba, HEX_BYTES_PER_LINE);
        }

        public static string HexDump(ByteArray ba, int bytesPerLine)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Binary Size: " + ba.Length.ToString() + NEW_LINE);
            if (ba.Length > MAX_DUMP_LENGTH)
            {
                builder.Append("** Data larger than max dump size of " + MAX_DUMP_LENGTH + ". Data not displayed");
                return builder.ToString();
            }
            StringBuilder builder2 = new StringBuilder();
            StringBuilder builder3 = new StringBuilder();
            int index = 0;
            int num3 = 0;
            do
            {
                char dOT;
                byte num4 = ba.Bytes[index];
                string str = string.Format("{0:x2}", num4);
                if (str.Length == 1)
                {
                    str = "0" + str;
                }
                builder2.Append(str + " ");
                if ((num4 >= 0x21) && (num4 <= 0x7e))
                {
                    dOT = Convert.ToChar(num4);
                }
                else
                {
                    dOT = DOT;
                }
                builder3.Append(dOT);
                if (++num3 == bytesPerLine)
                {
                    num3 = 0;
                    builder.Append(string.Concat(new object[] { builder2.ToString(), TAB, builder3.ToString(), NEW_LINE }));
                    builder2 = new StringBuilder();
                    builder3 = new StringBuilder();
                }
            }
            while (++index < ba.Length);
            if (num3 != 0)
            {
                for (int i = bytesPerLine - num3; i > 0; i--)
                {
                    builder2.Append("   ");
                    builder3.Append(" ");
                }
                builder.Append(string.Concat(new object[] { builder2.ToString(), TAB, builder3.ToString(), NEW_LINE }));
            }
            return builder.ToString();
        }
	}
}
