using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace PSXDLL.BLL
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
