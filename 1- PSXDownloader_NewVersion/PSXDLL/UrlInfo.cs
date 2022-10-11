using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSXDLL
{
    public class UrlInfo
    {
        public UrlInfo()
        {
            SetLixian = false;
            IsLixian = false;
        }

        public UrlInfo(string psnurl, string replacepath, string marktxt, bool isLixian = false, string? lixianurl = null)
        {
            SetLixian = false;
            PsnUrl = psnurl;
            ReplacePath = replacepath;
            MarkTxt = marktxt;
            LixianUrl = lixianurl;
            IsLixian = isLixian;
        }

        /// <summary>
        /// PSN connection
        /// </summary>
        public string? PsnUrl { get; set; }

        /// <summary>
        /// Replace the address
        /// </summary>
        public string? ReplacePath { get; set; }

        /// <summary>
        /// Remarks for the current connection
        /// </summary>
        public string? MarkTxt { get; set; }

        /// <summary>
        /// Offline address
        /// </summary>
        public string? LixianUrl { get; set; }

        /// <summary>
        /// Is it a line
        /// </summary>
        public bool IsLixian { get; set; }

        /// <summary>
        /// Add to offline
        /// </summary>
        public bool SetLixian { get; set; }

        /// <summary>
        /// Is it a CDN address?
        /// </summary>
        public bool IsCdn { get; set; }

        /// <summary>
        /// host address
        /// </summary>
        public string? Host { get; set; }
    }
}
