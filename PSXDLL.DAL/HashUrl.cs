using PSXDLL.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PSXDLL.DAL
{
    public class HashUrl
    {
        public static Hashtable? UrlList;
        private static HashUrl? _instance;
        private static readonly object Lock = new();

        public static HashUrl Instance()
        {
            if (_instance == null)
            {
                lock (Lock)
                {
                    _instance = new HashUrl();
                    UrlList ??= new Hashtable();
                }
            }

            return _instance;
        }

        /// <summary>
        ///     Increase psnurl to correspond to local files
        /// </summary>
        /// <param name="ui"></param>
        /// <returns></returns>
        public bool PushUrl(UrlInfo ui)
        {
            try
            {
                if (UrlList!.ContainsKey(ui.PsnUrl!))
                {
                    UrlList[ui.PsnUrl!] = ui.ReplacePath;
                }
                else
                {
                    UrlList.Add(ui.PsnUrl!, ui.ReplacePath);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get the local path where there is a replacement connection, if not, return empty
        /// </summary>
        /// <param name="psnurl"></param>
        /// <returns></returns>
        public string PsnLocalPath(string psnurl)
        {
            try
            {
                UrlInfo? temp = DataHistory.Instance().GetInfo(psnurl);
                if (temp != null && !string.IsNullOrEmpty(temp.ReplacePath))
                {
                    return temp.ReplacePath;
                }
                if (!AppConfig.Instance().IsAutoFindFile)
                {
                    return string.Empty;
                }

                //If you turn on automatic matching of local files, then automatically find
                string filename = GetUrlFileName(psnurl);
                if (!string.IsNullOrEmpty(filename))
                {
                    return AppConfig.Instance().LocalFileDirectory + "\\" + filename;
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// get filename in url
        /// </summary>
        /// <param name="psnurl"></param>
        /// <returns></returns>
        public string GetUrlFileName(string psnurl)
        {
            List<string>? rules = AppConfig.Instance().Rule!.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (rules.Count <= 0)
            {
                return string.Empty;
            }

            rules = rules.Select(r => r.Replace("*", "")).ToList();
            string filename = string.Empty;
            rules.ForEach(r =>
            {
                if (psnurl.IndexOf(r) > 0)
                {
                    filename = psnurl.Substring(0, psnurl.IndexOf(r) + r.Length);
                    filename = filename.Substring(filename.LastIndexOf("/") + 1);
                }
            });

            return filename;
        }
    }
}
