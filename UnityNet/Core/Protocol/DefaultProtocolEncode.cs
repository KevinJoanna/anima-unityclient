using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Core.Protocol;
using UnityNet.Common.Utils;
using UnityNet.FSM;
using UnityNet.Logging;
using UnityNet.Core.Remoting;
using UnityNet.Common.Serializa;
using UnityNet.Core.Message;

namespace UnityNet.Core.Protocol
{
	public class DefaultProtocolEncode : ProtocolEncode
	{
        private Endpoint endpoint;
        private Codec codec;
        public static readonly int SHORT_BYTE_SIZE = 2;
        public static readonly int INT_BYTE_SIZE = 4;


        public DefaultProtocolEncode(Endpoint endpoint)
        {
            this.endpoint = endpoint;
            codec = new DefaultCodec(endpoint);
        }

        public void Encode(Channel ch,object obj,ByteArray output)
        {
            codec.Encode(ch, obj, output);
        }
    }
}
