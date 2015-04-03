using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Core.Remoting;
using UnityNet.Common.Serializa;
using UnityNet.Common.Utils;

namespace UnityNet.Core.Protocol
{
	public interface Codec
	{
        object Decode(Channel ch,Packet packet);

        void Encode(Channel ch, object message, ByteArray output);
	}
}
