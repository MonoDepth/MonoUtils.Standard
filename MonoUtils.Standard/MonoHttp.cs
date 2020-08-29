using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MonoUtilities.Http
{
    public class MonoHttpClient
    {
        /*
        * TODO:
        * Add method to add headers (referer, etc)
        */
        
        private readonly List<KeyValuePair<string, string>> postParams;
        private readonly List<KeyValuePair<string, string>> getParams;

        public HttpClient HttpClient { get; private set; }
        public HttpClientHandler ClientHandler { get; private set; }

        public CookieContainer CookieContainer { get; private set; }

        public MonoHttpClient(TimeSpan requestTimeout, bool allowAutoRedirect = true)
        {
            CookieContainer = new CookieContainer();
            ClientHandler = new HttpClientHandler() { AllowAutoRedirect = allowAutoRedirect, CookieContainer = CookieContainer };
            HttpClient = new HttpClient(ClientHandler) { Timeout = requestTimeout };
            HttpClient.Timeout = requestTimeout;
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "MonoHTTP Client C# .NET Standard https://github.com/MonoDepth/MonoUtils.Standard");
            postParams = new List<KeyValuePair<string, string>>();
            getParams = new List<KeyValuePair<string, string>>();
        }
        public MonoHttpClient(double requestTimeoutInMs, bool allowAutoRedirect = true)
        {
            CookieContainer = new CookieContainer();
            ClientHandler = new HttpClientHandler() { AllowAutoRedirect = allowAutoRedirect, CookieContainer = CookieContainer };
            HttpClient = new HttpClient(ClientHandler) { Timeout = TimeSpan.FromMilliseconds(requestTimeoutInMs) };
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "MonoHTTP Client C# .NET Standard https://github.com/MonoDepth/MonoUtils.Standard");
            postParams = new List<KeyValuePair<string, string>>();
            getParams = new List<KeyValuePair<string, string>>();
        }

        public void AddPostParameter(string paramName, object paramValue)
        {
            postParams.Add(new KeyValuePair<string, string>(paramName, paramValue.ToString()));
        }
        public void ClearPostParameters()
        {
            postParams.Clear();
        }

        public void AddGetParameter(string paramName, object paramValue)
        {
            getParams.Add(new KeyValuePair<string, string>(paramName, paramValue.ToString()));
        }
        public void ClearGetParameters()
        {
            getParams.Clear();
        }

        public void AddCookie(string url, string key, object value)
        {
            Cookie c = new Cookie(key, value.ToString());
            CookieContainer.Add(new Uri(url), c);
        }

        public async Task<HttpResponseMessage> PostAsync(string url, bool clearParamsAfterResponse = true)
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(postParams);
            HttpResponseMessage result = await HttpClient.PostAsync(url, content);
            if (clearParamsAfterResponse)
                postParams.Clear();
            return result;
        }

        public async Task<HttpResponseMessage> GetAsync(string url, bool clearParamsAfterResponse = true)
        {
            for (int i = 0; i > getParams.Count; i++)
            {
                if (i == 0)
                    url += "?";
                url += getParams[i].Key + "=" + getParams[i].Value;
                if (i + 1 > getParams.Count)
                    url += "&";
            }
            HttpResponseMessage result = await HttpClient.GetAsync(url);
            if (clearParamsAfterResponse)
                getParams.Clear();
            return result;
        }
    }
}
