using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Common.Utils;

namespace UnityNet.Core.Protocol
{
	public class Packet
	{

        public const byte FLAG_HANDSNAKE = 0x10;

        public const byte FLAG_HEARTBEAT = 0x20;

        public const byte FLAG_MESSAGE = 0x30;

        public const byte FLAG_KICK = 0x40;

        private PacketHeader header;
        private ByteArray content;

        public Packet()
        {
            this.content = new ByteArray();
        }

        public Packet(PacketHeader header):this()
        {
            this.content = new ByteArray();
        }

        public PacketHeader Header
        {
            get
            {
                return header;
            }

            set
            {
                this.header = value;
            }
        }

        public ByteArray Content
        {
            set
            {
                this.content = value;
            }
            get
            {
                return this.content;
            }
        }
	}
}
