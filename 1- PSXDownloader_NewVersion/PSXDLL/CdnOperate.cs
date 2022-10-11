using System.Net;

namespace PSXDLL
{
    public class CdnOperate
    {
        private static readonly CdnHost CdnInstance = CdnHost.Instance();

        /// <summary>
        /// Read CDN link
        /// </summary>
        public static void ReadCdnConfig()
        {
            CdnInstance.ReadCdnConfig();
        }

        /// <summary>
        /// Get DNS resolution, if there is a CDN host, return the host address, otherwise return the default resolution
        /// </summary>
        /// <param name="host"></param>
        /// <param name="isCdn"></param>
        /// <returns></returns>
        public static IPAddress GetCdnAddress(string host, out bool isCdn)
        {
            return CdnInstance.GetCdnAddress(host, out isCdn);
        }

        /// <summary>
        /// Export the final CDN list
        /// </summary>
        /// <returns></returns>
        public static bool ExportHost()
        {
            return CdnInstance.ExportHost();
        }
    }
}

