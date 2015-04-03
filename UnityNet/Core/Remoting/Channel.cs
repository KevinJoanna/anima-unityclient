using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityNet.Logging;

namespace UnityNet.Core.Remoting
{

    #region Channel Delegate

    /// <summary>
    /// This delegate used to received the message from the remote server.
    /// </summary>
    /// <param name="message"></param>
    public delegate void OnDataDelegate(Channel ch, byte[] message);
    /// <summary>
    ///This delegate Used to connected or disconnected to the server
    /// </summary>
    /// <param name="ipEndPoint"></param>
    public delegate void OnConnectionDelegate(Channel ch);
    /// <summary>
    /// This delegate Used to failed 
    /// </summary>
    /// <param name="errorInfo"></param>
    /// <param name="error"></param>
    public delegate void OnErrorDelegate(string errorInfo, SocketError error);

    #endregion
   
    
    public interface Channel
    {
        /// <summary>
        /// Connect remote server
        /// </summary>
        /// <param name="endpoint">remote endpoint</param>
        void Connect(IPEndPoint endpoint);
        /// <summary>
        /// Write data to this channel
        /// </summary>
        /// <param name="data">message byte[] data</param>
        void Send(byte[] data);
        /// <summary>
        /// Close channel
        /// </summary>
        void Close();
        /// <summary>
        /// Close channel with the reason
        /// </summary>
        /// <param name="reason"></param>
        void Close(string reason);
        /// <summary>
        /// Kill this channel
        /// </summary>
        void Kill();

        IPEndPoint SendTaget
        {
            get;
        }

        IPEndPoint IPEenPoint
        {
            get;
        }

        OnDataDelegate OnData
        {
            set;
            get;
        }

        OnErrorDelegate OnError
        {
            set;
            get;
        }

        OnConnectionDelegate OnConnect
        {
            set;
            get;
        }

        OnConnectionDelegate OnDisconnect
        {
            set;
            get;
        }

        bool IsConnected
        {
            get;
        }

        Logger Log
        {
            set;
            get;
        }
    }
}
