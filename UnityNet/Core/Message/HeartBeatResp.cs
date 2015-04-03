using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Common.Serializa;

namespace UnityNet.Core.Message
{
    public class HeartBeatResp : Decodeable
    {
        private bool oneWay;

        public void DeserializeTo(DataInput input)
        {
            this.oneWay = input.ReadBool();
        }

        public bool OneWay
        {
            get { return oneWay; }
        }
    }
}
