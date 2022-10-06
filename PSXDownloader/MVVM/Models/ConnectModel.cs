using System.Net;

namespace PSXDownloader.MVVM.Models
{
    public class ConnectModel
    {
        public int Port { get; set; } = 8080;
        public IPAddress? IP { get; set; }
    }
}
