namespace PSXDLL
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

        /// <summary>
        /// Matching rules
        /// </summary>
        public string? Rule { get; set; } = "*.pkg|*.pup";

        /// <summary>
        /// host
        /// </summary>
        public string? Host { get; set; } = "";

        /// <summary>
        /// Whether to automatically find local files to replace
        /// </summary>
        public bool IsAutoFindFile { get; set; } = true;

        /// <summary>
        /// Local replacement directory
        /// </summary>
        public string? LocalFileDirectory { get; set; } = "D:\\PSXDownloader";

        /// <summary>
        /// Size of Buffer for Transfer Data
        /// </summary>
        public int BufferSize { get; set; } = 512;
    }
}
