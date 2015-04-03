using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityNet.Logging;
using System.Threading;
using UnityNet.FSM;

namespace UnityNet.Core.Remoting
{
    public abstract class AbstractChannel : Channel
    {
        protected static readonly int DEFAULT_READ_BUFFER_SIZE = 0x1000;
        protected FiniteStateMachine fsm;
        protected IPEndPoint endpoint;
        protected Logger log;
        protected OnDataDelegate onDataHandler;
        protected OnConnectionDelegate onConnectHandler;
        protected OnConnectionDelegate OnDisconnectHandler;
        protected OnErrorDelegate onErrorHandler;
        protected Thread thrSocketReader;
        protected Thread thrConnect;
        protected bool isDisconnecting;
        protected byte[] buffer = new byte[DEFAULT_READ_BUFFER_SIZE];
        protected IPEndPoint sendTarget = new IPEndPoint(IPAddress.Any,0);

        public enum States
        {
            Disconnected,
            Connecting,
            Connected
        }

        public enum Transitions
        {
            StartConnect,
            ConnectionSuccess,
            ConnectionFailure,
            Disconnect
        }

        public AbstractChannel()
        {
            this.InitStates();
        }

        public States CurrentState
        {
            get
            {
                return (States)this.fsm.GetCurrentState();
            }
        }

        private void InitStates()
        {
            this.fsm = new FiniteStateMachine();
            this.fsm.AddAllStates(typeof(States));
            this.fsm.AddStateTransition(States.Disconnected, States.Connecting, Transitions.StartConnect);
            this.fsm.AddStateTransition(States.Connecting, States.Connected, Transitions.ConnectionSuccess);
            this.fsm.AddStateTransition(States.Connecting, States.Disconnected, Transitions.ConnectionFailure);
            this.fsm.AddStateTransition(States.Connected, States.Disconnected, Transitions.Disconnect);
            this.fsm.SetCurrentState(States.Disconnected);
        }

        public void Connect(IPEndPoint endpoint)
        {
            if (this.CurrentState != States.Disconnected)
            {
                this.LogWarn("Calling connect when the socket is not disconnected");
            }
            else
            {
                this.endpoint = endpoint;
                this.fsm.ApplyTransition(Transitions.StartConnect);
                this.thrConnect = new Thread(new ThreadStart(this.ConnectThread));
                this.thrConnect.Start();
            }
        }

        public void Send(byte[] data)
        {
            if (this.CurrentState != States.Connected)
            {
                this.LogWarn("Calling Write when the socket is not connected");
                return;
            }

            try
            {
                 DoWrite(data);
            }
			catch(SocketException e)
			{
                string err = "Error writing to socket: " + e.Message;
                this.HandleOnError(err, e.SocketErrorCode);
            }
            catch (Exception e)
            {
                string err = "Error writing to socket: " + e.Message;
                this.HandleOnError(err);
            }
        }

        public void Close()
        {
            Close(null);
        }

        public void Close(string reason)
        {
            if (this.CurrentState != States.Connected)
            {
                this.LogWarn("Calling disconnect when the socket is not connected");
            }
            else
            {
                this.isDisconnecting = true;
                try
                {
                    DoClose();
                }
                catch (Exception)
                {
                    this.LogWarn("Trying to disconnect a non-connected tcp socket");
                }
                this.HandleOnDisConnect();
                this.isDisconnecting = false;
            }
        }

        public void Kill()
        {
            this.fsm.ApplyTransition(Transitions.Disconnect);
            DoClose();
        }

        private void ConnectThread()
        {
            try
            {   
                DoConnect(endpoint);
                this.fsm.ApplyTransition(Transitions.ConnectionSuccess);
                this.HandleOnConnect();
                this.thrSocketReader = new Thread(new ThreadStart(this.Read));
                this.thrSocketReader.Start();
            }
            catch (SocketException se)
            {
                string err = "Connection error: " + se.Message + " " + se.StackTrace;
                this.HandleOnError(err, se.SocketErrorCode);
            }
            catch (Exception e)
            {
                string str2 = "General exception on connection: " + e.Message + " " + e.StackTrace;
                this.HandleOnError(str2);
            }
        }

        protected virtual void Read()
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
						//Remote server side close channel
                        this.HandleOnError("Connection closed by the remote side");
                        break;
					}

                    this.HandleOnData(this.buffer, size);
                }
                catch (SocketException se)
                {
                    this.HandleOnError("Error reading data from socket: " + se.Message, se.SocketErrorCode);
                }
                catch (System.Exception ex)
                {
                    this.HandleOnError("General error reading data from socket: " + ex.Message + " " + ex.StackTrace);
                }
            }
        }

        public abstract void DoConnect(IPEndPoint endpoint);

        public abstract void DoClose();

        public abstract int DoRead(byte[] buffer);

        public abstract void DoWrite(byte[] buffer);

        protected virtual void HandleOnData(byte[] buf, int size)
        {
            if (onDataHandler != null)
            {
                byte[] dst = new byte[size];
                Buffer.BlockCopy(buf, 0, dst, 0, size);
                this.onDataHandler(this,dst);
            }
        }

        protected void HandleOnConnect()
        {
            if (onConnectHandler != null)
            {
                this.onConnectHandler(this);
            }
        }

        protected void HandleOnDisConnect()
        {
            if (CurrentState != States.Disconnected)
            {
                this.fsm.ApplyTransition(Transitions.Disconnect);
                if (OnDisconnectHandler != null)
                {
                    this.OnDisconnectHandler(this);
                }
            }
        }

        protected virtual void HandleOnError(string msg, SocketError se)
        {
            if (CurrentState == States.Connecting)
            {
                this.fsm.ApplyTransition(Transitions.ConnectionFailure);
            }
            if (!this.isDisconnecting)
            {
                this.LogError(msg);
                if (this.onErrorHandler != null)
                {
                    this.onErrorHandler(msg, se);
                }
            }
            this.HandleOnDisConnect();
        }

        public IPEndPoint SendTaget
        {
            get { return this.sendTarget; }
        }

        protected virtual void HandleOnError(string msg)
        {
            HandleOnError(msg, SocketError.NotSocket);
        }

        public IPEndPoint IPEenPoint
        {
            get
            {
                return this.endpoint;
            }
        }

        public OnDataDelegate OnData
        {
            set
            {
                this.onDataHandler = value;
            }
            get
            {
                return this.onDataHandler;
            }
        }

        public OnErrorDelegate OnError
        {
            set
            {
                this.onErrorHandler = value;
            }
            get
            {
                return this.onErrorHandler;
            }
        }

        public OnConnectionDelegate OnConnect
        {
            set
            {
                this.onConnectHandler = value;
            }
            get
            {
                return this.onConnectHandler;
            }
        }

        public OnConnectionDelegate OnDisconnect
        {
            set
            {
                this.OnDisconnectHandler = value;
            }
            get
            {
                return this.OnDisconnectHandler;
            }
        }

        public bool IsConnected
        {
            get
            {
                return (this.CurrentState == States.Connected);
            }
        }

        public Logger Log
        {
            set 
            {
                this.log = value;
            }
            get
            {
                return this.log;
            }
        }

        protected void LogError(string msg)
        {
            if (this.log != null)
            {
                string[] messages = new string[] { "AbstractEndPoint: " + msg };
                this.log.Error(messages);
            }
        }

        protected void LogWarn(string msg)
        {
            if (this.log != null)
            {
                string[] messages = new string[] { "AbstractEndPoint: " + msg };
                this.log.Warn(messages);
            }
        }
    }
}
