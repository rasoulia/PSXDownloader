namespace PSXDLL
{
    public class UrlInfo
    {
        public UrlInfo() { }

        public UrlInfo(string psnurl, string replacepath)
        {
            PsnUrl = psnurl;
            ReplacePath = replacepath;
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
        /// host address
        /// </summary>
        public string? Host { get; set; }
    }
}
