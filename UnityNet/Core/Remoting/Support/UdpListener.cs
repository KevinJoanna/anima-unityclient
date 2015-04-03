using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityNet.Logging;
using System.Threading;

namespace UnityNet.Core.Remoting.Support
{
    public delegate void OnServerStarted() ;

    public delegate void OnData(IPEndPoint sender,byte[] data);

    public delegate void OnError(string message,SocketError error);

    public delegate void OnServerStoped() ;

	internal class UdpListener 
	{
        private Socket server;
        private IPEndPoint sender;
        private string listenIp ;
        private int listenPort;
        private Logger log;
        private Endpoint parent;
        private bool isStarted;
        private Thread readThread;
        private OnServerStarted serverStarted;
        private OnServerStoped serverStoped;
        private OnData dataDelegate;
        private OnError errorDelegate;
        private static readonly int BUFFER_SIZE = 0x1000 ;
        private byte[] buf = new byte[BUFFER_SIZE];
		private UdpClient client;

        public UdpListener(Endpoint endpoint)
        {
            this.parent = endpoint;
            this.log = endpoint.Logger;
        }

        public void Bind(string ipAddress,int port)
        {
            try
            {
                this.listenIp = ipAddress;
                this.listenPort = port;
                server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                sender = new IPEndPoint(IPAddress.Any, 0);
                server.Bind(new IPEndPoint(IPAddress.Parse(listenIp), listenPort));
				client = new UdpClient();
                isStarted = true;
                if (this.serverStarted != null)
                {
                    this.serverStarted();
                }
                readThread = new Thread(new ThreadStart(this.Read));
                readThread.Start();
            }
            catch(SocketException se)
            {
                string err = "Socket bind error: " + se.Message + " " + se.StackTrace;
                this.HandleOnError(err, se.SocketErrorCode);
            }
            catch(Exception e)
            {
                string str2 = "General exception on connection: " + e.Message + " " + e.StackTrace;
                this.HandleOnError(str2);
            }
        }

        protected void HandleOnError(string msg, SocketError se)
        {  
            this.Stop();
            this.LogError(msg);
            if (errorDelegate != null)
            {
                errorDelegate(msg, se);
            }
        }

        private void LogError(string msg)
        {
            if (this.log != null)
            {
                string[] messages = new string[] { "UdpListener: " + msg };
                this.log.Error(messages);
            }
        }

        private void LogWarn(string msg)
        {
            if (this.log != null)
            {
                string[] messages = new string[] { "UdpListener: " + msg };
                this.log.Warn(messages);
            }
        }

        protected void HandleOnError(string msg)
        {
            HandleOnError(msg, SocketError.NotSocket);
        }

        public void Stop()
        {
            if (isStarted)
            {
                try
                {
                    isStarted = false;
					readThread.Abort();
                    if (this.serverStoped != null)
                    {
                        this.serverStoped();
                    }
					if (this.server != null)
					{
						this.server.Close();
						this.server = null;
					}
                }
                catch (Exception e)
                {
                    string messages =  "Stop Udp server error: " + e.Message ;
                    this.LogWarn(messages);
                }
            }
        }

        private void Read()
        {
            while (true)
            {
                try
                {
					
					if (!isStarted)
						break; 
					
                    System.Net.EndPoint  s = (System.Net.EndPoint)sender;
                    int size = server.ReceiveFrom(buf, ref s);
                    if (size < 1)
                    {
                        this.HandleOnError("UpdSever read data error,Cause ") ;
                        break;
                    }
                    this.HandleOnData(sender, this.buf, size);
                }
                catch (SocketException se)
                {
                    this.HandleOnError("Error reading data from socket: " + se.Message, se.SocketErrorCode);
                }
                catch (Exception e)
                {
                    string str2 = "General exception on reader: " + e.Message + " " + e.StackTrace;
                    this.HandleOnError(str2);
                }
            }
        }
		
		public void Write(IPEndPoint endpoint,byte[] data)
		{
			if (this.IsStarted)
			{
				client.Send(data,data.Length,endpoint);
			}
		}
		
        protected void HandleOnData(IPEndPoint sender , byte[] buf, int size)
        {
            if (dataDelegate != null)
            {
                byte[] dst = new byte[size];
                Buffer.BlockCopy(buf, 0, dst, 0, size);
                this.dataDelegate(sender, dst);
            }
        }

        public OnServerStarted ServerStarted
        {
            set
            {
                this.serverStarted = value;
            }
            get
            {
                return this.serverStarted;
            }
        }

        public OnServerStoped ServerStoped
        {
            set
            {
                this.serverStoped = value;
            }
            get
            {
                return this.serverStoped;
            }
        }

        public OnData OnData
        {
            set
            {
                this.dataDelegate = value;
            }
            get
            {
                return this.dataDelegate;
            }
        }

        public OnError OnError
        {
            set
            {
                this.errorDelegate = value;
            }
            get
            {
                return this.errorDelegate;
            }
        }

        public bool IsStarted
        {
            get
            {
                return this.isStarted;
            }
        }
	}
}
