using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Common.Serializa;

namespace UnityNet.Core.Message
{
    public class HandSnakeResp : Decodeable
    {
        private bool success;
        private int heartbeatTime;
        private int payload;
        private string reconnectToken;

        public void DeserializeTo(DataInput input)
        {
            this.success = input.ReadBool();
            this.heartbeatTime = input.ReadInt();
            this.payload = input.ReadInt();
            this.reconnectToken = input.ReadUTF();
        }

        public bool Success
        {
            get { return success; }
            set { success = value; }
        }

        public int HeartbeatTime
        {
            get { return heartbeatTime; }
            set { heartbeatTime = value; }
        }

        public int Payload
        {
            get { return payload; }
            set { payload = value; }
        }

        public string ReconnectToken
        {
            get { return reconnectToken; }
            set { reconnectToken = value; }
        }


        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[ HandSnakeResp {");
            builder.Append("");
            return builder.ToString();
        }
    }
}
