﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PSXDLL
{
    public sealed class HttpClient : Client
    {
        private readonly UpdataUrlLog _updataUrlLog;
        private string? _mHttpPost;
        private LocalFile? _mLocalFile;
        private UrlInfo _uinfo = new();

        public HttpClient(Socket clientSocket, DestroyDelegate destroyer, UpdataUrlLog updataUrlLog)
            : base(clientSocket, destroyer)
        {
            _updataUrlLog = updataUrlLog;
        }

        private Dictionary<string, string>? HeaderFields { get; set; }

        private string? HttpQuery { get; set; }

        private string? HttpRequestType { get; set; }

        private string? HttpVersion { get; set; }

        public string? RequestedPath { get; set; }

        public string? RequestedUrl { get; set; }

        private void QueryHandle(string query)
        {
            HeaderFields = ParseQuery(query);
            if ((HeaderFields == null) || !HeaderFields.ContainsKey("Host"))
            {
                SendBadRequest();
            }
            else
            {
                int num;
                string requestedPath;
                int index;
                if (HttpRequestType!.ToUpper().Equals("CONNECT"))
                {
                    index = RequestedPath!.IndexOf(":");
                    if (index >= 0)
                    {
                        requestedPath = RequestedPath.Substring(0, index);
                        num = RequestedPath.Length > (index + 1) ? int.Parse(RequestedPath.Substring(index + 1)) : 443;
                    }
                    else
                    {
                        requestedPath = RequestedPath;
                        num = 80;
                    }
                }
                else
                {
                    index = HeaderFields["Host"].IndexOf(":");
                    if (index > 0)
                    {
                        requestedPath = HeaderFields["Host"].Substring(0, index);
                        num = int.Parse(HeaderFields["Host"].Substring(index + 1));
                    }
                    else
                    {
                        requestedPath = HeaderFields["Host"];
                        num = 80;
                    }
                    if (HttpRequestType.ToUpper().Equals("POST"))
                    {
                        int tempnum = query.IndexOf("\r\n\r\n");
                        _mHttpPost = query[(tempnum + 4)..];
                    }
                }

                string localFile = string.Empty;
                if (MonitorLog.RegexUrl(RequestedUrl!))
                {
                    localFile = UrlOperate.MatchFile(RequestedUrl!);
                }

                _uinfo.PsnUrl = string.IsNullOrEmpty(_uinfo.PsnUrl) ? RequestedUrl : _uinfo.PsnUrl;//psnurl assignment
                if (!HttpRequestType.ToUpper().Equals("CONNECT") && localFile != string.Empty && File.Exists(localFile))
                {
                    _uinfo.ReplacePath = localFile;
                    _updataUrlLog(_uinfo);
                    SendLocalFile(localFile, HeaderFields.ContainsKey("Range") ? HeaderFields["Range"] : null, HeaderFields.ContainsKey("Proxy-Connection") ? HeaderFields["Proxy-Connection"] : null);
                }
                else
                {
                    try
                    {
                        bool iscdn;
                        IPAddress hostIp = CdnOperate.GetCdnAddress(requestedPath, out iscdn);
                        IPEndPoint remoteEp = new(hostIp, num);
                        DestinationSocket = new Socket(remoteEp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        if (HeaderFields.ContainsKey("Proxy-Connection") &&
                            HeaderFields["Proxy-Connection"].ToLower().Equals("keep-alive"))
                        {
                            DestinationSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, 1);
                        }
                        DestinationSocket.BeginConnect(remoteEp, OnConnected, DestinationSocket);

                        //add to main panel
                        _uinfo.Host = hostIp.ToString();
                        _uinfo.IsCdn = iscdn;
                        _uinfo.ReplacePath = string.Empty;
                        _updataUrlLog(_uinfo);
                    }
                    catch
                    {
                        SendBadRequest();
                    }
                }
            }
        }

        private Dictionary<string, string> ParseQuery(string query)
        {
            int index;
            Dictionary<string, string> dictionary = new();
            string[] strArray = query.Replace("\r\n", "\n").Split(new[] { '\n' });
            if (strArray.Length > 0)
            {
                index = strArray[0].IndexOf(' ');
                if (index > 0)
                {
                    HttpRequestType = strArray[0][..index];
                    strArray[0] = strArray[0][index..].Trim();
                }
                index = strArray[0].LastIndexOf(' ');
                if (index > 0)
                {
                    HttpVersion = strArray[0][index..].Trim();
                    RequestedUrl = strArray[0][..index];
                }
                else
                {
                    RequestedUrl = strArray[0];
                }
                if (!string.IsNullOrEmpty(RequestedUrl) && RequestedUrl.ToLower().StartsWith("http"))
                {
                    if (RequestedUrl.ToLower().StartsWith("http://"))
                    {
                        index = RequestedUrl.IndexOf('/', 7);
                    }
                    else if (RequestedUrl.ToLower().StartsWith("https://"))
                    {
                        index = RequestedUrl.IndexOf('/', 8);
                    }

                    RequestedPath = index == -1 ? "/" : RequestedUrl.Substring(index);
                }
                else
                {
                    RequestedPath = RequestedUrl;
                }
            }
            for (int i = 1; i < strArray.Length; i++)
            {
                index = strArray[i].IndexOf(":");
                if ((index <= 0) || (index >= (strArray[i].Length - 1)))
                {
                    continue;
                }

                try
                {
                    dictionary.Add(strArray[i].Substring(0, index), strArray[i].Substring(index + 1).Trim());
                }
                catch
                {
                }
            }
            return dictionary;
        }

        private string RebuildQuery()
        {
            string str = $"{HttpRequestType} {RequestedPath} {HttpVersion}\r\n";
            if (HeaderFields != null)
            {
                str = HeaderFields.Keys.Aggregate(str, (current, s) => $"{current}{s.Replace("Proxy-", "")}: {HeaderFields[s]}\r\n");
                str += "\r\n";
                if (_mHttpPost != null)
                {
                    str += _mHttpPost;
                }
            }
            return str;
        }

        private async void SendLocalFile(string? localFile, string? requestRange, string? connection)
        {
            _mLocalFile = new LocalFile(localFile!);
            string responseStr = BuildResponse(requestRange!, connection!, out long startRange);
            _mLocalFile.FileStream!.Seek(startRange, SeekOrigin.Begin);
            try
            {
                await Task.Run(() => ClientSocket?.BeginSend(Encoding.ASCII.GetBytes(responseStr), 0, responseStr.Length, SocketFlags.None, OnLocalFileSent, ClientSocket));
            }
            catch
            {
                _mLocalFile.FileStream.Close();
                Dispose();
            }
        }

        /// <summary>
        /// local echo reply request
        /// </summary>
        /// <param name="requestRange"></param>
        /// <param name="connection"></param>
        /// <param name="startRange"></param>
        /// <returns></returns>

        private string BuildResponse(string requestRange, string connection, out long startRange)
        {
            string status = "200 OK";
            startRange = 0;
            long endRange = _mLocalFile!.Filesize - 1;

            StringBuilder response = new("HTTP/1.1 {0}\r\n");
            response.Append("Server: Apache\r\n");
            response.Append("Accept-Ranges: bytes\r\n");
            response.Append("Cache-Control: max-age=3600\r\n");
            response.Append("Content-Type: application/octet-stream\r\n");
            if (!string.IsNullOrEmpty(requestRange) && AppConfig.Instance().ConnType != 1)
            {
                status = "206 Partial Content";
                string rangeStr = requestRange.Split('=')[1].Trim();
                List<string> temp = rangeStr.Split('-').Select(r => r.Trim()).ToList();
                startRange = long.Parse(temp[0]);
                if (!string.IsNullOrEmpty(temp[1]))
                {
                    endRange = long.Parse(temp[1]);
                }
                else
                {
                    rangeStr += (_mLocalFile.Filesize - 1);
                }

                rangeStr += "/" + _mLocalFile.Filesize;

                response.Append(string.Format("Content-Range: bytes {0}\r\n", rangeStr));
            }
            response.Append("Date: {1}\r\n");
            response.Append("Last-Modified: {2}\r\n");
            response.Append("Content-Length: {3}\r\n");
            if (string.IsNullOrEmpty(connection))
            {
                connection = "close";
            }

            response.Append(string.Format("Connection: {0}\r\n\r\n", connection));

            string responseStr = string.Format(response.ToString(), status, DateTime.Now.ToUniversalTime().ToString("r"), _mLocalFile.LastModified.ToUniversalTime().ToString("r"), endRange + 1 - startRange);
            return responseStr;
        }

        private async void SendBadRequest()
        {
            const string s =
                "HTTP/1.1 400 Bad Request\r\nConnection: close\r\nContent-Type: text/html\r\n\r\nBad Request";
            try
            {
                if (ClientSocket != null)
                {
                  await  Task.Run(() => ClientSocket.BeginSend(Encoding.ASCII.GetBytes(s), 0, s.Length, SocketFlags.None, OnErrorSent, ClientSocket));
                }
            }
            catch
            {
                Dispose();
            }
        }

        private bool IsValidQuery(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return false;
            }

            HeaderFields = ParseQuery(query);
            return !HttpRequestType!.ToUpper().Equals("POST") || !HeaderFields.ContainsKey("Content-Length");
        }

        private void OnQuerySent(IAsyncResult ar)
        {
            try
            {
                if (DestinationSocket != null && DestinationSocket.EndSend(ar) == -1)
                {
                    Dispose();
                }
                else
                {
                    StartRelay();
                }
            }
            catch
            {
                Dispose();
            }
        }

        private void OnReceiveQuery(IAsyncResult ar)
        {
            int num = -1;
            try
            {
                if (ClientSocket != null)
                {
                    num = ClientSocket.EndReceive(ar);
                }
            }
            catch
            {
                num = -1;
            }
            if (num <= 0)
            {
                Dispose();
            }
            else
            {
                HttpQuery += Encoding.ASCII.GetString(Buffer, 0, num);
                if (IsValidQuery(HttpQuery))
                {
                    QueryHandle(HttpQuery);
                }
                else
                {
                    try
                    {
                        if (ClientSocket != null)
                        {
                            ClientSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, OnReceiveQuery, ClientSocket);
                        }
                    }
                    catch
                    {
                        Dispose();
                    }
                }
            }
        }

        private async void OnConnected(IAsyncResult ar)
        {
            try
            {
                if (DestinationSocket == null)
                {
                    return;
                }

                string str;
                DestinationSocket.EndConnect(ar);
                if (HttpRequestType!.ToUpper().Equals("CONNECT"))
                {
                    if (ClientSocket != null)
                    {
                        str = $"{HttpVersion} 200 Connection established\r\n\r\n";
                       await Task.Run(()=> ClientSocket.BeginSend(Encoding.ASCII.GetBytes(str), 0, str.Length, SocketFlags.None, OnOkSent, ClientSocket));
                    }
                }
                else
                {
                    str = RebuildQuery();
                    await Task.Run(()=> DestinationSocket.BeginSend(Encoding.ASCII.GetBytes(str), 0, str.Length, SocketFlags.None, OnQuerySent, DestinationSocket));
                }
            }
            catch
            {
                Dispose();
            }
        }

        private void OnOkSent(IAsyncResult ar)
        {
            try
            {
                if (ClientSocket != null && ClientSocket.EndSend(ar) == -1)
                {
                    Dispose();
                }
                else
                {
                    StartRelay();
                }
            }
            catch
            {
                Dispose();
            }
        }

        private void OnErrorSent(IAsyncResult ar)
        {
            try
            {
                if (ClientSocket != null)
                {
                    ClientSocket.EndSend(ar);
                }
            }
            catch
            {
                Dispose();
            }
        }

        private async void OnLocalFileSent(IAsyncResult ar)
        {
            try
            {
                if (_mLocalFile!.FileStream!.Position < _mLocalFile.FileStream.Length)
                {
                    byte[] buffer = new byte[1024 * 4];
                    _mLocalFile.FileStream.Read(buffer, 0, buffer.Length);
                   await Task.Run(() => ClientSocket?.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, OnLocalFileSent, ClientSocket));
                }
                else
                {
                    ClientSocket?.EndSend(ar);
                    _mLocalFile.FileStream.Close();
                }
            }
            catch
            {
                _mLocalFile!.FileStream!.Close();
                Dispose();
            }
        }

        public override void StartHandshake()
        {
            try
            {
                if (ClientSocket != null)
                {
                    ClientSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, OnReceiveQuery, ClientSocket);
                }
            }
            catch
            {
                Dispose();
            }
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool withUrl)
        {
            string str;
            try
            {
                if ((DestinationSocket == null) || (DestinationSocket.RemoteEndPoint == null))
                {
                    str = $"Incoming HTTP connection from {((IPEndPoint)ClientSocket?.RemoteEndPoint!).Address}";
                }
                else
                {
                    str = $"HTTP connection from {((IPEndPoint)ClientSocket?.RemoteEndPoint!).Address} to {((IPEndPoint)DestinationSocket.RemoteEndPoint).Address} on port {((IPEndPoint)DestinationSocket.RemoteEndPoint).Port.ToString(CultureInfo.InvariantCulture)}";
                }
                if ((HeaderFields != null) && HeaderFields.ContainsKey("Host") && (RequestedPath != null))
                {
                    str += $"\r\n requested URL: http://{HeaderFields["Host"]}{RequestedPath}";
                }
            }
            catch
            {
                str = "HTTP Connection";
            }
            return str;
        }
    }
}
