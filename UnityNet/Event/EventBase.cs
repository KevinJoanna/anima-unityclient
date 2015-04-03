using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace UnityNet.Event
{
    public class EventBase
    {
        protected Hashtable arguments;
        protected object source;
        protected string type;

        public EventBase(string type):this(type,null,null)
        {
            
        }

        public EventBase(string type, Hashtable arguments)
            : this(type, null, arguments)
        {

        }

        public EventBase(string type, object source, Hashtable arguments)
        {
            this.Type = type;
            this.Source = source;
            this.Params = arguments;
        }

        public Hashtable Params
        {
            get
            {
                return this.arguments;
            }
            set
            {
                this.arguments = value;
            }
        }

        public object Source
        {
            get
            {
                return this.source;
            }
            set
            {
                this.source = value;
            }
        }

        public string Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }
    }
}
