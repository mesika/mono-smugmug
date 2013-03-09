using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Web;
using System.Threading.Tasks;

namespace SmugMugModel
{
    public class CommunicationHelper
    {
        private string baseAddrSecure = "https://secure.smugmug.com/services/api/json/1.2.2/";
        private string baseAddrNormal = "http://api.smugmug.com/services/api/json/1.2.2/";
        private string apiKey = "bOZadEKXwVbwgekCd9WnLjSLuwQ98uo0";

        /// <summary>
        /// Will do a call to the SM JSON endpoint and will return the response
        /// </summary>
        /// <param name="method">The method</param>
        /// <param name="args">The arguments</param>
        /// <returns></returns>
        public async Task<SMR> ExecuteMethod<SMR>(string method, SmugMugBase sessionState, params object[] args) where SMR : SmugMugResponse
        {
            return (await ExecuteMethodReturnCookie<SMR>(method, sessionState, args)).Item1;
        }

        /// <summary>
        /// Calls to the SM JSON endpoint and returns a tuple that contains the response from SM and the login cookie
        /// </summary>
        /// <typeparam name="SMR"></typeparam>
        /// <param name="method"></param>
        /// <param name="sessionState"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<Tuple<SMR, string>> ExecuteMethodReturnCookie<SMR>(string method, SmugMugBase sessionState, params object[] args) where SMR : SmugMugResponse
        {
            //most calls go to the normal api
            string baseAddr = baseAddrNormal;
            string su = string.Empty; //used to keep the su cookie

            //for login, go to secure 
            if (method.ToLower().Contains("login"))
                baseAddr = baseAddrSecure;

            //if we don't have a method or the parameters are not in pairs of 2, bail
            if (string.IsNullOrEmpty(method))
                throw new ArgumentException("The method cannot be null or empty", "method");

            if (args.Length % 2 != 0)
                throw new ArgumentException("The number of arguments must be even", "args");

            //we need to build the command.
            StringBuilder sb = new StringBuilder();
            //Pretty - Return a more human friendly response (default: false)
            sb.AppendFormat("method={0}&APIKey={1}&Pretty=true", method, apiKey);

            if (sessionState != null)
                sb.Append("&" + sessionState.ToString());

            for (int i = 0; i < args.Length; i += 2)
            {
                sb.AppendFormat("&{0}={1}", args[i].ToString(), HttpUtility.UrlEncode(args[i + 1].ToString()));
            }

            //we need to send that request as a stream of bytes.
            var message = System.Text.UTF8Encoding.UTF8.GetBytes(sb.ToString());

            var myWebRequest = HttpWebRequest.Create(baseAddr);
            ((HttpWebRequest)myWebRequest).UserAgent = "SmugMugModel_v1.0";

            //do we have a proxy?
            if (Site.Proxy != null && !Site.Proxy.IsBypassed(new Uri(baseAddr)))
                myWebRequest.Proxy = Site.Proxy;

            myWebRequest.ContentType = "application/x-www-form-urlencoded";
            myWebRequest.Method = "POST";
            myWebRequest.ContentLength = message.Length;

            //we send the request
            using (Stream sw = await myWebRequest.GetRequestStreamTaskAsync())
            {
                sw.Write(message, 0, message.Length);
            }

            //we read the response
            try
            {
                var resp = await myWebRequest.GetResponseTaskAsync();
                Console.WriteLine("The server that was hit: " + resp.Headers["Server"]);
            }
            catch (WebException e)
            {
                Console.WriteLine("The server that was hit: " + e.Response.Headers["Server"]);
            }
            string result = string.Empty;
            using (var response = await myWebRequest.GetResponseTaskAsync())
            {
                //we only get the _su cookie if SessionState is null
                su = null;
                if (sessionState == null)
                {
                    var header = response.Headers["Set-Cookie"];
                    var startIndex = header.IndexOf("_su");
                    su = header.Substring(startIndex + 4, header.IndexOf(';', startIndex + 3) - startIndex - 4);
                }

                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }
            }

            var temp = JSONDotNET.Deserializer.Deserialize<SMR>(result);

            return new Tuple<SMR, string>(temp, su);
        }


        private static string Get_suCookie(WebRequest wr)
        {
            string _su = string.Empty;
            using (var resp = wr.GetResponse())
            {
                var setCookie = resp.Headers["Set-Cookie"];
                var posStart = setCookie.IndexOf("_su");
                _su = setCookie.Substring(posStart + 4, setCookie.IndexOf(';', posStart + 3) - posStart - 4);

                using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                {
                    var rez = sr.ReadToEnd();
                }
            }
            return _su;
        }
    }
}

public static class WebRequestExtensions
{
    public static Task<Stream> GetRequestStreamTaskAsync(this WebRequest request)
    {
        return Task.Factory.FromAsync<Stream>(request.BeginGetRequestStream, request.EndGetRequestStream, TaskCreationOptions.None);
    }
    public static Task<WebResponse> GetResponseTaskAsync(this WebRequest request)
    {
        return Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, TaskCreationOptions.None);
    }

}