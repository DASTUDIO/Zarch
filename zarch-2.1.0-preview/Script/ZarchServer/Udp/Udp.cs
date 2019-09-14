using System;
using System.Net;
using System.Net.Sockets;
using Z.Tools;

namespace Z
{
    public class Udp
    {
        #region Elements

        public Socket socketHandler { get; private set; }

        public Action<EndPoint, byte[]> ResponseCallback;


        IPEndPoint InitEndPoint;

        IPType IPType;

        int bufferSize;

        int port;

        #endregion

        public Udp(int Port = 8085, IPType IpType = IPType.IPv4, Action<EndPoint, byte[]> ResponseCallBack = null, int bufferSize = 1024)
        {
            ResponseCallback = ResponseCallBack;

            IPType = IpType;

            port = Port;

            this.bufferSize = bufferSize;

            init();
        }

        public void runServer()
        {
            if (socketHandler == null)
            {
                Logger.LogError("IP Type Error at UDPServer Init");
                return;
            }

            UdpPeer peerInit = new UdpPeer(socketHandler, bufferSize, IPType);

            socketHandler.BeginReceiveFrom
                         (peerInit.buffer, 0, bufferSize, SocketFlags.None,
                          ref peerInit.remoteEndPoint, new AsyncCallback(BeginResponseCallBack), peerInit);
        }

        public void StopServer()
        {
            socketHandler.Close();
            socketHandler = null;
        }

        public void SendTo(EndPoint endPoint, byte[] msg)
        {
            if (socketHandler == null)
                init();

            this.socketHandler.BeginSendTo
                (msg, 0, msg.Length, SocketFlags.None, endPoint, new AsyncCallback(BeginSendToCallBack), null);

        }

        #region Interior Stuff

        void BeginResponseCallBack(IAsyncResult ar)
        {
            UdpPeer peer = (UdpPeer)ar.AsyncState;

            int rev = peer.serverSocket.EndReceiveFrom(ar, ref peer.remoteEndPoint);

            if (rev > 0)
            {
                if (ResponseCallback != null)
                    ThreadBridge.Invoke(() => { ResponseCallback(peer.remoteEndPoint, peer.buffer); });

                    peer.ResetBuffer();

                socketHandler.BeginReceiveFrom
                             (peer.buffer, 0, bufferSize, SocketFlags.None,
                              ref peer.remoteEndPoint, new AsyncCallback(BeginResponseCallBack), peer);
            }
        }

        void BeginSendToCallBack(IAsyncResult ar)
        {
            socketHandler.EndSendTo(ar);
        }

        void init()
        {
            if (IPType == IPType.IPv4)
            {
                socketHandler = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                InitEndPoint = new IPEndPoint(IPAddress.Any, port);
            }
            else if (IPType == IPType.IPv6)
            {
                socketHandler = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
                InitEndPoint = new IPEndPoint(IPAddress.IPv6Any, port);
            }
            socketHandler.Bind(InitEndPoint);
        }

        #endregion

    }

    public class UdpPeer
    {
        public EndPoint remoteEndPoint;
        public byte[] buffer;
        public Socket serverSocket;

        int bufferSize;

        IPType iPType = IPType.IPv4;

        public UdpPeer(Socket _serverSocket, int bufferSize, IPType iPType)
        {
            serverSocket = _serverSocket;

            this.iPType = iPType;

            if (this.iPType == IPType.IPv4)
                remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            else
                remoteEndPoint = new IPEndPoint(IPAddress.IPv6Any, 0);

            buffer = new byte[bufferSize];

            this.bufferSize = bufferSize;

            this.iPType = iPType;
        }

        public void ResetBuffer()
        {
            buffer = new byte[bufferSize];

            if (iPType == IPType.IPv4)
                remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            else
                remoteEndPoint = new IPEndPoint(IPAddress.IPv6Any, 0);
        }
    }
}
