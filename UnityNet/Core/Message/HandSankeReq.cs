using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Common.Serializa;

namespace UnityNet.Core.Message
{
    public class HandSankeReq : RequestArg
    {
        /// <summary>
        /// 客户端类型：Android or unity3d or IPHONE
        /// </summary>
        private string clientType;

        public string ClientType
        {
            get { return clientType; }
            set { clientType = value; }
        }
        /// <summary>
        /// API 版本 
        /// </summary>
        private string apiVersion;

        public string ApiVersion
        {
            get { return apiVersion; }
            set { apiVersion = value; }
        }
        /// <summary>
        /// 重连token
        /// </summary>
        private string reconnectToken;

        public string ReconnectToken
        {
            get { return reconnectToken; }
            set { reconnectToken = value; }
        }

        public void SerializaTo(Dataoutput output)
        {
            output.WriteUTF(this.clientType);
            output.WriteUTF(this.apiVersion);
            output.WriteUTF(this.reconnectToken);
        }
    }
}
