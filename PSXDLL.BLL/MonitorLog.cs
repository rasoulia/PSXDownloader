using PSXDLL.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace PSXDLL.BLL
{
    public class MonitorLog
    {
        /// <summary>
        /// language
        /// </summary>
        public static readonly Dictionary<int, string> Languages = new()
        {
                {0,"zh-CHS"},
                {1,"zh-CHT"},
                {2,"pt-BR"},
                {3,"en"}
            };

        /// <summary>
        /// theme color
        /// </summary>
        public static readonly List<string> Themes = new()
        {
                "#46B9F0",
                "#E98EA8",
                "#F2886E",
                "#F4AA6E",
                "#89C97D",
                "#66C0B8",
                "#BB9EC3",
                "#6AC9E0",
                "#F65314",
                "#7CBB00",
                "#00A1F1",
                "#FFBB00"
            };

        /// <summary>
        /// matching link
        /// </summary>
        /// <returns></returns>
        public static bool RegexUrl(string urls)
        {
            string[]? rules = AppConfig.Instance().Rule!.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (rules.Length <= 0 || string.IsNullOrEmpty(urls))
            {
                return false;
            }

            return
                rules.Select(rule => new Regex(rule.ToLower().Replace(".", @"\.").Replace("*", ".*?")))
                     .Any(regex => regex.Match(urls.ToLower()).Success);
        }

        private static string updatelog = "";
        private static int? version;

        /// <summary>
        /// Get the latest version number
        /// </summary>
        /// <returns></returns>
        public static int GetNewVersion()
        {
            /*
             * Version update check, if you provide this service, please add it yourself。
             * The following code is the original check and update code
             */
            return 0;

            if (version.HasValue)
            {
                return version.Value;
            }

            const string url = "http://blog.acgpedia.com/extensions/service/update.txt";
            string strValue = GetWebContent(url);
            Regex regexversion = new(@"#Version:(.*?)#PSX Download Helper", RegexOptions.Singleline);
            string newversion = regexversion.Match(strValue).Groups[1].Value.Replace(".", "");
            version = int.Parse(newversion);
            return version.Value;
        }

        /// <summary>
        ///Get check update version information
        /// </summary>
        /// <returns></returns>
        public static string GetUpdateLog()
        {
            if (!string.IsNullOrEmpty(updatelog))
            {
                return updatelog;
            }

            const string strUrl = "http://blog.acgpedia.com/extensions/service/update.txt";
            string strValue = GetWebContent(strUrl);
            Regex regupdate = new(@"<div id=""newversion"">.*?</div>", RegexOptions.Singleline);
            updatelog = regupdate.Match(strValue).Value;
            return updatelog;
        }

        /// <summary>
        ///update cdn list
        /// </summary>
        /// <returns></returns>
        public static bool UpdateCdn()
        {
            try
            {
                string cdnlist = GetWebContent("http://blog.acgpedia.com/extensions/service/cdnhosts.txt");
                StreamWriter sw = new(@"Hosts\CdnHosts.ini", false);
                sw.Write(cdnlist);
                sw.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get web content
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string GetWebContent(string url)
        {
            WebClient? wc = new() { Credentials = CredentialCache.DefaultCredentials };
            Encoding enc = Encoding.GetEncoding("UTF-8");
            byte[] pageData = wc.DownloadData(url); // Download data from the resource and return a byte array.
            string strValue = enc.GetString(pageData);
            return strValue;
        }
    }
}
