using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityNet.Entities
{
	public class HostManager
	{
        private Dictionary<string,HostData> hostDic ;

        public HostManager()
        {
            hostDic = new Dictionary<string,HostData>();
        }

        public HostData FindByIp(string ip)
        {
            if (String.IsNullOrEmpty(ip))
                return null;

            if(hostDic.ContainsKey(ip))
                return hostDic[ip];
            else
                return null;
        }

        public HostData[] GetAllHosts()
        {
            return hostDic.Values.ToArray<HostData>();
        }

        public void Remove(string ip)
        {
            if (hostDic.ContainsKey(ip))
                hostDic.Remove(ip);
        }

        public void Remove(HostData host)
        {
            Remove(host.Ip);
        }

        public void AddHost(HostData host)
        {
            HostData data = FindByIp(host.Ip);

            if (data == null)
            {
                hostDic[host.ip] = host;
            }
            else
            {
                //update hostdata
                hostDic[host.ip] = host;
            }
        }

        public void Clear()
        {
            if (hostDic != null)
                hostDic.Clear();
        }
	}
}
