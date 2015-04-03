using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityNet.Core.Protocol
{
    public enum PacketReadTransition
    {
        NewPakcetReceived,
        PacketHeaderReceived,
        IncompletePacketHeader,
        WholePacketHeaderReceived,
        PacketFinished,
        InvalidData,
        InvalidDataFinished
    }
}
