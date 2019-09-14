using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Z
{
    public class WebClient
    {
        public static string RequestPostData
        (
            Dictionary<string, string> data,
            string url,
            string content_type = "application/x-www-form-urlencoded",
            bool cryptVarName = false,
            WebHeaderCollection headers = null,
            CookieContainer cookies = null
        )
        {
            #region Translate and EnPackage Data

            string postData = "";

            if (data != null)
            {

                StringBuilder sb = new StringBuilder();

                int i = 0;

                foreach (var kvItem in data)
                {
                    sb.AppendFormat(
                        (i > 0) ? "&{0}={1}" : "{0}={1}",
                        ((cryptVarName) ? (kvItem.Key) : kvItem.Key),
                        (kvItem.Value)
                    );
                    i++;
                }

                postData = sb.ToString();
            }

            //Logger.Log("REQUEST HTTP POST :\n" + url + ":" + postData + "\n\n");

            #endregion

            #region Create and Post Request

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "POST";

            request.ContentType = content_type;

            request.ContentLength = Zarch.Network.CurrentEncoding.GetBytes(postData).Length;

            if (headers != null)
                request.Headers = headers;

            if (cookies != null)
                request.CookieContainer = cookies;

            Stream requestStream = request.GetRequestStream();

            requestStream.Write(Zarch.Network.CurrentEncoding.GetBytes(postData), 0, Zarch.Network.CurrentEncoding.GetBytes(postData).Length);

            requestStream.Close();

            #endregion

            #region Get Response

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();

            StreamReader responseReader = new StreamReader(responseStream, Zarch.Network.CurrentEncoding);

            string htmlResult = (responseReader.ReadToEnd());

            responseReader.Close();

            responseStream.Close();

            #endregion

            return (htmlResult);
        }


        public static string RequestGetData
        (
            Dictionary<string, string> data,
            string url,
            string content_type = "text/html;charset=UTF-8",
            bool cryptVarName = false,
            WebHeaderCollection headers = null,
            CookieContainer cookies = null
        )
        {
            #region Translate and Enpackage Data

            string getData = "";

            if (data != null)
            {

                StringBuilder sb = new StringBuilder();

                int i = 0;

                foreach (var kvItem in data)
                {
                    sb.AppendFormat(
                        (i > 0) ? "&{0}={1}" : "{0}={1}",
                        ((cryptVarName) ? (kvItem.Key) : kvItem.Key),
                        (kvItem.Value)
                    );
                    i++;
                }

                getData = sb.ToString();

                sb.Clear();

                string strRegex = @"(" + "&" + ")" + "$";

                getData = Regex.Replace(getData, strRegex, "");
            }
            else
            {
                getData = "";
            }

            #endregion

            #region Create and Send Request

            //Logger.Log("REQUEST HTTP GET :\n" + url + "?" + getData + "\n\n");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + (getData == "" ? "" : "?") + getData);

            request.Method = "GET";

            request.ContentType = content_type;

            if (headers != null)
                request.Headers = headers;

            if (cookies != null)
                request.CookieContainer = cookies;

            #endregion

            #region Get Response

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();

            StreamReader responseReader = new StreamReader(responseStream, Zarch.Network.CurrentEncoding);

            string htmlResult = responseReader.ReadToEnd();

            responseReader.Close();

            responseStream.Close();

            #endregion

            return ((htmlResult));

        }


        private WebClient() { }

    }
}
