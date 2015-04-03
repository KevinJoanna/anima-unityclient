using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Event;

namespace UnityNet.Event
{
    public class GeneralEventArgs : MyEvent
    {

        public GeneralEventArgs(string evtType)
        {
            if (string.IsNullOrEmpty(evtType))
            {
                throw new ArgumentNullException("evtType == null");
            }

            Type = evtType;
        }

        public GeneralEventArgs(string evtType,object parame)
        {
            if (string.IsNullOrEmpty(evtType))
            {
                throw new ArgumentNullException("evtType == null");
            }

            Type = evtType;
            Parame = parame;
        }
    } 
}
