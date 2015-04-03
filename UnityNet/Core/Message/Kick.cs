using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Common.Serializa;

namespace UnityNet.Core.Message
{
    public class KickClient : Decodeable
    {
        private string reason;

        public void DeserializeTo(DataInput input)
        {
            this.reason = input.ReadUTF();
        }

        public string Reason
        {
            get { return reason; }
        }
    }
}
