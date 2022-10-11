using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSXDLL
{
    public class UrlOperate
    {
        private static readonly HashUrl HashurlOperate = HashUrl.Instance();
        /// <summary>
        /// Find local replacement URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string MatchFile(string url)
        {
            return HashurlOperate.PsnLocalPath(url);
        }

        /// <summary>
        ///     Increase psnurl to correspond to local files
        /// </summary>
        /// <param name="ui"></param>
        /// <returns></returns>
        public static bool PushUrl(UrlInfo ui)
        {
            return HashurlOperate.PushUrl(ui);
        }

        /// <summary>
        ///     Get the local path where there is a replacement connection, if not, return empty
        /// </summary>
        /// <param name="psnurl"></param>
        /// <returns></returns>
        public static string PsnLocalPath(string psnurl)
        {
            return HashurlOperate.PsnLocalPath(psnurl);
        }

        /// <summary>
        /// get filename in url
        /// </summary>
        /// <param name="psnurl"></param>
        /// <returns></returns>
        public static string GetUrlFileName(string psnurl)
        {
            return HashurlOperate.GetUrlFileName(psnurl);
        }
    }
}