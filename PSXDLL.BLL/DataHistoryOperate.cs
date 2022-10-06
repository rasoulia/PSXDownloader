using PSXDLL.DAL;
using PSXDLL.Model;
using System.Collections.Generic;

namespace PSXDLL.BLL
{
    public class DataHistoryOperate
    {
        private static readonly DataHistory DataHistoryInstance = DataHistory.Instance();

        /// <summary>
        ///  add a record
        /// </summary>
        /// <param name="urlInfo"></param>
        /// <returns></returns>
        public static bool AddLog(UrlInfo urlInfo)
        {
            return DataHistoryInstance.AddLog(urlInfo);
        }

        /// <summary>
        ///     update a record
        /// </summary>
        /// <param name="urlInfo"></param>
        /// <returns></returns>
        public static bool UpdataLog(UrlInfo urlInfo)
        {
            return DataHistoryInstance.UpdataLog(urlInfo);
        }

        /// <summary>
        ///     Returns already recorded information
        /// </summary>
        /// <param name="psnurl"></param>
        /// <returns></returns>
        public static UrlInfo? GetInfo(string psnurl)
        {
            return DataHistoryInstance.GetInfo(psnurl);
        }

        /// <summary>
        ///     Get full history
        /// </summary>
        /// <returns></returns>
        public static List<UrlInfo> GetAllUrl()
        {
            return DataHistoryInstance.GetAllUrl();
        }

        /// <summary>
        ///     delete a record
        /// </summary>
        /// <param name="urlInfo"></param>
        /// <returns></returns>
        public static bool DelLog(UrlInfo urlInfo)
        {
            return DataHistoryInstance.DelLog(urlInfo);
        }
    }
}
