using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace UnityNet.Event
{
	public class SFEvent : EventBase
	{
        public static readonly string VALIDATE_PASSWORD_SUCCESS = "ValidatePasswordSuccess";

        public static readonly string VALIDATE_PASSWORD_FAILTURE = "ValidatePasswordFailture";

        public static readonly string REGISTER_HOST = "RegisterHost";

        public static readonly string UNREGISTER_HOST = "UnRegisterHost";

        public static readonly string CONNECTION = "Connection";

        public static readonly string CONNECT_SERVER = "ConnectServer";
		
		public static readonly string CONNECT_SERVER_SUCCESS = "ConnectServerSuccess";

        public static readonly string HEART = "heart";

        public SFEvent(string type) : base(type, null)
        {
        }

        public SFEvent(string type, Hashtable data)
            : base(type, data)
        {
        }
	}
}
