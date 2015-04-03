using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityNet.Core.Protocol 
{
    public class ProtocolCodecException : Exception
	{
        public ProtocolCodecException(string message)
            : base(message)
        {
        }
	}
}
