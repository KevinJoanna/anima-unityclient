using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityNet.Core.Message
{
    public class BroadCast  : AbstractMessage
    {
        public BroadCast(int id)
            : base(id, AbstractMessage.TYPE_BROADCAST)
        {

        }
    }
}
