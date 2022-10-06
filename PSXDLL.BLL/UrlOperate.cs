using PSXDLL.DAL;
using PSXDLL.Model;

namespace PSXDLL.BLL
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


        public static LixianUrl LixianInstance = LixianUrl.Instance();

        /// <summary>
        ///     Deposit offline address
        /// </summary>
        /// <param name="psnurl"></param>
        /// <param name="lixianurl"></param>
        /// <returns></returns>
        public static bool PushLixianUrl(string psnurl, string lixianurl)
        {
            return LixianInstance.PushLixianUrl(psnurl, lixianurl);
        }

        /// <summary>
        ///     Get existing offline paths
        /// </summary>
        /// <param name="psnurl"></param>
        /// <param name="lixianurl"></param>
        /// <returns></returns>
        public static bool GetLixianUrl(string psnurl, out string? lixianurl)
        {
            return LixianInstance.GetLixianUrl(psnurl, out lixianurl);
        }

        /// <summary>
        /// Get offline request headers
        /// </summary>
        /// <param name="query"></param>
        /// <param name="uinfo"></param>
        /// <returns></returns>
        public static string GetQuery(string query, ref UrlInfo uinfo)
        {
            return LixianInstance.GetQuery(query, ref uinfo);
        }
    }
}
