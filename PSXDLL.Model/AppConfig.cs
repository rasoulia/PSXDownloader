using System;

namespace PSXDLL.Model
{
    public class AppConfig
    {
        private static AppConfig? _instance;
        private static readonly object Lock = new();
        public static AppConfig Instance()
        {
            if (_instance == null)
            {
                lock (Lock)
                {
                    _instance = new AppConfig();
                }
            }

            return _instance;
        }

        public string? Language { get; set; }
        /// <summary>
        /// Matching rules
        /// </summary>
        public string? Rule { get; set; }

        /// <summary>
        /// host
        /// </summary>
        public string? Host { get; set; }

        /// <summary>
        /// login information
        /// </summary>
        public string? Cookie { get; set; }

        private static bool _enablelixian;
        /// <summary>
        /// Whether to enable offline
        /// </summary>
        public bool EnableLixian
        {
            get { return _enablelixian; }
            set { _enablelixian = value; }
        }

        /// <summary>
        /// connection mode
        /// </summary>
        public int ConnType { get; set; }

        public bool IsUsePcProxy { get; set; }

        public bool IsUseLixian { get; set; }

        public bool IsUserLocal { get; set; }

        public string? Ip { get; set; }

        public int Port { get; set; }

        /// <summary>
        /// Whether to enable CDN acceleration
        /// </summary>
        public bool IsUseCdn { get; set; }

        /// <summary>
        /// The last time the CDN host list was checked
        /// </summary>
        public DateTime LastCheckCdn { get; set; }

        public bool IsAutoCheckUpdate { get; set; }

        public DateTime LastCheckUpdate { get; set; }

        public string? Ssid { get; set; }

        public string? Theme { get; set; }

        public string? ApPassword { get; set; }

        /// <summary>
        /// Whether fuzzy match
        /// </summary>
        public bool IsDimrule { get; set; }

        /// <summary>
        /// Whether to use a custom host address
        /// </summary>
        public bool IsUseCustomeHosts { get; set; }

        /// <summary>
        /// Whether to automatically find local files to replace
        /// </summary>
        public bool IsAutoFindFile { get; set; }

        /// <summary>
        /// Local replacement directory
        /// </summary>
        public string? LocalFileDirectory { get; set; }
    }
}