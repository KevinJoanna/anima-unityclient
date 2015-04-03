using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace UnityNet.Core.Remoting.Support
{
    public class UDPChannel : AbstractChannel
    {
        private UdpClient client;
        private IPEndPoint sender;
		private UdpClient senderTarget ;
        public UDPChannel()
        {
        }

        public override void DoConnect(IPEndPoint endpoint)
        {
			client = new UdpClient(endpoint.Port);
			senderTarget = new UdpClient();
            sender = new IPEndPoint(IPAddress.Any, 0);
        }
		
		
		protected override void Read()
        {
            while (true)
            {
                try
                {
                    if (this.CurrentState != States.Connected)
                    {
                        break;
                    }

                    int size = DoRead(this.buffer);
					
                    if (size < 1) 
					{
						//远程主机连接断开连接
						this.HandleOnError("Connect disconnect by the server");
                        break;
					}

                    this.HandleOnData(this.buffer, size);
					continue;
                }
                catch (SocketException se)
                {
                    this.HandleOnError("Error reading data from socket: " + se.Message, se.SocketErrorCode);
                    continue;
                }
                catch (System.Exception ex)
                {
                    this.HandleOnError("General error reading data from socket: " + ex.Message + " " + ex.StackTrace);
                    continue;
                }
            }
        }

        protected override void HandleOnError(string msg, SocketError se)
        {
            this.fsm.ApplyTransition(Transitions.ConnectionFailure);
            if (!this.isDisconnecting)
            {
                this.LogError(msg);
                if (this.onErrorHandler != null)
                {
                    this.onErrorHandler(msg, se);
                }
            }
        }

        protected override void HandleOnError(string msg)
        {
            HandleOnError(msg, SocketError.NotSocket);
        }

        public override void DoClose()
        {
			if (this.client != null)
			{
				this.client.Close();
			}
        }

        public override int DoRead(byte[] buffer)
        {
            this.buffer = this.client.Receive(ref this.sender);
            return this.buffer.Length;
        }

        public override void DoWrite(byte[] buf)
        {
            this.client.Send(buf, buf.Length);
        }
		
		public void WriteTo(byte[] buf,IPEndPoint endpoint)
		{
			try
			{
				this.senderTarget.Send(buf,buf.Length,endpoint);
			}catch(Exception e)
			{
				 this.HandleOnError("Send message error:" + e.Message);
			}
			
		}
    }
}
