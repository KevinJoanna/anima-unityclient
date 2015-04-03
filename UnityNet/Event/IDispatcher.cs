using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityNet.Event
{
    public interface IDispatcher
    {
        void AddEventListener(string evtType, EventListenerDelegate<MyEvent> listener);

        void Dispatch(MyEvent evtArgs);

        void RemoveAll();

        void Remove(string evtType, EventListenerDelegate<MyEvent> listener);
    }
}
