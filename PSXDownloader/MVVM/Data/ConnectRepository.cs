using PSXDLL;
using PSXDownloader.MVVM.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PSXDownloader.MVVM.Data
{
    public class ConnectRepository
    {
        private Dictionary<string, IList<PsnUrlLog>> _listUrl = new();
        private static HttpListenerHelp? _listener;

        public void AddLog(string name, string url, string filePath)
        {
            if (!_listUrl.ContainsKey(name))
            {
                _listUrl[name] = new List<PsnUrlLog> { new PsnUrlLog { PsnUrl = url, FilePath = filePath } };
            }
            else if (_listUrl[name].FirstOrDefault(p => p.PsnUrl == url) == null)
            {
                _listUrl[name].Add(new PsnUrlLog { PsnUrl = url, FilePath = filePath });
            }
            else
            {
                return;
            }
        }

        public void ClearLog()
        {
            _listUrl = new();
        }

        public void Connect(IPAddress ip, int port, UpdataUrlLog urlLog)
        {
            if (_listener != null)
            {
                _listener.Dispose();
                _listener = null;
            }
            else
            {
                _listener = new(ip, port, urlLog);
                _listener.Start();
            }
        }

        public Dictionary<string, IList<PsnUrlLog>> GetAllUrl()
        {
            return _listUrl;
        }

        public async Task<string> GetLocalPath(string? titleID)
        {
            using PSXDataContext dataContext = new();
            PSXDatabase? local = dataContext.Set<PSXDatabase>().FirstOrDefault(s => s.TitleID == titleID);
            if (local != null)
            {
                return await Task.FromResult(local.LocalPath!);
            }
            return await Task.FromResult(string.Empty);
        }
    }
}
