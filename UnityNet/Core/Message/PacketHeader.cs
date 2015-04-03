using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityNet.Core.Protocol
{
    /// <summary>
    /// Message packet header
    /// </summary>
	public class PacketHeader
	{
        public const byte COMPRESSED = 1;

        public const byte UNCOMPRESSED = 0;
        /// <summary>
        /// Packet type
        /// </summary>
        private byte type;
        /// <summary>
        /// Packet compressed flag
        /// </summary>
        private bool compressed;
        /// <summary>
        /// data packet length
        /// </summary>
        private int expectedLen = -1;

        public PacketHeader(byte type,byte compressed,int length)
        {
            this.type = type;
            this.compressed = compressed == COMPRESSED ? true : false;
            this.expectedLen = length;
        }

        public byte Type
        {
            get { return type; }
            set { type = value; }
        }

        public bool Compressed
        {
            get { return compressed; }
            set { compressed = value; }
        }

        public int ExpectedLen
        {
            get { return expectedLen; }
            set { expectedLen = value; }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("---------------------------------------------\n");
            builder.Append("Type:\t" + this.type + "\n");
            builder.Append("Compressed:\t" + this.compressed + "\n");
            builder.Append("Length:\t" + this.expectedLen + "\n");
            builder.Append("---------------------------------------------\n");
            return builder.ToString();
        }
	}
}
