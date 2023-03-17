using System;
using System.Net;
using System.Net.Sockets;

namespace PSXDLL
{
    public abstract class Client : IDisposable
    {
        private readonly DestroyDelegate? _destroyer;
        private readonly byte[] _buffer;
        private readonly byte[] _remoteBuffer;
        private Socket? _clientSocket;
        private Socket? _destinationSocket;

        protected Client()
        {
            _buffer = new byte[256 * 1024]; //0x1000
            _remoteBuffer = new byte[128 * 1024]; //0x400
            ClientSocket = null;
            _destroyer = null;
        }

        protected Client(Socket clientSocket, DestroyDelegate destroyer)
        {
            _buffer = new byte[256 * 1024]; //0x1000
            _remoteBuffer = new byte[128 * 1024]; //0x400
            ClientSocket = clientSocket;
            _destroyer = destroyer;
        }

        public byte[] Buffer => _buffer;

        public Socket? ClientSocket
        {
            get => _clientSocket;
            set
            {
                if (_clientSocket != null)
                {
                    _clientSocket.Close();
                }
                _clientSocket = value;
            }
        }

        public Socket? DestinationSocket
        {
            get => _destinationSocket;
            set
            {
                if (_destinationSocket != null)
                {
                    _destinationSocket.Close();
                }
                _destinationSocket = value;
            }
        }

        public byte[] RemoteBuffer => _remoteBuffer;

        public void Dispose()
        {
            try
            {
                if (ClientSocket != null)
                {
                    ClientSocket.Shutdown(SocketShutdown.Both);
                }
            }
            catch
            {
            }
            try
            {
                if (DestinationSocket != null)
                {
                    DestinationSocket.Shutdown(SocketShutdown.Both);
                }
            }
            catch
            {
            }
            if (ClientSocket != null)
            {
                ClientSocket.Close();
            }
            if (DestinationSocket != null)
            {
                DestinationSocket.Close();
            }
            ClientSocket = null;
            DestinationSocket = null;
            _destroyer?.Invoke(this);
        }

        public void OnClientReceive(IAsyncResult ar)
        {
            try
            {
                if (ClientSocket == null)
                {
                    return;
                }

                int size = ClientSocket.EndReceive(ar);
                if (size > 0 && DestinationSocket != null)
                {
                    DestinationSocket.BeginSend(Buffer, 0, size, SocketFlags.None, OnRemoteSent, DestinationSocket);
                }
            }
            catch
            {
                Dispose();
            }
        }

        public void OnClientSent(IAsyncResult ar)
        {
            try
            {
                if (ClientSocket != null && ClientSocket.EndSend(ar) > 0)
                {
                    DestinationSocket?.BeginReceive(RemoteBuffer, 0, RemoteBuffer.Length, SocketFlags.None,
                                                   OnRemoteReceive, DestinationSocket);
                }
            }
            catch
            {
                Dispose();
            }
        }

        public void OnRemoteReceive(IAsyncResult ar)
        {
            try
            {
                if (DestinationSocket != null)
                {
                    int size = DestinationSocket.EndReceive(ar);
                    if (size > 0 && ClientSocket != null)
                    {
                        ClientSocket.BeginSend(RemoteBuffer, 0, size, SocketFlags.None, OnClientSent, ClientSocket);
                    }
                }
            }
            catch
            {
                Dispose();
            }
        }

        public void OnRemoteSent(IAsyncResult ar)
        {
            try
            {
                if (DestinationSocket?.EndSend(ar) > 0 && ClientSocket != null)
                {
                    ClientSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, OnClientReceive, ClientSocket);
                }
            }
            catch { Dispose(); }
        }

        public abstract void StartHandshake();

        public void StartRelay()
        {
            try
            {
                if (ClientSocket != null)
                {
                    ClientSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, OnClientReceive, ClientSocket);
                    DestinationSocket?.BeginReceive(RemoteBuffer, 0, RemoteBuffer.Length, SocketFlags.None, OnRemoteReceive,
                                                   DestinationSocket);
                }
            }
            catch
            {
                Dispose();
            }
        }

        public override string ToString()
        {
            try
            {
                return ($"connecting： {((IPEndPoint)DestinationSocket!.RemoteEndPoint!).Address}");
            }
            catch
            {
                return "Connection established successfully";
            }
        }
    }
}
