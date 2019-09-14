using System;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net;


namespace Z.Tools
{
    public class NetWorkTools
    {
        NetWorkTools() { }

        #region Port Scan

        /// <summary>
        /// 端口扫描
        /// </summary>
        /// <param name="IPPrefix">IP的c前缀 比如”192.168.0.“ </param>
        /// <param name="DStart">D段起始 比如 1.</param>
        /// <param name="DEnd">D段结尾 比如 255.</param>
        /// <param name="port">探测的端口.</param>
        /// <param name="ConnectEvent">探测结果的回掉 传入IPAndPort终端类对象和是否开启的布尔状态.</param>
        public static void IPv4ScanPort(string IPPrefix, int DStart, int DEnd, int port, Action<IPAndPort, bool> ConnectEvent)
        {
            // Configure CallBack Event
            ConnectedEvent = ConnectEvent;

            // Check
            if (!(IPv4Verify(IPPrefix + DStart) && IPv4Verify(IPPrefix + DEnd)))
                throw new Exception("Wrong Scan Parameters");

            if (DStart > DEnd)
            {
                int temp = DEnd;

                DEnd = DStart;

                DStart = temp;

            }

            // Init
            scanThreads = new List<Thread>();

            // Scan
            for (int i = DStart; i <= DEnd; i++)
            {
                string ip = IPPrefix + i;

                scanThreads.Add(new Thread(new ParameterizedThreadStart(ScanOne)));

                if (scanThreads.Count > 0)
                    scanThreads[scanThreads.Count - 1].Start(new IPAndPort(ip, port));

            }

            return;

        }

        /// <summary>
        /// 停止所有的端口扫描线程
        /// </summary>
        public static void StopPortScan()
        {
            if (scanThreads == null)
                return;
            if (scanThreads.Count == 0)
                return;

            foreach (Thread t in scanThreads)
            {
                if (t == null)
                    continue;
                t.Abort();
            }

            scanThreads.Clear();
        }

        static List<Thread> scanThreads;

        static Action<IPAndPort, bool> ConnectedEvent;

        static void ScanOne(object _para)
        {
            IPAndPort para = (IPAndPort)_para;

            if (para.port == 0 || para.ip == null || para.ip == string.Empty)
            {
                Logger.LogError("Wrong IP or Port"); return;
            }

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                Logger.Log(string.Format("try to connect {0}: ar {1}", para.ip, para.port));
                socket.Connect(para.ip, para.port);
            }
            catch (Exception e)
            {
                Logger.LogError(string.Format(" {0}: at {1} is close ,error msg :{2}", para.ip, para.port, e.Message));
            }
            finally
            {
                if (ConnectedEvent != null)
                {
                    if (socket.Connected)
                        ConnectedEvent(para, true);
                    else
                        ConnectedEvent(para, false);
                }

                socket.Close();
                socket = null;

            }
        }

        public static bool IPv4Verify(string IP)
        {
            return Regex.IsMatch(IP, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        #endregion

        #region 获取内网IP

        /// <summary>
        /// 获取内网IP
        /// </summary>
        /// <returns>The local ip.</returns>
        public static string[] GetIpv4()
        {
            string name = Dns.GetHostName();

            List<string> result = new List<string>();

            IPAddress[] iPs = Dns.GetHostAddresses(name);

            foreach (IPAddress ip in iPs)
            {
                result.Add(ip.ToString());
            }

            return result.ToArray();

        }

        public static string GetIpv6()
        {
            string ip = "";
            IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            for (int i = 0; i < addressList.Length; i++)
            {
                ip = addressList[i].ToString();
            }
            return ip;
        }

        #endregion

    }


    public class IPAndPort
    {
        public string ip;

        public int port;

        public IPAndPort(string _ip, int _port)
        {
            this.ip = _ip;
            this.port = _port;
        }
    }

}
