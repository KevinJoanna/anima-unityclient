using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using UnityNet.Core;
using UnityNet.Event;
using UnityNet.Logging;
using UnityNet.Core.Handler;
using System.Reflection;
using UnityNet.Core.Message;
using System.Timers;

namespace UnityNet
{
    public class UnityClient : AbstractEndpoint,Client
    {
        private ExchangeClient client;
        private string clientType;
        private int majVersion;
        private int minVersion;
        private int subVersion;
        private const int DEFAULT_PORT = 0x2199; //8601
        private bool inited;
        private bool isConnecting;
        private string lastConnectHost;
        private object eventsLocker;
        private Queue<MyEvent> eventsQueue;
        private string sessionToken;
        private System.Timers.Timer disconnectTimer;

        public UnityClient(bool debug,string clientType)
        {
            base.log = new Logger(this);
            this.clientType = clientType;
            this.majVersion = 0;
            this.minVersion = 0;
            this.subVersion = 1;
            this.inited = false;
            this.isConnecting = false;
            this.eventsLocker = new object();
            this.eventsQueue = new Queue<MyEvent>();
            base.debug = debug;
            if (debug)
            {
                base.log.LoggingLevel = LogLevel.DEBUG;
            }

            this.Init();
        }

        private void Init()
        {
            if (!inited)
            {
                if (dispatcher == null)
                {
                    dispatcher = new EventDispatcher(this);
                }
                client = new ExchangeClient(this);
                client.Logger = log;
                client.Handler = new DefaultHandler(client);

                client.AddEventListener(ClientEvent.CONNECT, new EventListenerDelegate<MyEvent>(OnChannelConnect));
                client.AddEventListener(ClientEvent.DISCONNECT, new EventListenerDelegate<MyEvent>(this.OnChnnelDisconnect));
                client.AddEventListener(ClientEvent.DATA_ERROR, new EventListenerDelegate<MyEvent>(this.OnChnnelDataError));
                client.AddEventListener(ClientEvent.IO_ERROR, new EventListenerDelegate<MyEvent>(this.OnChnnelIOError));
                client.AddEventListener(ClientEvent.RECONNECTION_TRY, new EventListenerDelegate<MyEvent>(this.OnChannelReconnectionTry));
                client.AddEventListener(ClientEvent.KICK_CLIENT, new EventListenerDelegate<MyEvent>(this.HandleKickClient));
                AddEventListener(UnityClientEvent.HANDSHAKE, new EventListenerDelegate<MyEvent>(this.HandlHandSnake));
                this.inited = true;
            }
        }

        public string RemoteAddress
        {
            get { return this.client != null ? this.client.RemoteAddress : "N/A"; }
        }

        public int RemotePort
        {
            get { return this.client != null ? this.client.RemotePort : 0; }
        }

        public void Connect(string host, int port, string password)
        {
            if (this.IsConnected)
            {
                string[] messages = new string[] { "Already connected" };
                this.log.Warn(messages);
            }
            else if (this.isConnecting)
            {
                string[] textArray2 = new string[] { "A connection attempt is already in progress" };
                this.log.Warn(textArray2);
            }
            else
            {
                if ((host == null) || (host.Length == 0))
                {
                    throw new ArgumentException("Invalid connection host/address");
                }
                if ((port < 0) || (port > 0xffff))
                {
                    throw new ArgumentException("Invalid connection port");
                }
                try
                {
                    IPAddress.Parse(host);
                }
                catch (FormatException)
                {
                    try
                    {
                        host = Dns.GetHostEntry(host).AddressList[0].ToString();
                    }
                    catch (Exception exception)
                    {
                        string str = "Failed to lookup hostname " + host + ". Connection failed. Reason " + exception.Message;
                        string[] textArray3 = new string[] { str };
                        this.log.Error(textArray3);
                        UnityClientEvent evt = new UnityClientEvent(UnityClientEvent.CONNECTION);
                        evt.Success = false;
                        evt.ErrorDes = str;
                        this.Dispatch(evt);
                        return;
                    }
                }
                this.lastConnectHost = host;
                this.isConnecting = true;
                this.client.Connect(host, port);
            }
        }

        public void Connect(string host, int port)
        {
            if (this.IsConnected)
            {
                string[] messages = new string[] { "Already connected" };
                this.log.Warn(messages);
            }
            else if (this.isConnecting)
            {
                string[] textArray2 = new string[] { "A connection attempt is already in progress" };
                this.log.Warn(textArray2);
            }
            else
            {
                if ((host == null) || (host.Length == 0))
                {
                    throw new ArgumentException("Invalid connection host/address");
                }
                if ((port < 0) || (port > 0xffff))
                {
                    throw new ArgumentException("Invalid connection port");
                }
                try
                {
                    IPAddress.Parse(host);
                }
                catch (FormatException)
                {
                    try
                    {
                        host = Dns.GetHostEntry(host).AddressList[0].ToString();
                    }
                    catch (Exception exception)
                    {
                        string str = "Failed to lookup hostname " + host + ". Connection failed. Reason " + exception.Message;
                        string[] textArray3 = new string[] { str };
                        this.log.Error(textArray3);
                        UnityClientEvent evt = new UnityClientEvent(UnityClientEvent.CONNECTION);
                        evt.Success = false;
                        evt.ErrorDes = str;
                        this.Dispatch(evt);
                        return;
                    }
                }
                this.lastConnectHost = host;
                this.isConnecting = true;
                this.client.Connect(host, port);
            }
        }

        private void OnChannelConnect(MyEvent evt)
        {
            if (evt.Success)
            {
                this.SendHandshakeRequest(this.client.IsReconnecting);
            }
            else
            {
                string[] messages = new string[] { "Connection attempt failed" };
                this.log.Warn(messages);
                this.HandleConnectionProblem(evt);
            }
        }

        private void OnChnnelDisconnect(MyEvent evt)
        {
            GeneralEventArgs gevt = evt as GeneralEventArgs;
            string reason = gevt.GetParame<string>();
            if (string.IsNullOrEmpty(reason))
            {
                reason = ClientDisconnectionReason.UNKNOWN;
            }
            this.Dispatch(new UnityClientEvent(UnityClientEvent.CONNECTION_LOST, reason));
        }

        private void OnChnnelDataError(MyEvent evt)
        {
            this.Dispatch(new UnityClientEvent(UnityClientEvent.CHANNEL_DATA_ERROR, evt.Parame));
        }

        private void OnChnnelIOError(MyEvent evt)
        {
            if (isConnecting)
            {
                GeneralEventArgs gevt = evt as GeneralEventArgs;
                string message = gevt.Parame as string;
                this.HandleConnectionProblem(gevt);
            }
        }

        private void OnChannelReconnectionTry(MyEvent evt)
        {
            this.Dispatch(new UnityClientEvent(UnityClientEvent.CONNECTION_RETRY));
        }

        private void HandlHandSnake(MyEvent evt)
        {
            UnityClientEvent ucEvt = evt as UnityClientEvent;
            if (evt.Success)
            {
                HandSnakeResp resp = evt.GetParame<HandSnakeResp>();
                this.sessionToken = resp.ReconnectToken;

                if (this.debug)
                {
                    string[] messages = new string[] { string.Format("Handshake response: sessionToken => {0}, heartbeatTime => {1}", this.sessionToken, resp.HeartbeatTime) };
                    this.log.Debug(messages);
                }

                if (this.client.IsReconnecting)
                {
                    this.client.IsReconnecting = false;
                    this.Dispatch(new UnityClientEvent(UnityClientEvent.CONNECTION_RESUME));
                }
                else
                {
                    this.isConnecting = false;
                    this.Dispatch(new UnityClientEvent(UnityClientEvent.CONNECTION));
                }
            }
            else
            {
                UnityClientEvent cEvt = new UnityClientEvent(UnityClientEvent.CONNECTION);
                cEvt.Success = false;
                cEvt.ErrorDes = "Failed to HandSnake";
                this.Dispatch(cEvt);
            }
        }

        private void HandleKickClient(MyEvent evt)
        {
            GeneralEventArgs geva = evt as GeneralEventArgs;
            string reason = geva.Parame as string;
            this.client.ReconnectionSeconds = 0;
            if (this.client.IsConnected)
            {
                this.client.Close(reason);
            }
            if (reason != null)
            {
                this.Dispatch(new UnityClientEvent(UnityClientEvent.CONNECTION_LOST, reason));
            }
        }

        private void HandleConnectionProblem(MyEvent evt)
        {
            GeneralEventArgs event2 = evt as GeneralEventArgs;
            UnityClientEvent ucevt = new UnityClientEvent(UnityClientEvent.CONNECTION);
            ucevt.Success = false;
            ucevt.ErrorDes = event2.ErrorDes;
            ucevt.Parame = event2.Parame;
            this.Dispatch(ucevt);
            this.isConnecting = false;
            this.client.Destroy();
        }

        private void SendHandshakeRequest(bool isReconnection)
        {
            HandSankeReq req = new HandSankeReq();
            req.ClientType = clientType;
            req.ApiVersion = APIVersion;
            if (isReconnection) {
                req.ReconnectToken = this.sessionToken;
            }else {
                req.ReconnectToken = null;
            }

            this.client.Request(req);
        }

        public override void AddEventListener(string evtType, EventListenerDelegate<MyEvent> listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException("listen == null");
            }

            if (!ClientEvent.ContainEventName(evtType) && !UnityClientEvent.ContainEventName(evtType) && !LoggerEvent.ContainEventName(evtType))
            {
                Delegate[] delegateArray = listener.GetInvocationList();
                object[] attributes = delegateArray[0].Method.GetCustomAttributes(typeof(NetCallBackParame), false);
                if (attributes == null || attributes.Length == 0)
                {
                    throw new ArgumentNullException("Failed to add event listener,cause : Cloud not found the delegate method [NetCallBackeParame] attribute!");
                }
                NetCallBackParame netCallBack = (NetCallBackParame)attributes[0];
                Type type = netCallBack.ParameType;
                if (!typeof(ResponseArg).IsAssignableFrom(type))
                {
                    throw new ArgumentNullException("Failed to add event listener,cause : the delegate method attribute parame not extend ResponseArg");
                }

                ResponseMappingInfo.Instance.AddResponeMapping(Int32.Parse(evtType), type);
            }

            base.AddEventListener(evtType, listener);
        }
        public bool IsConnected
        {
            get { return this.client != null ? this.client.IsConnected : false; }
        }

        public int Request(string rId, RequestArg reqArg)
        {
            if (string.IsNullOrEmpty(rId))
            {
                throw new ArgumentNullException("rId == null");
            }

            if (reqArg == null)
            {
                throw new ArgumentNullException("reqArg == null");
            }
            return this.client.Request(rId, reqArg);
        }

        public int Push(string rId, RequestArg reqArg)
        {
            if (string.IsNullOrEmpty(rId))
            {
                throw new ArgumentNullException("rId == null");
            }

            if (reqArg == null)
            {
                throw new ArgumentNullException("reqArg == null");
            }
            return this.client.Request(rId, reqArg);
        }

        internal override void Dispatch(MyEvent eventAgrs)
        {
            this.EnqueueEvent(eventAgrs);
        }

        private void EnqueueEvent(MyEvent evt)
        {
            object eventsLocker = this.eventsLocker;
            lock (eventsLocker)
            {
                this.eventsQueue.Enqueue(evt);
            }
        }

        public void ProcessEvents()
        {

            MyEvent[] eventArray;
            object eventsLocker = this.eventsLocker;
            lock (eventsLocker)
            {
                eventArray = this.eventsQueue.ToArray();
                this.eventsQueue.Clear();
            }
            foreach (MyEvent event2 in eventArray)
            {
                this.Dispatcher.Dispatch(event2);
            }
        }

        public string ClientType
        {
            get { return this.clientType; }
        }

        public string APIVersion
        {
            get
            {
                object[] objArray1 = new object[] { "", this.majVersion, ".", this.minVersion, ".", this.subVersion };
                return string.Concat(objArray1);
            }
        }

        public void Close()
        {
            Close(ClientDisconnectionReason.MANUAL);
        }

        public void Close(int timeout)
        {
            if (this.disconnectTimer == null)
            {
                this.disconnectTimer = new System.Timers.Timer();
            }
            this.disconnectTimer.AutoReset = false;
            this.disconnectTimer.Elapsed += new ElapsedEventHandler(this.OnDisconnectConnectionEvent);
            this.disconnectTimer.Enabled = true;
        }

        private void Close(string reason)
        {
            if (this.IsConnected)
            {
                GeneralEventArgs gevt = new GeneralEventArgs(ClientEvent.KICK_CLIENT, reason);
                this.HandleKickClient(gevt);
            }
            else
            {
                string[] messages = new string[] { "You are not connected" };
                this.log.Info(messages);
            }
        }

        public void ReconnectionSeconds(int sec)
        {
            this.client.ReconnectionSeconds = sec;
        }

        private void OnDisconnectConnectionEvent(object source, ElapsedEventArgs e)
        {
            this.disconnectTimer.Enabled = false;
            this.client.Close(ClientDisconnectionReason.MANUAL);
        }
    }
}
