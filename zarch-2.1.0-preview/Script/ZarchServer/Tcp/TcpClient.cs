using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using  Z.Tools;

namespace  Z
{
    public class TcpClient
    {
        #region Elements

        protected Socket dasSocket;

        protected byte[] buffer = new byte[1024];       //for receive

        protected int BufferSize = 1024;

        protected byte[] sendDataBuffer;                //for send

        protected bool isSpitePackage ;

        protected int restPackage = -1;

        protected int bufferIndex ;

        protected int maxSinglePackageSize = 1024;

        public Action<byte[]> OnReceived;

        #endregion

        public TcpClient(string _ServerIpAddr, int _ServerPort, Action<byte[]> OnReceived,  IPType ipType = IPType.IPv4)
        {
            if(ipType ==  IPType.IPv6)
                dasSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            else
                dasSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress ipa = IPAddress.Parse(_ServerIpAddr);

            IPEndPoint ipe = new IPEndPoint(ipa, _ServerPort);

            this.OnReceived = OnReceived;

            dasSocket.BeginConnect(ipe, new AsyncCallback(ConnectAsynCallBack), dasSocket);

        }

        #region CallBack Stuff

        void ConnectAsynCallBack(IAsyncResult ar)
        {
            Socket socketHandler = (Socket)ar.AsyncState;

            try
            {
                socketHandler.EndConnect(ar);
                socketHandler.BeginReceive(
                    buffer,
                    0,
                    BufferSize,
                    SocketFlags.None,
                    new AsyncCallback(ReceivedAsynCallBack),
                    socketHandler
                    );
            }
            catch (Exception e)
            {
                 Logger.LogError("[TcpClient] Remote computer reject this request" + e.Message + "\n");
            }
        }

        void ReceivedAsynCallBack(IAsyncResult ar)
        {
            Socket socketHandler = (Socket)ar.AsyncState;

            int byteLength = socketHandler.EndReceive(ar);

            if (byteLength > 0)
            {
                if (OnReceived != null)
                    ThreadBridge.Invoke(() => { OnReceived(buffer); });

            }

            socketHandler.BeginReceive(
                buffer,
                0,
                BufferSize,
                SocketFlags.None,
                new AsyncCallback(ReceivedAsynCallBack),
                socketHandler
                );
        }

        void SendAsynCallBack(IAsyncResult ar)
        {
            try
            {
                Socket socketHandler = (Socket)ar.AsyncState;
                socketHandler.EndSend(ar);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }
        }

        #endregion

        #region Facade Zone

        public void SendDataToServer(byte[] msg)
        {
            if (!dasSocket.Connected)
            {
                Logger.LogError("TcpClient has not Connected");
                return;
            }
            
                dasSocket.BeginSend(
                    msg,
                    0,
                    msg.Length,
                    0,
                    new AsyncCallback(SendAsynCallBack),
                    dasSocket
                    );
        }

        #endregion

    }
}
