using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Logging;
using UnityNet.Event;
using UnityNet.Core.Handler;
using System.Reflection;

namespace UnityNet.Core
{
	public abstract class AbstractEndpoint : Endpoint
	{
        protected Logger log = null;
        protected bool debug;
        protected IHandler hander;
        protected double time;
        protected EventDispatcher dispatcher;
        protected AbstractEndpoint parent;

        public AbstractEndpoint()
        {

        }

        public AbstractEndpoint(AbstractEndpoint parent)
        {
            this.parent = parent;
			this.log = parent.Logger ;
            dispatcher = new EventDispatcher(this);
        }

        public Logger Logger
        {
            set
            {
                this.log = value;
            }
            get
            {
                return this.log;
            }
        }

        public LogLevel LogLevel
        {
            get
            {
                return this.log.LoggingLevel;
            }
            set
            {
                this.log.LoggingLevel = value;
            }
        }

        public IHandler Handler
        {
            get
            {
                return this.hander;
            }
            set
            {
                this.hander = value;
            }
        }

        public bool Debug
        {
            get 
            {
                return this.debug;
            }
        }

        public AbstractEndpoint Parent
        {
            get
            {
                return this.parent;
            }
        }

        public virtual void AddEventListener(string evtType, Event.EventListenerDelegate<MyEvent> listener)
        {
            this.dispatcher.AddEventListener(evtType, listener);
        }

        public Event.IDispatcher Dispatcher
        {
            get { return this.dispatcher; }
        }

        internal virtual void Dispatch(MyEvent eventAgrs)
        {
            this.dispatcher.Dispatch(eventAgrs);
        }

        public void RemoveAll()
        {
            this.dispatcher.RemoveAll();
        }

        public void Remove(string evtType, Event.EventListenerDelegate<MyEvent> listener)
        {
            this.dispatcher.Remove(evtType, listener);
        }

        public virtual string LocalAddress
        {
            get
            {
                if (parent != null)
                {
                    return this.parent.LocalAddress;
                }
                return String.Empty;
            }
        }
    }
}
