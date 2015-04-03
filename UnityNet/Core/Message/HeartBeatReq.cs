using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Common.Serializa;

namespace UnityNet.Core.Message
{
    public class HeartBeatReq : RequestArg
    {
        private bool oneWay;


        public HeartBeatReq(bool oneWay)
        {
            this.oneWay = oneWay;
        }

        public void SerializaTo(Dataoutput output)
        {
            output.WriteBool(oneWay);
        }

        public bool OneWay
        {
            set { this.oneWay = value; }
            get { return oneWay; }
        }
    }
}
