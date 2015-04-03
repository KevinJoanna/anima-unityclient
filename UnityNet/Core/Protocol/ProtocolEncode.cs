using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Common.Utils;
using UnityNet.Core.Remoting;

namespace UnityNet.Core.Protocol
{
	public interface ProtocolEncode
	{
        void Encode(Channel ch, object obj, ByteArray output);
	}
}
