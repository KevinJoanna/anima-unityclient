using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityNet.Core.Protocol
{
	public class DefaultProtocolCodecFactory : ProtocolCodecFactory
	{
        private ProtocolEncode encode; 
        private ProtocolDecode decode;

        public DefaultProtocolCodecFactory(Endpoint endpoint)
        {
            encode = new DefaultProtocolEncode(endpoint);
            decode = new DefaultProtocolDecode(endpoint);
        }

        public ProtocolEncode Encode
        {
            get { return this.encode; }
        }

        public ProtocolDecode Decode
        {
            get { return this.decode; }
        }
    }
}
