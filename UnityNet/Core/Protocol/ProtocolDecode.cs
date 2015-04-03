using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Common.Utils;
using UnityNet.Core.Protocol;
using UnityNet.Core.Remoting;

namespace UnityNet.Core.Protocol
{
	public interface ProtocolDecode
	{
        void Decode(Channel ch,ByteArray input);
    }
}
