using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Core.Message;

namespace UnityNet.Core
{
	public interface Client : Endpoint 
	{
        string RemoteAddress { get; }

        int RemotePort { get; }

        void Connect(string host, int port, string password);

        void Connect(string host, int port);

        int Request(string rId,RequestArg reqArg);

        int Push(string rId, RequestArg reqArg);

        void Close();

        bool IsConnected { get; }
    }
}
