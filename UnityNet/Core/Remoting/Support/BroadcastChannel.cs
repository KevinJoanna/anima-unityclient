using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace UnityNet.Core.Remoting.Support
{
	public class BroadcastChannel : AbstractChannel
	{

		private UdpClient receiveClient ;
        private UdpClient sendClient;
        private IPEndPoint broadcastIEP;
        //private string localIP = "";
		private int listenPort ;

        public BroadcastChannel()
        {
            this.buffer = new byte[DEFAULT_READ_BUFFER_SIZE];
        }

        public override void DoConnect(IPEndPoint endpoint)
        {
			listenPort = endpoint.Port;
			receiveClient = new UdpClient(listenPort);
            broadcastIEP = new IPEndPoint(IPAddress.Broadcast, listenPort);
        }

        public override void DoClose()
        {
            if (receiveClient != null)
            {
                receiveClient.Close();
                receiveClient = null; 
            }
            if (sendClient != null)
            {
                sendClient.Close();
                sendClient = null;
            }
        }

        public override int DoRead(byte[] buf)
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any,listenPort);
            this.buffer = receiveClient.Receive(ref endpoint);
			//IPEndPoint sendEndpoint = ((IPEndPoint)sendAddress) ;
			//if (localIP.Equals(sendEndpoint.Address.ToString()))
			//{/
			    //return 0;		
			//}
            return this.buffer != null ? this.buffer.Length : this.buffer.Length;
        }

        public override void DoWrite(byte[] buf)
        {
			sendClient = new UdpClient();
			if(buffer != null && buffer.Length > 0)
				//receiveClient.Send(buf,buf.Length,new IPEndPoint(IPAddress.Broadcast,listenPort));
           		sendClient.Send(buf, buf.Length, broadcastIEP.Address.ToString(), listenPort);
        }
	}
}

