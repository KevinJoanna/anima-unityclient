using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Core.Remoting;
using UnityNet.Logging;
using UnityNet.Event;

namespace UnityNet.Core.Handler
{
	public abstract class AbstractHandler : IHandler
	{
        protected Endpoint endpoint;
        protected Logger log;
        protected EventDispatcher dispatcher;

        public AbstractHandler(Endpoint endpoint)
        {
            this.endpoint = endpoint;
            this.log = endpoint.Logger;
            dispatcher = new EventDispatcher(this);
        }

        public virtual void OnReceive(object messag)
        {

        }
    }
}
