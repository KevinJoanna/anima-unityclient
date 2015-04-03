using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace UnityNet.Entities
{
	public class ClientTarget
	{
        internal string ipAddress;
        internal int port;
        internal IPEndPoint endpoint;
        internal long lastHeartTime;

        public ClientTarget()
        {
           
        }

        public ClientTarget(string ipAddress, int port)
        {
            this.ipAddress = ipAddress;
            this.port = port;
        }

        public string IpAddress
        {
            set
            {
                this.ipAddress = value;
            }
            get
            {
                return this.ipAddress;
            }
        }

        public int Port
        {
            set
            {
                this.port = value;
            }
            get
            {
                return this.port;
            }
        }

        public override int GetHashCode()
        {
            return this.ipAddress.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (!(other is ClientTarget))
            {
                return false;
            }
            ClientTarget target = (ClientTarget)other;
            return target.ipAddress.Equals(this.ipAddress);
        }

        public IPEndPoint EndPoint
        {
            get
            {
                if (endpoint == null)
                {
                    endpoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
                }
                return endpoint; 
            }
        }

        public long HeartTime
        {
            get
            {
                return this.lastHeartTime;
            }
            set
            {
                this.lastHeartTime = value;
            }
        }

        public void UpdateHeartTime(long time)
        {
            this.lastHeartTime = time;
        }
	}
}
