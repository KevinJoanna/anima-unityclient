using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Core.Message;

namespace UnityNet.Core.Message
{
    public class Request : AbstractMessage
    {
        private int sequence;
        private bool twoWay;

        public Request(int id)
            : base(id, AbstractMessage.TYPE_REQUEST)
        {

        }

        public int Sequence
        {
            get { return sequence; }
            set { sequence = value; }
        }

        public bool TwoWay
        {
            get { return twoWay; }
            set { twoWay = value; }
        }

        public bool IsRequest()
        {
            return twoWay;
        }

        public bool IsNotify()
        {
            return twoWay ? false : true;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
