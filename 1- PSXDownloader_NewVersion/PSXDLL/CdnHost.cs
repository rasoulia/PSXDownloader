using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PSXDLL
{
    public class CdnHost
    {
        private static Hashtable _cdnhost = new();
        private static Hashtable _blur = new();
        private static Hashtable _temp = new();
        private static object _lock = new();
        private static CdnHost? _instance;

        public Hashtable CdnHostList => _cdnhost;

        public static CdnHost Instance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    return _instance = new CdnHost();
                }
            }

            return _instance;
        }
        /// <summary>
        /// Read CDN link
        /// </summary>
        public void ReadCdnConfig()
        {
            _cdnhost.Clear();
            _blur.Clear();
            _temp.Clear();
            ReadCdnConfig(@"Hosts\CdnHosts.ini", true);//Read the default host list
            if (AppConfig.Instance().IsUseCustomeHosts)
            {
                ReadCdnConfig(@"Hosts\CustomHosts.ini", false);//Read custom host list
            }
        }

        /// <summary>
        /// Read host information
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isPing"></param>
        static void ReadCdnConfig(string path, bool isPing)
        {
            if (!File.Exists(path))
            {
                throw new Exception(path + "config file not found！");
            }

            using StreamReader sr = new(path, System.Text.Encoding.UTF8);
            while (sr.Peek() > 0)
            {
                string? lineStr = sr.ReadLine();
                if (string.IsNullOrEmpty(lineStr) || lineStr.StartsWith("#"))
                {
                    continue;
                }

                Task t = new(delegate ()
                {
                    bool isForceUse = lineStr.StartsWith("``");
                    Regex regex = new(@"\s*\s");
                    lineStr = regex.Replace(lineStr.Replace("``", ""), "&");
                    string[] cdnInfo = lineStr.Split('&');
                    try
                    {
                        if (cdnInfo.Length < 2)
                        {
                            return;
                        }

                        PingReply? reply = null;
                        if (isPing && !isForceUse)
                        {
                            reply = PingHost(cdnInfo[0], cdnInfo[1]);
                            if (reply == null)
                            {
                                return;
                            }

                            cdnInfo[0] = reply.Address.ToString();
                        }
                        BuildCdnList(cdnInfo, reply!);
                    }
                    catch
                    {
                        return;
                    }
                });
                t.Start();
            }
        }

        static object _buildlock = new();
        /// <summary>
        /// Create a hashtable after comparing the results of the returned ping values
        /// </summary>
        /// <param name="cdnInfo"></param>
        /// <param name="reply"></param>
        static void BuildCdnList(string[] cdnInfo, PingReply reply)
        {
            lock (_buildlock)
            {
                if (reply == null)
                {
                    AddToHashTable(cdnInfo);
                    return;
                }

                if (!_temp.ContainsKey(cdnInfo[1]))
                {
                    AddToHashTable(cdnInfo);
                    _temp.Add(cdnInfo[1], reply.RoundtripTime);
                }
                else if ((long)_temp[cdnInfo[1]]! > reply.RoundtripTime)
                {
                    AddToHashTable(cdnInfo);
                    _temp[cdnInfo[1]] = reply.RoundtripTime;
                }
            }
        }
        /// <summary>
        /// Store the host address and the corresponding domain name in the hashtable
        /// </summary>
        /// <param name="cdnInfo"></param>
        static void AddToHashTable(string[] cdnInfo)
        {
            if (cdnInfo[1].StartsWith("*."))
            {
                AddToBlur(cdnInfo[1], cdnInfo[0]);
            }
            else
            {
                AddToCdnHost(cdnInfo[1], cdnInfo[0]);
            }
        }
        static void AddToBlur(string key, string value)
        {
            key = key.Replace("*.", "");
            if (_blur.ContainsKey(key))
            {
                _blur[key] = value;
            }
            else
            {
                _blur.Add(key, value);
            }
        }
        static void AddToCdnHost(string key, string value)
        {
            if (_cdnhost.ContainsKey(key))
            {
                _cdnhost[key] = value;
            }
            else
            {
                _cdnhost.Add(key, value);
            }
        }

        /// <summary>
        /// Synchronize PING remote host, and return the fastest IP address in the system and CDN table after success
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        private static PingReply? PingHost(string ip, string host)
        {
            if (string.IsNullOrEmpty(ip))
            {
                return null;
            }

            try
            {
                Ping pinghandle = new();
                PingReply reply = pinghandle.Send(ip, 1000);
                if (reply.Status != IPStatus.Success)
                {
                    return null;
                }

                if (reply.RoundtripTime <= 150)
                {
                    return reply;
                }

                host = host.Replace("*", "1");
                PingReply tempreply = pinghandle.Send(host);
                return tempreply != null && tempreply.Status == IPStatus.Success &&
                       tempreply.RoundtripTime < reply.RoundtripTime
                           ? null
                           : reply;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get the cdn address, if not, return the system resolution address
        /// </summary>
        /// <param name="host"></param>
        /// <param name="isCdn"></param>
        /// <returns></returns>
        public IPAddress GetCdnAddress(string host, out bool isCdn)
        {
            isCdn = true;
            if (_cdnhost.ContainsKey(host))
            {
                return IPAddress.Parse(_cdnhost[host]?.ToString()!);
            }

            string temphost = host.Substring(host.IndexOf(".") + 1);
            if (_blur.ContainsKey(temphost))
            {
                return IPAddress.Parse(_blur[temphost]?.ToString()!);
            }

            isCdn = false;
            return Dns.GetHostEntry(host).AddressList[0];
        }

        /// <summary>
        /// Export the final CDN list
        /// </summary>
        /// <returns></returns>
        public bool ExportHost()
        {
            using StreamWriter sw = new(@"Hosts\ExportCdnHosts.txt");
            sw.WriteLine("#PSX Download Helper\r\n\r\n");
            foreach (object? host in _blur.Keys)
            {
                sw.WriteLine(_blur[host] + " *." + host);
            }

            foreach (object? host in _cdnhost.Keys)
            {
                sw.WriteLine(_cdnhost[host] + " " + host);
            }

            return true;
        }
    }
}
