using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Core.Remoting;
using UnityNet.Core.Protocol;
using System.Threading;
using UnityNet.Common.Utils;
using System.Net;
using System.Collections;
using System.Net.Sockets;
using UnityNet.Logging;
using UnityNet.Event ;
using UnityNet.Core.Message;

namespace UnityNet.Core
{
	public abstract class AbstractClient :AbstractEndpoint, Client
	{
        protected Channel channel;
        protected string remoteAddress;
        protected int remotePort;
        protected ProtocolCodecFactory codecFactory;
        protected string password;
        protected Queue<object> reqQueue;
        protected object reqQueueLock;
        protected Thread sendThread;
        protected bool isStated = false;
        private DateTime lastConnectTime;
        public AbstractClient(AbstractEndpoint parent)
            : base(parent)
        {
            codecFactory = new DefaultProtocolCodecFactory(this);
            reqQueue = new Queue<object>();
            reqQueueLock = new object();
            Init();
        }

        public abstract void Init();

        public string RemoteAddress
        {
            get { return this.remoteAddress; }
        }

        public int RemotePort
        {
            get { return this.remotePort; }
        }

        public virtual void Connect(string ipAdress, int port, string password)
        {
            this.remoteAddress = ipAdress;
            this.remotePort = port;
            this.password = password;
            this.channel.Connect(new System.Net.IPEndPoint(IPAddress.Parse(ipAdress), port));
            lastConnectTime = DateTime.Now;
        }

        public virtual void Connect(string ipAddress, int port)
        {
            this.Connect(ipAddress, port, null);
        }

        internal virtual void OnChannelConnect(Channel ch)
        {
            StartSendThread();
            GeneralEventArgs evt = new GeneralEventArgs(ClientEvent.CONNECT);
            evt.Success = true;
            DateTime now = DateTime.Now;
            TimeSpan span = (TimeSpan)(now - lastConnectTime);
            log.Info("Connect Time Distance:" + span.TotalMilliseconds);
            base.Dispatch(evt);
        }

        internal virtual void OnChannelDisconnect(Channel ch)
        {
            StopSendThread();
        }

        internal virtual void OnChannelData(Channel ch, byte[] msg)
		{
            try
            {
                ByteArray data = new ByteArray(msg);
                codecFactory.Decode.Decode(ch, data);
            }
            catch (Exception e)
            {
                string[] messages = new string[] { "## ChannelDataError: " + e.Message };
                this.log.Error(messages);
                GeneralEventArgs evt = new GeneralEventArgs(ClientEvent.DATA_ERROR);
                evt.ErrorDes = e.ToString();
                evt.Parame = evt.ErrorDes;
                base.Dispatch(evt);
            }
        }

        internal virtual void OnChannelError(string errorInfo, SocketError error)
        {
            GeneralEventArgs evt = new GeneralEventArgs(ClientEvent.IO_ERROR);
            string message = errorInfo + " : " + error.ToString();
            evt.ErrorDes = message;
            evt.Parame = message;
            base.Dispatch(evt);
        }

        public virtual int Request(string evtType,RequestArg reqArg)
        {
            lock (reqQueueLock)
            {
                Request request = new Request(Int32.Parse(evtType));
                request.TwoWay = true;
                //TODO generate auto increase sequence 
                request.Sequence = 1;
                request.Content = reqArg;
                reqQueue.Enqueue(request);
                Monitor.PulseAll(reqQueueLock);
                return request.Sequence;
            }
        }

        public virtual void Request(RequestArg reqArg)
        {
            lock (reqQueueLock)
            {

                reqQueue.Enqueue(reqArg);
                Monitor.PulseAll(reqQueueLock);
            }
        }

        public int Push(string rId, RequestArg reqArg)
        {
            lock (reqQueueLock)
            {
                Request request = new Request(Int32.Parse(rId));
                request.TwoWay = false;
                //TODO generate auto increase sequence 
                request.Sequence = 1;
                request.Content = reqArg;
                reqQueue.Enqueue(request);
                Monitor.PulseAll(reqQueueLock);
                return request.Sequence;
            }
        }

        private void SendRequestThread()
        {
            while (true)
            {
                if (!isStated)
                {
                    break;
                }
                lock (reqQueueLock)
                {
                    object[] reqs;

                    if (reqQueue.Count == 0)
                    {
                        Monitor.Wait(reqQueueLock);
                    }
                    object queueLock = this.reqQueueLock;
                    lock (queueLock)
                    {
                        reqs = this.reqQueue.ToArray();
                        this.reqQueue.Clear();
					}

                    foreach (object req in reqs)
                    {
                        try
                        {
                            if (!this.IsConnected)
                            {
                                reqQueue.Enqueue(req);
                            }
							ByteArray output = new ByteArray();
                        	codecFactory.Encode.Encode(channel, req, output);
                        	channel.Send(output.Bytes);
						}
						catch(Exception e)
						{
							this.log.Error("Send Request error:" + e.StackTrace);
						}
                    }
                }
            }
        }
		
        protected void StartSendThread()
        {
            if (!isStated)
            {
                isStated = true;
                sendThread = new Thread(new ThreadStart(this.SendRequestThread));
                sendThread.Start();
            }
        }

        protected void StopSendThread()
        {
            if (isStated)
            {
                isStated = false;
                lock (reqQueueLock)
                {
                    Monitor.Pulse(reqQueueLock);
                }
            }
        }

        public virtual void Close()
        {
            if (this.channel != null && this.channel.IsConnected)
            {
                this.channel.Close();
                this.channel = null;
            }
        }

        public virtual void Close(string reason)
        {
            if (this.channel != null && this.channel.IsConnected)
            {
                this.channel.Close(reason);
                this.channel = null;
            }
        }

        public virtual bool IsConnected
        {
            get { return channel.IsConnected; }
        }
    }
}
