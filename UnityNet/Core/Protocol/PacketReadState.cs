using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityNet.Core.Protocol
{
    public enum PacketReadState
    {
        WAIT_NEW_PACKET,
        WAIT_PACKET_HEADER,
        WAIT_PACKET_HEADER_FRAGMETN,
        WAIT_DATA,
        INVALID_DATA
    }
}
