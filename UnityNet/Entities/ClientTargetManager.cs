using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace UnityNet.Entities
{
	public class ClientTargetManager
	{
        private Dictionary<string,ClientTarget> hostDic ;
        private System.Threading.Timer timer;
        private static readonly int DEFAULT_PERIOD = 3000;         //3 second
        private object objLock;

        public ClientTargetManager()
        {
            hostDic = new Dictionary<string, ClientTarget>();
            timer = new System.Threading.Timer(new TimerCallback(this.OnCallback), null, 0, DEFAULT_PERIOD);
            objLock = new object();
        }

        public ClientTarget FindByIp(string ip)
        {
            if (String.IsNullOrEmpty(ip))
                return null;

            if (hostDic.ContainsKey(ip))
                return hostDic[ip];
            else
                return null;
        }

        public ClientTarget[] GetAllTargets()
        {
            lock (objLock)
            {
                return hostDic.Values.ToArray<ClientTarget>();
            }
        }

        public void Remove(string ip)
        {
            lock (objLock)
            {
                if (hostDic.ContainsKey(ip))
                    hostDic.Remove(ip);
            }
        }

        public void Remove(ClientTarget target)
        {
            Remove(target.ipAddress);
        }

        public void AddTarget(ClientTarget target)
        {
            lock (this.objLock)
            {
                ClientTarget data = FindByIp(target.ipAddress);

                if (data == null)
                {
                    hostDic[target.ipAddress] = target;
                }
                else
                {
                    //update hostdata
                    hostDic[target.ipAddress] = target;
                }
            }
            
        }

        public void Clear()
        {
            lock (this.objLock)
            {
                if (hostDic != null)
                    hostDic.Clear();
            }
            
        }

        private void OnCallback(object state)
        {
            lock (this.objLock)
            {
                long currentTime = DateTime.Now.Ticks / 1000 ;
                foreach (string key in hostDic.Keys)
                {
                    long timeDistance = currentTime - hostDic[key].lastHeartTime;

                    if (timeDistance > DEFAULT_PERIOD)
                    {
                        hostDic.Remove(key);
                    }
                }
            }
        }

        public void Destory()
        {
            
            lock (timer)
            {
                this.Clear();
                hostDic = null;
                if (this.timer != null)
                {
                    this.timer.Dispose();
                    this.timer = null;
                }
            }
        }
	}
}
