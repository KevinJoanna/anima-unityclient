using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Event;
using System.Collections;

namespace UnityNet.Event
{
    public class ClientEvent : EventBase
    {
        public static readonly string CONNECT = "connect";
        public static readonly string DATA_ERROR = "dataError";
        public static readonly string DISCONNECT = "disconnect";
        public static readonly string IO_ERROR = "ioError";
        public static readonly string RECONNECTION_TRY = "reconnectionTry";
        public static readonly string KICK_CLIENT = "kickClient";
        
        public ClientEvent(string type)
            : base(type, null)
        {
        }

        public ClientEvent(string type, Hashtable arguments)
            : base(type, arguments)
        {
        }

        public static bool ContainEventName(string eveName)
        {
            if (eveName.Equals(CONNECT))
            {
                return true;
            }
            else if (eveName.Equals(DATA_ERROR))
            {
                return true;
            }
            else if (eveName.Equals(DISCONNECT))
            {
                return true;
            }
            else if (eveName.Equals(IO_ERROR))
            {
                return true;
            }
            return false;
        }
    }
}
