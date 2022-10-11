using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PSXDLL
{
    public class ProxyServer
    {
        /// <summary>
        /// Get local IP
        /// </summary>
        /// <returns></returns>
        public static IPAddress[] GetHostIp()
        {
            IPHostEntry ip = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] iplist = ip.AddressList.Where(p => p.AddressFamily == AddressFamily.InterNetwork).ToArray();
            return iplist;
        }
    }
}
