using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityNet.Core.Remoting;
using UnityNet.Core.Remoting.Support;
using System.Net.Sockets;
using UnityNet.Core.Protocol;
using System.Net;
using UnityNet.Event;
using UnityNet.Common.Utils;
using System.Threading;
using System.Collections;
using UnityNet.Core.Message;
using System.Timers;

namespace UnityNet.Core
{



    public class ExchangeClient : AbstractClient
    {
        private int heatbeatTime;
        private int payload;
        private bool attemptingReconnection;
        private int reconnCounter;
        private readonly double reconnectionDelayMillis;
        private int reconnectionSeconds;
        private DateTime firstReconnAttempt;
        private System.Timers.Timer retryTimer;
        private HeartBeatService heatbeatService;

        public ExchangeClient(AbstractEndpoint parent)
            : base(parent)
        {
            attemptingReconnection = false;
            this.reconnectionSeconds = 0;
            this.attemptingReconnection = false;
            this.firstReconnAttempt = DateTime.MinValue;
            this.reconnCounter = 1;
            this.reconnectionDelayMillis = 1000.0;
        }

        public override void Init()
        {
            if (channel == null)
            {
                this.channel = new TCPChannel();
                this.channel.Log = this.log;
                this.channel.OnConnect = (OnConnectionDelegate)Delegate.Combine(this.channel.OnConnect, new OnConnectionDelegate(base.OnChannelConnect));
                this.channel.OnDisconnect = (OnConnectionDelegate)Delegate.Combine(this.channel.OnDisconnect, new OnConnectionDelegate(OnChannelDisconnect));
                this.channel.OnData = (OnDataDelegate)Delegate.Combine(this.channel.OnData, new OnDataDelegate(this.OnChannelData));
                this.channel.OnError = (OnErrorDelegate)Delegate.Combine(this.channel.OnError, new OnErrorDelegate(OnChannelError));
            }
        }

        public void Kick(string reason)
        {
            //base.channel.Kill();
            //this.ExecuteDisconnection(ClientDisconnectionReason.KICK + "=>>" + reason);
            Dispatch(new GeneralEventArgs(ClientEvent.KICK_CLIENT,reason));
        }

        internal override void OnChannelError(string errorInfo, SocketError error)
        {
            if (this.attemptingReconnection)
            {
                this.Reconnect(base.channel);
            }
            else
            {
                base.OnChannelError(errorInfo, error);
            }
        }

        internal override void OnChannelDisconnect(Channel ch)
        {

            if (heatbeatService != null)
            {
                this.heatbeatService.Stop();
            }

            if (this.reconnectionSeconds == 0)
            {
                this.firstReconnAttempt = DateTime.MinValue;
                this.ExecuteDisconnection(null);
            }
            else if (this.attemptingReconnection)
            {
                this.Reconnect(ch);
            }
            else
            {
                this.attemptingReconnection = true;
                this.firstReconnAttempt = DateTime.Now;
                this.reconnCounter = 1;
                this.Dispatch(new GeneralEventArgs(ClientEvent.RECONNECTION_TRY));
                this.Reconnect(ch);
            }
        }

        private void ExecuteDisconnection(string reason)
        {
            GeneralEventArgs evt = new GeneralEventArgs(ClientEvent.DISCONNECT);
            evt.Parame = reason;
            base.Dispatch(evt);
            base.OnChannelDisconnect(channel);
            ReleaseResources();
        }

        private void Reconnect(Channel channel)
        {
            if (this.attemptingReconnection)
            {
                DateTime now = DateTime.Now;
                TimeSpan span = (TimeSpan)((this.firstReconnAttempt + new TimeSpan(0, 0, reconnectionSeconds)) - now);
                if (span > TimeSpan.Zero)
                {
                    string[] messages = new string[1];
                    object[] objArray1 = new object[] { "Reconnection attempt: counter =>>", this.reconnCounter, " - time left:", span.TotalSeconds, " sec." };
                    messages[0] = string.Concat(objArray1);
                    this.log.Info(messages);
                    this.SetTimeout(new ElapsedEventHandler(this.OnRetryConnectionEvent), this.reconnectionDelayMillis);
                    this.reconnCounter++;
                }
                else
                {
                    this.ExecuteDisconnection(null);
                }
            }
        }

        private void SetTimeout(ElapsedEventHandler handler, double timeout)
        {
            if (this.retryTimer == null)
            {
                this.retryTimer = new System.Timers.Timer(timeout);
                this.retryTimer.Elapsed += handler;
            }
            this.retryTimer.AutoReset = false;
            this.retryTimer.Enabled = true;
            this.retryTimer.Start();
        }

        private void OnRetryConnectionEvent(object source, ElapsedEventArgs e)
        {
            this.retryTimer.Enabled = false;
            this.retryTimer.Stop();
            base.Connect(base.remoteAddress, this.remotePort);
        }

        public override void Close()
        {
            Disconnect(null);
        }

        public override void Close(string reason)
        {
            Disconnect(reason);
        }

        private void Disconnect(string reason)
        {
            base.Close(reason);
            this.ReleaseResources();
        }

        public void KillChannel()
        {
            this.channel.Kill();
            this.OnChannelDisconnect(this.channel);
        }

        public void Destroy()
        {
            base.channel.OnConnect = (OnConnectionDelegate)Delegate.Remove(this.channel.OnConnect, new OnConnectionDelegate(this.OnChannelConnect));
            base.channel.OnDisconnect = (OnConnectionDelegate)Delegate.Remove(this.channel.OnDisconnect, new OnConnectionDelegate(this.OnChannelDisconnect));
            base.channel.OnData = (OnDataDelegate)Delegate.Remove(this.channel.OnData, new OnDataDelegate(this.OnChannelData));
            base.channel.OnError = (OnErrorDelegate)Delegate.Remove(this.channel.OnError, new OnErrorDelegate(this.OnChannelError));
            base.Close();
        }

        private void ReleaseResources()
        {
            if (heatbeatService != null)
            {
                this.heatbeatService.Destroy();
                this.heatbeatService = null;
            }
        }

        public void HandlerHandSnake(HandSnakeResp resp)
        {
            this.heatbeatTime = resp.HeartbeatTime;
            this.payload = resp.Payload;
            if (heatbeatService == null)
            {
                heatbeatService = new HeartBeatService(this, heatbeatTime);
            }
            //heatbeatService.Start();
            UnityClientEvent evt = new UnityClientEvent(UnityClientEvent.HANDSHAKE);
            evt.Success = resp.Success;
            evt.Parame = resp;
            parent.Dispatch(evt);
        }

        public bool IsReconnecting
        {
            get
            {
                return this.attemptingReconnection;
            }
            set
            {
                this.attemptingReconnection = value;
            }
        }

        public int ReconnectionSeconds
        {
            get
            {
                if (this.reconnectionSeconds < 0)
                {
                    return 0;
                }
                return this.reconnectionSeconds;
            }
            set
            {
                this.reconnectionSeconds = value;
            }
        }

        public int Maxpayload
        {
            get { return this.payload; }
        }
    }

    public class HeartBeatService
    {
        private ExchangeClient mClient;
        private System.Timers.Timer mTimer;

        public HeartBeatService(ExchangeClient client, int interval)
        {
            this.mClient = client;
            this.mTimer = new System.Timers.Timer();
            this.mTimer.Enabled = false;
            this.mTimer.AutoReset = true;
            this.mTimer.Elapsed += new ElapsedEventHandler(this.OnPollEvent);
            this.mTimer.Interval = interval;
        }

        public void Start()
        {
            if (!this.IsRunning)
            {
                this.mTimer.Start();
            }
        }

        public void Stop()
        {
            if (this.IsRunning)
            {
                this.mTimer.Stop();
            }
        }

        public void Destroy()
        {
            this.Stop();
            this.mTimer.Dispose();
            this.mTimer = null;
        }

        public bool IsRunning
        {
            get
            {
                return this.mTimer.Enabled;
            }
        }

        private void OnPollEvent(object source, ElapsedEventArgs e)
        {
            this.mClient.Request(new HeartBeatReq(true));
        }
    }
}
