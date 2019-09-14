using System;
using System.Net.Sockets;

using Z.Tools;

namespace Z
{
    public class TcpConnectedPeer
    {
        #region Facade Zone

        public string token;

        /// <summary>
        /// 接收到消息时的回掉函数 返回值直接回复给客户端 返回null 则不回复
        /// </summary>
        public Action<string, byte[]> ResponseCallBack;

        /// <summary>
        /// 向该节点的客户端发送一条消息
        /// </summary>
        /// <param name="msg">消息.</param>
        public void SendDataToClient(byte[] msg)
        {
            try
            {
                this.socketHandler.BeginSend(
                    msg,
                    0,
                    msg.Length,
                    SocketFlags.None,
                    new AsyncCallback(PeerSendCallBack), this.socketHandler);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                socketHandler.Close();
            }
        }

        public TcpServer Server { get; private set; }

        public Socket socketHandler { get; private set; }

        public Action<Exception> OnDisconnected;

        public int BufferSize = 1024;

        byte[] buffer;

        public TcpConnectedPeer(
            string token,
            Socket socket,
            Action<string, byte[]> OnReceived,
            TcpServer fromServer)
        {
            this.token = token;
            this.socketHandler = socket;
            this.ResponseCallBack = OnReceived;

            buffer = new byte[BufferSize];

            this.socketHandler.BeginReceive(
                buffer,
                0,
                BufferSize,
                SocketFlags.None,
                new AsyncCallback(PeerReceiveCallBack),
                this.socketHandler
            );

        }

        void PeerReceiveCallBack(IAsyncResult ar)
        {
            Socket _clientHander = (Socket)ar.AsyncState;

            int byteLength = 0;

            byte[] receivedData;

            byteLength = _clientHander.EndReceive(ar);

            receivedData = buffer;

            buffer = null;

            buffer = new byte[BufferSize];

            if (byteLength > 0)
            {
                _clientHander.BeginReceive(
                buffer,
                0,
                BufferSize,
                SocketFlags.None,
                new AsyncCallback(PeerReceiveCallBack),
                _clientHander
                );
            }
            else
            {
                socketHandler.Close();

                if (OnDisconnected != null)
                    OnDisconnected(null);

                return;
            }

            try
            {
                if (ResponseCallBack != null)
                    ThreadBridge.Invoke(() => { ResponseCallBack(token, receivedData); });

            }
            catch (Exception e)
            {
                Logger.LogError(e.Message + ":::" + e.StackTrace);
            }

        }

        void PeerSendCallBack(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                int SendBytesLength = handler.EndSend(ar);
            }
            catch (Exception e)
            {
                this.socketHandler.Shutdown(SocketShutdown.Both);
                this.socketHandler.Close();
                if (OnDisconnected != null)
                    OnDisconnected(e);
                
                return;
            }
        }

        #endregion

    }
}
