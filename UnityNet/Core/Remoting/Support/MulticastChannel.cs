using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace UnityNet.Core.Remoting.Support
{
	public class MulticastEndPoint : AbstractChannel
	{
        private UdpClient client;
        private int bindPort ;
        private IPAddress groupAddress = IPAddress.Parse("239.255.255.255");
        private IPEndPoint clientTarget;

        public override void DoConnect(IPEndPoint endpoint)
        {
            bindPort =  endpoint.Port;
            client = new UdpClient(bindPort, AddressFamily.InterNetwork);
            client.JoinMulticastGroup(groupAddress);
            clientTarget = new IPEndPoint(groupAddress, bindPort);
        }

        public override void DoClose()
        {
            client.DropMulticastGroup(groupAddress);
            client.Close();
            client = null;
        }

        public override int DoRead(byte[] buf)
        {
            int size = 0;
            this.buffer = client.Receive(ref sendTarget);
            size = buffer.Length;
            return size;
        }

        public override void DoWrite(byte[] buffer)
        {
            client.Send(buffer, buffer.Length, clientTarget);
        }
    }
}
