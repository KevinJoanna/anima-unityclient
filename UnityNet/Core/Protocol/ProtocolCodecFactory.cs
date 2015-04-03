using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Common.Utils;
using UnityNet.Core.Remoting;

namespace UnityNet.Core.Protocol
{
    public interface ProtocolCodecFactory
    {
        ProtocolEncode Encode { get; }

        ProtocolDecode Decode { get; }
    }
}
