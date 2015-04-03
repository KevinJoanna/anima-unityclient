using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityNet.Core.Message
{
    public class Push : AbstractMessage
    {
        public Push(int id)
            : base(id, AbstractMessage.TYPE_PUSH)
        {

        }
    }
}
