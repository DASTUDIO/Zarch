using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Z.Tools;
using System.Text;

namespace Z
{
    public class WebServer
    {
        #region Facade Methods

        public void runServer()
        {
            try
            {
                this.httpListener = new HttpListener();

                try
                {
                    httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;

                    foreach (string urlPrefixItem in this.urlPrefixSet)
                    {
                        if (urlPrefixItem != null && urlPrefixItem != "")
                            httpListener.Prefixes.Add("http://" + urlPrefixItem + "/");
                    }
                }
                catch (Exception e)
                {
                    Logger.LogWarnning(e.Message);
                }

                httpListener.Start();

                httpListener.BeginGetContext(new AsyncCallback(ResponseCallBack), httpListener);

            }
            catch (Exception e)
            {
                Logger.LogError("WebServer Start Failed " + e.Message);

                if (OnDisconnect != null)
                    OnDisconnect(e);
            }
        }

        public void stopServer()
        {
            this.httpListener.Stop();
            this.httpListener = null;
        }

        #region Callback Delegates

        public Action Setup;

        public Action<HttpListenerContext> OnConnect;

        public Action<Exception> OnDisconnect;

        /// <summary>
        /// 参数从左到右 依次为 RawSubUrl ，Post数据 ， 返回值类型string
        /// </summary>
        public Func<string, string, string> OnResponse;

        #endregion

        #endregion

        #region Elements

        #region Members

        public DResponseType ResponseType = DResponseType.String;

        public string E404Message = "Error 404";

        public HttpListener httpListener { get; private set; }

        #endregion

        List<string> urlPrefixSet = new List<string>();

        #endregion

        #region Constructors

        public WebServer(Func<string, string, string> responseCallback, int port = 8080, string ip = null)
        {
            if (this.Setup != null)
                this.Setup();

            this.OnResponse = responseCallback;

            try
            {
                this.urlPrefixSet.Add("localhost:" + port);

                this.urlPrefixSet.Add("127.0.0.1:" + port);

                string[] localIPs = NetWorkTools.GetIpv4();

                foreach (var item in localIPs)
                {
                    this.urlPrefixSet.Add(item + ":" + port);
                }

                if (ip != null && ip != string.Empty && ip != "")
                    //if (DNetWorkTools.Varify(ip))
                    this.urlPrefixSet.Add(ip + ":" + port);
                //else
                //DLog.LogError("Wrong IP");



            }
            catch (Exception e)
            {
                Logger.LogError("Initial Error:" + e.Message);
            }

        }

        #endregion

        #region Internal Facade

        string getGetParameter(HttpListenerContext _contextHandler, string ParameterName)
        {
            return ((_contextHandler.Request.QueryString[(ParameterName)]));
        }

        string getPostData(HttpListenerContext _contextHandler)
        {
            HttpListenerRequest request = _contextHandler.Request;

            try
            {
                Stream s = request.InputStream;

                int count = 0;

                byte[] buffer = new byte[1024];

                StringBuilder builder = new StringBuilder();

                while ((count = s.Read(buffer, 0, 1024)) > 0)
                {
                    builder.Append(Zarch.Network.CurrentEncoding.GetString(buffer, 0, count));
                }
                s.Flush();

                s.Close();

                s.Dispose();

                return builder.ToString();

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return null;
            }
        }

        public void SendStrings(HttpListenerContext _contextHandler, string strMsg, bool notEnd = false)
        {
            string CryptStrData = (strMsg);

            using (StreamWriter writer = new StreamWriter(_contextHandler.Response.OutputStream, Zarch.Network.CurrentEncoding))
            {
                writer.Write(CryptStrData);

                writer.Close();

                if (!notEnd)
                    _contextHandler.Response.Close();
            }
        }

        void SendHTMLContent(HttpListenerContext _contextHandler, string httpContent, bool notEnd = false)
        {
            _contextHandler.Response.ContentType = "text/html";

            using (StreamWriter writer = new StreamWriter(_contextHandler.Response.OutputStream, Zarch.Network.CurrentEncoding))
            {
                writer.Write(httpContent);

                writer.Close();

                if (!notEnd)
                    _contextHandler.Response.Close();

            }
        }

        void SendCustomTypeContent(HttpListenerContext _contextHandler, string customContent, string contentType, bool notEnd = false)
        {
            _contextHandler.Response.ContentType = contentType;

            using (StreamWriter writer = new StreamWriter(_contextHandler.Response.OutputStream, Zarch.Network.CurrentEncoding))
            {
                writer.Write(customContent);

                writer.Close();

                if (!notEnd)
                    _contextHandler.Response.Close();

            }
        }

        void show404(HttpListenerContext _contextHandler, string e404msg)
        {
            _contextHandler.Response.ContentType = "text/html";

            StreamWriter sw = new StreamWriter(_contextHandler.Response.OutputStream, Zarch.Network.CurrentEncoding);

            sw.Write(e404msg);

            sw.Close();

            _contextHandler.Response.Close();
        }

        #endregion

        #region HandlerMethod

        protected void ResponseCallBack(IAsyncResult ar)
        {
            HttpListener _httplistener = (HttpListener)ar.AsyncState;

            HttpListenerContext _httpListenerContext = _httplistener.EndGetContext(ar);

            _httplistener.BeginGetContext(new AsyncCallback(ResponseCallBack), _httplistener);

            if(OnConnect != null)
                OnConnect(_httpListenerContext); 

            //Logger.Log(string.Format(
            //    "Http Request Received \n at : {0} \n from {1} \n with header : {2} \n",
            //    _httpListenerContext.Request.Url,
            //    _httpListenerContext.Request.UserHostAddress,
            //    _httpListenerContext.Request.Headers,
            //    _httpListenerContext.Request.Cookies));

            _httpListenerContext.Response.StatusCode = 200;

            // prepare
            string validPath = ((_httpListenerContext.Request.RawUrl));

            string postData = getPostData(_httpListenerContext);


            if (this.OnResponse != null)
            {
                switch (this.ResponseType)
                {
                    case DResponseType.String:
                        {
                            SendStrings(_httpListenerContext, OnResponse(validPath, postData));
                            break;
                        }
                    case DResponseType.HTML:
                        {
                            SendHTMLContent(_httpListenerContext, OnResponse(validPath, postData));
                            break;
                        }
                    case DResponseType.E404:
                        {
                            show404(_httpListenerContext, this.E404Message);
                            break;
                        }
                }
            }
        }
        #endregion
    }

    public enum DResponseType : Byte
    {
        String,
        HTML,
        E404
    }

}