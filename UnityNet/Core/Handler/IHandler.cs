using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Core.Remoting;
using UnityNet.Event;

namespace UnityNet.Core.Handler
{
	public interface IHandler
	{
        void OnReceive(object message);
	}
}
