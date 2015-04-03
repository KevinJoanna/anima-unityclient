using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityNet.Event
{
    public interface IDispatchable
    {
        void AddEventListener(string evtType, EventListenerDelegate<MyEvent> listener);

        IDispatcher Dispatcher { get; }
    }
}
