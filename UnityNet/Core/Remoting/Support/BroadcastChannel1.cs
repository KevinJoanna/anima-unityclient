using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace UnityNet.Core.Remoting.Support
{
    public class BroadcastChannel1 : AbstractChannel
    {
        private Socket client ;
        private IPEndPoint rep;
        private IPEndPoint broadcastIEP;

        public BroadcastChannel1()
        {
            this.buffer = new byte[DEFAULT_READ_BUFFER_SIZE];
        }

        public override void DoConnect(IPEndPoint endpoint)
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            broadcastIEP = new IPEndPoint(IPAddress.Broadcast, endpoint.Port);
            client.Bind(endpoint);
            this.rep = endpoint;
        }

        public override void DoClose()
        {
            client.Close();
        }

        public override int DoRead(byte[] buffer)
        {
            System.Net.EndPoint endpoint = ( System.Net.EndPoint)sendTarget;
            int size = client.ReceiveFrom(buffer, ref endpoint);
            return size;
        }

        public override void DoWrite(byte[] buffer)
        {
            client.SendTo(buffer, broadcastIEP);
        }

        public IPEndPoint Sender
        {
            get
            {
                return this.sendTarget;
            }
        }
    }
}
