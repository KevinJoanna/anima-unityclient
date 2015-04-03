using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Logging;
using UnityNet.Event;
using UnityNet.Core.Handler;

namespace UnityNet.Core
{
    public interface Endpoint : IDispatchable
	{
        Logger Logger { set; get; }

        LogLevel LogLevel { set; get; }

        IHandler Handler { set; get; }

        bool Debug { get; }

        string LocalAddress { get; }
	}
}
