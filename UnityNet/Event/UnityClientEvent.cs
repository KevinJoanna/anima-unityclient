using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Event;

namespace UnityNet
{
    public class UnityClientEvent : MyEvent
    {
        public static readonly string CONNECTION = "connection";
        public static readonly string CONNECTION_LOST = "connection_lost";
        public static readonly string CONNECTION_RESUME = "connection_resume";
        public static readonly string CONNECTION_RETRY = "connectionRetry";
        public static readonly string HANDSHAKE = "handshake";
        public static readonly string CHANNEL_DATA_ERROR = "channelDataError";

        public UnityClientEvent(string evtType)
        {
            if (evtType == null)
            {
                throw new ArgumentNullException("evtType == null");
            }
            Type = evtType;
        }

        public UnityClientEvent(string evtType,object parame)
        {
            if (evtType == null)
            {
                throw new ArgumentNullException("evtType == null");
            }
            Type = evtType;
            Parame = parame;
        }

        public static bool ContainEventName(string eveName)
        {
            if (eveName.Equals(CONNECTION))
            {
                return true;
            }
            else if (eveName.Equals(CONNECTION_LOST))
            {
                return true;
            }
            else if (eveName.Equals(CONNECTION_RESUME))
            {
                return true;
            }
            else if (eveName.Equals(CONNECTION_RETRY))
            {
                return true;
            }
            else if (eveName.Equals(HANDSHAKE))
            {
                return true;
            }
            else if (eveName.Equals(CHANNEL_DATA_ERROR))
            {
                return true;
            }
            return false;
        }
    }
}
