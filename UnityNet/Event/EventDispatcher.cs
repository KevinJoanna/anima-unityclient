using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace UnityNet.Event
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEventArgs"></typeparam>
    /// <param name="eventArgs"></param>
    public delegate void EventListenerDelegate<TEventArgs>(TEventArgs eventArgs) where TEventArgs : MyEvent;

    public class EventDispatcher :IDispatcher
    {
        private Hashtable listeners = new Hashtable();
        private object target;

        public EventDispatcher(object target)
        {
            this.target = target;
        }

        public void AddEventListener(string evtType, EventListenerDelegate<MyEvent> listener)
        {
            EventListenerDelegate<MyEvent> a = this.listeners[evtType] as EventListenerDelegate<MyEvent>;
            a = (EventListenerDelegate<MyEvent>)Delegate.Combine(a, listener);
            this.listeners[evtType] = a;
        }

        public void Dispatch(MyEvent evt)
        {
            EventListenerDelegate<MyEvent> delegate2 = this.listeners[evt.Type] as EventListenerDelegate<MyEvent>;
            if (delegate2 != null)
            {
                try
                {
                    delegate2(evt);
                }
                catch (Exception exception)
                {
                    string[] textArray1 = new string[] { "Error dispatching event ", evt.Type, ": ", exception.Message, " ", exception.StackTrace };
                    throw new Exception(string.Concat(textArray1), exception);
                }
            }
        }

        public void RemoveAll()
        {
            this.listeners.Clear();
        }

        public void Remove(string evtType, EventListenerDelegate<MyEvent> listener)
        {
            EventListenerDelegate<MyEvent> source = this.listeners[evtType] as EventListenerDelegate<MyEvent>;
            if (source != null)
            {
                source = (EventListenerDelegate<MyEvent>)Delegate.Remove(source, listener);
            }
            this.listeners[evtType] = source;
        }


        public IDispatcher Dispatcher
        {
            get { return this; }
        }
    }
}
