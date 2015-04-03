using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace UnityNet.Core.Remoting.Support
{
    public class TCPChannel : AbstractChannel
    {
        private TcpClient connection;
        private NetworkStream networkStream;

        public TCPChannel()
        {
        }

        public override void DoConnect(IPEndPoint endpoint)
        {
            this.connection = new TcpClient();
            this.connection.NoDelay = true;
            this.connection.Client.Connect(endpoint);
            this.networkStream = this.connection.GetStream();
        }

        public override void DoClose()
        {
            if (connection != null)
            {
                this.connection.Client.Shutdown(SocketShutdown.Both);
                this.connection.Close();
                this.networkStream.Close();
            }
        }

        public override int DoRead(byte[] buffer)
        {
            int size = this.networkStream.Read(buffer, 0, DEFAULT_READ_BUFFER_SIZE);
            return size;
        }

        public override void DoWrite(byte[] buffer)
        {
            this.networkStream.Write(buffer, 0, buffer.Length);
        }
    }
}
